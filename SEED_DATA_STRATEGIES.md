# 🌱 Seed Data - Domyślne Strategie

## 📋 Overview

Nowi użytkownicy automatycznie otrzymują **3 domyślne strategie** zaraz po rejestracji:

1. **Bezpieczna dla Początkujących**
2. **Zbilansowana**
3. **Agresywna - Łap Upadające Noże**

---

## 🎯 Trzy Domyślne Strategie

### 1️⃣ Bezpieczna dla Początkujących

```json
{
  "name": "Bezpieczna dla Początkujących",
  "targetReturn": 8,
  "investmentHorizon": 45,
  "rsiLow": 35,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": true,
  "isActive": false
}
```

**Charakterystyka:**
- ✅ Najmniej sygnałów
- ✅ Najwyższy hit rate
- ✅ Najmniejsze ryzyko
- ✅ Idealna dla początkujących
- ⏱️ Długi horyzont (45 dni)
- 📈 Niski cel (8%)

---

### 2️⃣ Zbilansowana

```json
{
  "name": "Zbilansowana",
  "targetReturn": 12,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": true,
  "isActive": false
}
```

**Charakterystyka:**
- ⚖️ Średnia liczba sygnałów
- ⚖️ Średni hit rate
- ⚖️ Średnie ryzyko
- ⚖️ Najlepsza dla większości
- ⏱️ Umiarkowany horyzont (30 dni)
- 📈 Umiarkowany cel (12%)

---

### 3️⃣ Agresywna - Łap Upadające Noże

```json
{
  "name": "Agresywna - Łap Upadające Noże",
  "targetReturn": 15,
  "investmentHorizon": 20,
  "rsiLow": 20,
  "rsiHigh": 75,
  "macdBuy": false,
  "sma50Above200": false,
  "isActive": false
}
```

**Charakterystyka:**
- ⚠️ Wiele sygnałów
- ⚠️ Niższy hit rate
- ⚠️ Wyższe ryzyko
- ⚠️ Dla doświadczonych
- ⏱️ Krótki horyzont (20 dni)
- 📈 Wysoki cel (15%)

---

## 🛠️ Implementacja Techniczna

### 1. SeederService (Nowy)

```csharp
public interface ISeederService
{
	Task SeedDefaultStrategiesAsync(Guid userId);
}

public class SeederService : ISeederService
{
	public async Task SeedDefaultStrategiesAsync(Guid userId)
	{
		// Sprawdź czy użytkownik już ma strategie
		if (_context.Strategies.Any(s => s.UserId == userId))
			return;

		// Utwórz 3 domyślne strategie
		var strategies = new List<Strategy> { /* ... */ };
		_context.Strategies.AddRange(strategies);
		await _context.SaveChangesAsync();
	}
}
```

### 2. AuthService (Zmodyfikowany)

```csharp
public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
{
	// Utwórz użytkownika
	var user = new User { /* ... */ };
	_context.Users.Add(user);
	await _context.SaveChangesAsync();

	// Seed domyślne strategie
	await _seederService.SeedDefaultStrategiesAsync(user.Id);

	// Zwróć token
	return new AuthResponse { /* ... */ };
}
```

### 3. Program.cs (Zmodyfikowany)

```csharp
// Dodana rejestracja w DI
builder.Services.AddScoped<ISeederService, SeederService>();
```

---

## 📊 Flow Procesu

```
1. Użytkownik kliknie "Register"
   │
2. AuthController.Register() wywoływany
   │
3. AuthService.RegisterAsync():
   ├─ Utwórz nowego użytkownika
   ├─ Zapisz do bazy
   ├─ Wywołaj ISeederService
   │
4. SeederService.SeedDefaultStrategiesAsync():
   ├─ Sprawdź czy ma już strategie
   ├─ Jeśli nie, utwórz 3 domyślne
   ├─ Dodaj do DbContext
   └─ SaveChanges()
   │
5. Zwróć JWT token i dane użytkownika
   │
6. Frontend pokazuje "Zalogowano!"
   │
7. Użytkownik otwiera "Strategie inwestycyjne"
   │
8. GET /api/strategies pobiera 3 domyślne strategie
   │
9. ✅ Wyświetlane na liście!
```

---

## ✅ Workflow Użytkownika

```
Nowy Użytkownik:

1. Rejestracja
   └─ Automatycznie otrzymuje 3 strategie

2. Login
   └─ Widzi 3 domyślne strategie

3. Może teraz:
   ├─ Włączyć strategię [Uruchom]
   ├─ Edytować strategię [Edytuj]
   ├─ Usunąć strategię [Usuń]
   └─ Stworzyć nową strategię [+ Nowa]

4. Kliknął [Uruchom] na Zbilansowanej
   └─ Strategia jest aktywna i monitoruje rynek!
```

---

## 🔍 Szczegóły Implementacji

### SeederService.cs

**Plik:** `WinWigApp.Server/Services/ISeederService.cs`

