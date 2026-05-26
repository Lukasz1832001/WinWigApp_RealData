using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using WinWigApp.Server.Data;
using WinWigApp.Server.DTOs;
using WinWigApp.Server.Models;

namespace WinWigApp.Server.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly WinWigDbContext _context;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(WinWigDbContext context, ILogger<TransactionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Nie można pobrać ID użytkownika");
        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Get user
            var user = _context.Users.Find(userId);
            if (user == null)
                return NotFound(new { message = "Użytkownik nie znaleziony" });

            // Validate request
            if (request.Quantity <= 0)
                return BadRequest(new { message = "Ilość akcji musi być większa niż 0" });

            if (request.Price <= 0)
                return BadRequest(new { message = "Cena musi być większa niż 0" });

            // Get stock (case-insensitive). If missing, create a minimal placeholder so user can trade.
            var symbolNormalized = (request.Symbol ?? string.Empty).Trim();
            var stock = _context.Stocks
                .FirstOrDefault(s => s.Symbol.ToUpper() == symbolNormalized.ToUpper());

            if (stock == null)
            {
                // Create a minimal stock record using provided price so transactions can proceed
                stock = new Stock
                {
                    Symbol = symbolNormalized.ToUpper(),
                    Name = symbolNormalized.ToUpper(),
                    CurrentPrice = request.Price,
                    OpenPrice = request.Price,
                    ClosePrice = request.Price,
                    UpdatedAt = DateTime.UtcNow,
                };
                _context.Stocks.Add(stock);
            }

            decimal totalValue = request.Quantity * request.Price;

            // Check balance for buy transactions
            if (request.Type == "buy" && user.Balance < totalValue)
                return BadRequest(new { message = "Niewystarczające środki na koncie" });

            // Create transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Symbol = symbolNormalized.ToUpper(),
                Name = stock.Name,
                Type = request.Type == "buy" ? TransactionType.Buy : TransactionType.Sell,
                Quantity = request.Quantity,
                Price = request.Price,
                Total = totalValue,
                StopLoss = request.StopLoss,
                Timestamp = DateTime.UtcNow
            };

            // Update user balance
            if (request.Type == "buy")
            {
                user.Balance -= totalValue;
            }
            else
            {
                user.Balance += totalValue;
            }

            // Update portfolio
            var portfolioItem = _context.Portfolios
                .FirstOrDefault(p => p.UserId == userId && p.Symbol.ToUpper() == symbolNormalized.ToUpper());

            if (request.Type == "buy")
            {
                if (portfolioItem == null)
                {
                    portfolioItem = new Portfolio
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Symbol = symbolNormalized.ToUpper(),
                        Name = stock.Name,
                        Quantity = request.Quantity,
                        AvgPrice = request.Price,
                        StopLoss = request.StopLoss
                    };
                    _context.Portfolios.Add(portfolioItem);
                }
                else
                {
                    // Calculate new average price
                    decimal oldTotal = portfolioItem.AvgPrice * portfolioItem.Quantity;
                    decimal newTotal = oldTotal + (request.Price * request.Quantity);
                    portfolioItem.Quantity += request.Quantity;
                    portfolioItem.AvgPrice = newTotal / portfolioItem.Quantity;

                    if (request.StopLoss.HasValue)
                    {
                        portfolioItem.StopLoss = request.StopLoss.Value;
                    }
                }
            }
            else // sell
            {
                if (portfolioItem != null)
                {
                    portfolioItem.Quantity -= request.Quantity;
                    if (portfolioItem.Quantity <= 0)
                    {
                        _context.Portfolios.Remove(portfolioItem);
                    }
                }
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var response = new
            {
                id = transaction.Id,
                symbol = transaction.Symbol,
                name = transaction.Name,
                type = transaction.Type.ToString().ToLower(),
                quantity = transaction.Quantity,
                price = transaction.Price,
                total = transaction.Total,
                stopLoss = transaction.StopLoss,
                timestamp = transaction.Timestamp,
                newBalance = user.Balance
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized transaction");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Błąd serwera podczas tworzenia transakcji" });
        }
    }

    [HttpGet]
    public IActionResult GetTransactions()
    {
        try
        {
            var userId = GetUserId();

            // Get all transactions for the user
            var transactions = _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Timestamp)
                .ToList();

            // Get current portfolio positions to get latest stop loss values
            var portfolioItems = _context.Portfolios
                .Where(p => p.UserId == userId)
                .ToList();

            // Map transactions to response with current stop loss from portfolio
            var result = transactions.Select(t => 
            {
                // Get current stop loss from portfolio position (if position is still open)
                var currentStopLoss = portfolioItems
                    .FirstOrDefault(p => p.Symbol.ToUpper() == t.Symbol.ToUpper())
                    ?.StopLoss;

                return new
                {
                    id = t.Id,
                    symbol = t.Symbol,
                    name = t.Name,
                    type = t.Type.ToString().ToLower(),
                    quantity = t.Quantity,
                    price = t.Price,
                    total = t.Total,
                    stopLoss = currentStopLoss, // Use current stop loss from portfolio
                    timestamp = t.Timestamp
                };
            })
            .ToList();

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to transactions");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Błąd serwera podczas pobierania transakcji" });
        }
    }
}

public class CreateTransactionRequest
{
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty; // "buy" or "sell"
    public decimal? StopLoss { get; set; }
}