```csharp
public interface ISeederService
{
	Task SeedDefaultStrategiesAsync(Guid userId);
}

public class SeederService : ISeederService
{
	private readonly WinWigDbContext _context;
	private readonly ILogger<SeederService> _logger;

	public SeederService(WinWigDbContext context, ILogger<SeederService> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task SeedDefaultStrategiesAsync(Guid userId)
	{
		try
		{
			// Sprawdzenie czy użytkownik już ma strategie
			var existingStrategies = _context.Strategies.Any(s => s.UserId == userId);
			if (existingStrategies)
			{
				_logger.LogInformation("User {UserId} already has strategies, skipping seed", userId);
				return;
			}

			var strategies = new List<Strategy>
			{
				// Strategia 1: Bezpieczna
				new()
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					Name = "Bezpieczna dla Początkujących",
					TargetReturn = 8m,
					InvestmentHorizon = 45,
					RsiLow = 35m,
					RsiHigh = 70m,
					MacdBuy = true,
					Sma50Above200 = true,
					IsActive = false,
					CreatedAt = DateTime.UtcNow
				},

				// Strategia 2: Zbilansowana
				new()
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					Name = "Zbilansowana",
					TargetReturn = 12m,
					InvestmentHorizon = 30,
					RsiLow = 30m,
					RsiHigh = 70m,
					MacdBuy = true,
					Sma50Above200 = true,
					IsActive = false,
					CreatedAt = DateTime.UtcNow
				},

				// Strategia 3: Agresywna
				new()
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					Name = "Agresywna - Łap Upadające Noże",
					TargetReturn = 15m,
					InvestmentHorizon = 20,
					RsiLow = 20m,
					RsiHigh = 75m,
					MacdBuy = false,
					Sma50Above200 = false,
					IsActive = false,
					CreatedAt = DateTime.UtcNow
				}
			};

			_context.Strategies.AddRange(strategies);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Successfully seeded 3 default strategies for user {UserId}", userId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error seeding default strategies for user {UserId}", userId);
			throw;
		}
	}
}
```

---

## 🧪 Testowanie

### Test 1: Rejestracja Nowego Użytkownika

```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "firstName": "Jan",
  "lastName": "Kowalski",
  "email": "jan@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGc...",
  "user": {
	"id": "abc123...",
	"firstName": "Jan",
	"lastName": "Kowalski",
	"email": "jan@example.com",
	"balance": 0
  }
}
```

### Test 2: Pobierz Strategie Nowego Użytkownika

```bash
GET http://localhost:5000/api/strategies
Authorization: Bearer eyJhbGc...
```

**Response:**
```json
[
  {
	"id": "uuid1",
	"name": "Bezpieczna dla Początkujących",
	"targetReturn": 8,
	"investmentHorizon": 45,
	"rsiLow": 35,
	"rsiHigh": 70,
	"macdBuy": true,
	"sma50Above200": true,
	"isActive": false,
	"createdAt": "2026-05-04T10:30:00Z"
  },
  {
	"id": "uuid2",
	"name": "Zbilansowana",
	"targetReturn": 12,
	"investmentHorizon": 30,
	"rsiLow": 30,
	"rsiHigh": 70,
	"macdBuy": true,
	"sma50Above200": true,
	"isActive": false,
	"createdAt": "2026-05-04T10:30:00Z"
  },
  {
	"id": "uuid3",
	"name": "Agresywna - Łap Upadające Noże",
	"targetReturn": 15,
	"investmentHorizon": 20,
	"rsiLow": 20,
	"rsiHigh": 75,
	"macdBuy": false,
	"sma50Above200": false,
	"isActive": false,
	"createdAt": "2026-05-04T10:30:00Z"
  }
]
```

✅ **Wszystkie 3 strategie zostały zwrócone!**

---

## 🛡️ Bezpieczeństwo

### Zmienne Warunki:
- ✅ Każdy użytkownik dostaje własne kopie strategii
- ✅ Strategie mają UserId = Id nowego użytkownika
- ✅ Nie mogą widzieć strategii innych użytkowników
- ✅ Strategie domyślne NIE są edytowalne globalnie

### Idempotentność:
- ✅ Jeśli użytkownik już ma strategie → nie dodawaj więcej
- ✅ Zapobiega duplikatom
- ✅ Bezpieczne dla retrytów

---

## 📊 Scenariusze Użycia

### Scenariusz 1: Całkowicie Nowy Użytkownik

```
1. Rejestracja
2. ✅ Otrzymuje 3 domyślne strategie
3. Zaloguje się
4. ✅ Widzi 3 strategie na liście
5. Edytuje lub tworzy nowe
```

### Scenariusz 2: Użytkownik Chce Reset Strategii

```
1. Usuwa wszystkie strategie
2. Pozostaje bez strategii
3. Może ręcznie odtworzyć z documentation
UWAGA: Seeder działa tylko przy rejestracji!
```

**Rozwiązanie:** Dodać endpoint `/api/strategies/reset-defaults`

---

## 🎯 Przyszłe Rozszerzenia

### 1. Endpoint Reset Strategii
```csharp
[HttpPost("reset-defaults")]
public async Task<IActionResult> ResetDefaults()
{
	var userId = GetUserId();
	await _seederService.SeedDefaultStrategiesAsync(userId);
	return Ok(new { message = "Strategie zostały zresetowane" });
}
```

### 2. Admin Panel - Edycja Domyślnych Strategii
- Admin może zmienić parametry domyślnych strategii
- Nowi użytkownicy otrzymują zmienione wartości

### 3. A/B Testing
- Różne grupy użytkowników otrzymują różne strategie
- Mierzyć jak parametry wpływają na zaangażowanie

---

## ✅ Checklist

- [x] SeederService stworzony
- [x] ISeederService interfejs
- [x] AuthService zmodyfikowany
- [x] Program.cs DI dodane
- [x] 3 strategie zdefiniowane
- [x] Logging added
- [x] Error handling
- [x] Dokumentacja

---

## 📝 Notatki

- Seeder uruchamia się **tylko** przy rejestracji
- Każdy użytkownik dostaje **własne kopie** (nie shared)
- Strategie są **nieaktywne** domyślnie
- Parametry **nie mogą być zmieniane** globalnie (na razie)
- Wszystkie strategie mają **isActive = false**

---

Ostatnia aktualizacja: 2026-05-04  
Status: ✅ Implementacja Kompletna
