# 🏗️ Implementacja Strategii - Dokumentacja Techniczna

## 📂 Struktura Plików

### Backend (.NET 9 / ASP.NET Core)

```
WinWigApp.Server/
├── Models/
│   └── Strategy.cs ............................ Model bazy danych
├── DTOs/
│   ├── CreateStrategyRequest.cs .............. Request do tworzenia/aktualizacji
│   ├── StrategyResponse.cs ................... Response ze strategią
│   ├── BaseResponse.cs ....................... Bazowa odpowiedź
│   └── ToggleStrategyResponse.cs ............ Response dla toggle
├── Services/
│   ├── IStrategyService.cs ................... Interfejs serwisu
│   └── StrategyService.cs .................... Implementacja serwisu
├── Controllers/
│   └── StrategiesController.cs ............... API Endpoints
├── Data/
│   └── WinWigDbContext.cs .................... DbContext (już skonfigurowany)
└── Program.cs ............................... Dependency Injection
```

### Frontend (React + TypeScript)

```
winwigapp.client/
├── src/app/
│   ├── components/
│   │   └── strategies/
│   │       └── Strategies.tsx ............... Główny komponent
│   └── utils/
│       └── strategiesApi.ts ................ API Service
└── STRATEGIES_DOCUMENTATION.md ............ Dokumentacja dla użytkowników
```

---

## 🔧 Szczegóły Implementacji

### 1. Model - Strategy.cs

```csharp
public class Strategy
{
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal TargetReturn { get; set; }
	public int InvestmentHorizon { get; set; }
	public decimal RsiLow { get; set; }
	public decimal RsiHigh { get; set; }
	public bool MacdBuy { get; set; }
	public bool Sma50Above200 { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public User User { get; set; } = null!;
}
```

**Uwagi:**
- `Id` - unikalny identyfikator GUID
- `UserId` - foreign key do użytkownika
- `TargetReturn` - decimal na wypadek różnych walut
- `InvestmentHorizon` - liczbę dni jako int
- `IsActive` - czy strategia jest włączona
- `CreatedAt` - timestamp tworzenia (UTC)

---

### 2. DTOs - Transferowe Obiekty Danych

#### CreateStrategyRequest.cs
```csharp
public class CreateStrategyRequest
{
	public string Name { get; set; } = string.Empty;
	public decimal TargetReturn { get; set; }
	public int InvestmentHorizon { get; set; }
	public decimal RsiLow { get; set; }
	public decimal RsiHigh { get; set; }
	public bool MacdBuy { get; set; }
	public bool Sma50Above200 { get; set; }
}
```

**Używany w:** POST (tworzenie) i PUT (aktualizacja)

#### StrategyResponse.cs
```csharp
public class StrategyResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal TargetReturn { get; set; }
	public int InvestmentHorizon { get; set; }
	public decimal RsiLow { get; set; }
	public decimal RsiHigh { get; set; }
	public bool MacdBuy { get; set; }
	public bool Sma50Above200 { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
}
```

**Zwracany w:** GET (pobieranie), POST (tworzenie)

#### BaseResponse.cs
```csharp
public class BaseResponse
{
	public bool Success { get; set; }
	public string Message { get; set; } = string.Empty;
}
```

**Zwracany w:** PUT (aktualizacja), DELETE (usuwanie)

#### ToggleStrategyResponse.cs
```csharp
public class ToggleStrategyResponse : BaseResponse
{
	public bool IsActive { get; set; }
}
```

**Zwracany w:** PUT toggle

---

### 3. Serwis - StrategyService.cs

#### Interfejs
```csharp
public interface IStrategyService
{
	Task<StrategyResponse> CreateStrategyAsync(Guid userId, CreateStrategyRequest request);
	Task<List<StrategyResponse>> GetUserStrategiesAsync(Guid userId);
	Task<StrategyResponse> GetStrategyByIdAsync(Guid strategyId, Guid userId);
	Task<BaseResponse> UpdateStrategyAsync(Guid strategyId, Guid userId, CreateStrategyRequest request);
	Task<BaseResponse> DeleteStrategyAsync(Guid strategyId, Guid userId);
	Task<ToggleStrategyResponse> ToggleStrategyAsync(Guid strategyId, Guid userId);
}
```

#### Implementacja - Kluczowe Metody

**CreateStrategyAsync:**
```csharp
public async Task<StrategyResponse> CreateStrategyAsync(Guid userId, CreateStrategyRequest request)
{
	ValidateStrategyRequest(request); // Walidacja

	var strategy = new Strategy
	{
		Id = Guid.NewGuid(),
		UserId = userId,
		Name = request.Name,
		TargetReturn = request.TargetReturn,
		InvestmentHorizon = request.InvestmentHorizon,
		RsiLow = request.RsiLow,
		RsiHigh = request.RsiHigh,
		MacdBuy = request.MacdBuy,
		Sma50Above200 = request.Sma50Above200,
		IsActive = false, // Nowa strategia domyślnie wyłączona
		CreatedAt = DateTime.UtcNow
	};

	_context.Strategies.Add(strategy);
	await _context.SaveChangesAsync();

	return MapToResponse(strategy);
}
```

**ValidateStrategyRequest:**
```csharp
private void ValidateStrategyRequest(CreateStrategyRequest request)
{
	if (string.IsNullOrWhiteSpace(request.Name))
		throw new InvalidOperationException("Nazwa strategii jest wymagana");

	if (request.TargetReturn <= 0)
		throw new InvalidOperationException("Planowana stopa zwrotu musi być większa od zera");

	if (request.InvestmentHorizon <= 0)
		throw new InvalidOperationException("Horyzont inwestycyjny musi być większy od zera");

	if (request.RsiLow < 0 || request.RsiLow > 100)
		throw new InvalidOperationException("RSI niski musi być między 0 a 100");

	if (request.RsiHigh < 0 || request.RsiHigh > 100)
		throw new InvalidOperationException("RSI wysoki musi być między 0 a 100");

	if (request.RsiLow >= request.RsiHigh)
		throw new InvalidOperationException("RSI niski musi być mniejszy niż RSI wysoki");
}
```

---

### 4. Kontroler - StrategiesController.cs

#### Struktura
```csharp
[ApiController]
[Route("api/strategies")]
[Authorize] // Wymagana autoryzacja JWT
public class StrategiesController : ControllerBase
{
	private Guid GetUserId() // Pobranie UserId z JWT
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
		if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
			throw new UnauthorizedAccessException("Nie można pobrać ID użytkownika");
		return userId;
	}
}
```

#### Endpointy

**POST /api/strategies**
```csharp
[HttpPost]
public async Task<ActionResult<StrategyResponse>> CreateStrategy([FromBody] CreateStrategyRequest request)
{
	if (!ModelState.IsValid)
		return BadRequest(ModelState);

	var userId = GetUserId();
	var response = await _strategyService.CreateStrategyAsync(userId, request);
	return CreatedAtAction(nameof(GetStrategy), new { id = response.Id }, response);
}
```

**GET /api/strategies**
```csharp
[HttpGet]
public async Task<ActionResult<List<StrategyResponse>>> GetStrategies()
{
	var userId = GetUserId();
	var response = await _strategyService.GetUserStrategiesAsync(userId);
	return Ok(response);
}
```

**GET /api/strategies/{id}**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<StrategyResponse>> GetStrategy(Guid id)
{
	var userId = GetUserId();
	var response = await _strategyService.GetStrategyByIdAsync(id, userId);
	return Ok(response);
}
```

**PUT /api/strategies/{id}**
```csharp
[HttpPut("{id}")]
public async Task<ActionResult<BaseResponse>> UpdateStrategy(Guid id, [FromBody] CreateStrategyRequest request)
{
	if (!ModelState.IsValid)
		return BadRequest(ModelState);

	var userId = GetUserId();
	var response = await _strategyService.UpdateStrategyAsync(id, userId, request);
	return Ok(response);
}
```

**DELETE /api/strategies/{id}**
```csharp
[HttpDelete("{id}")]
public async Task<ActionResult<BaseResponse>> DeleteStrategy(Guid id)
{
	var userId = GetUserId();
	var response = await _strategyService.DeleteStrategyAsync(id, userId);
	return Ok(response);
}
```

**PUT /api/strategies/{id}/toggle**
```csharp
[HttpPut("{id}/toggle")]
public async Task<ActionResult<ToggleStrategyResponse>> ToggleStrategy(Guid id)
{
	var userId = GetUserId();
	var response = await _strategyService.ToggleStrategyAsync(id, userId);
	return Ok(response);
}
```

---

### 5. Dependency Injection - Program.cs

```csharp
// Dodane do Program.cs
builder.Services.AddScoped<IStrategyService, StrategyService>();
```

**Gdzie dodać:**
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// ... inne serwisy ...
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IStrategyService, StrategyService>(); // ← TUTAJ
```

---

### 6. Frontend - API Service (strategiesApi.ts)

```typescript
export interface CreateStrategyRequest {
  name: string;
  targetReturn: number;
  investmentHorizon: number;
  rsiLow: number;
  rsiHigh: number;
  macdBuy: boolean;
  sma50Above200: boolean;
}

export interface StrategyResponse {
  id: string;
  name: string;
  targetReturn: number;
  investmentHorizon: number;
  rsiLow: number;
  rsiHigh: number;
  macdBuy: boolean;
  sma50Above200: boolean;
  isActive: boolean;
  createdAt: string;
}
```

**Metody:**
```typescript
export const strategiesApi = {
  async createStrategy(request: CreateStrategyRequest): Promise<StrategyResponse> { },
  async getStrategies(): Promise<StrategyResponse[]> { },
  async getStrategy(id: string): Promise<StrategyResponse> { },
  async updateStrategy(id: string, request: CreateStrategyRequest): Promise<BaseResponse> { },
  async deleteStrategy(id: string): Promise<BaseResponse> { },
  async toggleStrategy(id: string): Promise<ToggleStrategyResponse> { },
};
```

---

### 7. Frontend - Component (Strategies.tsx)

**Zmieniony kod:**

1. **Import API Service:**
```typescript
import { strategiesApi, StrategyResponse as Strategy } from "../../utils/strategiesApi";
```

2. **Załadowanie danych:**
```typescript
const loadStrategies = async () => {
  try {
	const data = await strategiesApi.getStrategies();
	setStrategies(data);
  } catch (error) {
	toast.error("Nie udało się załadować strategii");
  }
};
```

3. **Tworzenie strategii:**
```typescript
const handleSubmit = async (e: React.FormEvent) => {
  // Walidacja...
  const requestData = {
	name: formData.name,
	targetReturn,
	investmentHorizon,
	rsiLow,
	rsiHigh,
	macdBuy: formData.macdBuy,
	sma50Above200: formData.sma50Above200,
  };

  if (editingStrategy) {
	await strategiesApi.updateStrategy(editingStrategy.id, requestData);
  } else {
	const newStrategy = await strategiesApi.createStrategy(requestData);
	setStrategies([newStrategy, ...strategies]);
  }

  await loadStrategies(); // Odśwież listę
};
```

4. **Usuwanie i toggle:**
```typescript
const handleDelete = async (id: string) => {
  if (confirm("Czy na pewno?")) {
	await strategiesApi.deleteStrategy(id);
	setStrategies(strategies.filter((s) => s.id !== id));
  }
};

const toggleActive = async (id: string) => {
  const response = await strategiesApi.toggleStrategy(id);
  const updated = strategies.map((s) =>
	s.id === id ? { ...s, isActive: response.isActive } : s
  );
  setStrategies(updated);
};
```

---

## 🔐 Bezpieczeństwo

### 1. Autoryzacja
- Wszystkie endpointy wymagają JWT token w header'e `Authorization: Bearer <token>`
- Backend waliduje token za pomocą middleware'u `[Authorize]`

### 2. Izolacja danych
- Każdy użytkownik widzi tylko swoje strategie
- Backend sprawdza `UserId` przed zwróceniem danych
- Baza danych ma constraint `ON DELETE CASCADE` - jeśli usunięty user, usuniętą jego strategie

### 3. Walidacja
- Request DTO validation za pomocą `ModelState.IsValid`
- Business logic validation w `StrategyService`
- Błędy zwracane jako JSON

---

## 🧪 Testy

### Postman Collection

```json
{
  "info": {
	"name": "Strategies API",
	"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
	{
	  "name": "Create Strategy",
	  "request": {
		"method": "POST",
		"header": [
		  {
			"key": "Authorization",
			"value": "Bearer {{token}}"
		  },
		  {
			"key": "Content-Type",
			"value": "application/json"
		  }
		],
		"body": {
		  "mode": "raw",
		  "raw": "{\n  \"name\": \"Test Strategy\",\n  \"targetReturn\": 10,\n  \"investmentHorizon\": 30,\n  \"rsiLow\": 30,\n  \"rsiHigh\": 70,\n  \"macdBuy\": true,\n  \"sma50Above200\": false\n}"
		},
		"url": {
		  "raw": "{{baseUrl}}/api/strategies",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies"]
		}
	  }
	},
	{
	  "name": "Get All Strategies",
	  "request": {
		"method": "GET",
		"header": [
		  {
			"key": "Authorization",
			"value": "Bearer {{token}}"
		  }
		],
		"url": {
		  "raw": "{{baseUrl}}/api/strategies",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies"]
		}
	  }
	},
	{
	  "name": "Toggle Strategy",
	  "request": {
		"method": "PUT",
		"header": [
		  {
			"key": "Authorization",
			"value": "Bearer {{token}}"
		  }
		],
		"url": {
		  "raw": "{{baseUrl}}/api/strategies/{{strategyId}}/toggle",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies", "{{strategyId}}", "toggle"]
		}
	  }
	}
  ]
}
```

---

## 📊 Fluxowy Diagramy

### Tworzenie Strategii
```
User Input Form
	↓
Validate Input (Frontend)
	↓
POST /api/strategies (Backend)
	↓
Validate DTO (Backend)
	↓
Create Strategy Object
	↓
Save to Database
	↓
Return StrategyResponse
	↓
Update UI
```

### Pobieranie Strategii
```
Component Mount / Page Load
	↓
GET /api/strategies (Backend)
	↓
Get User ID from JWT
	↓
Query Database (WHERE UserId = ...)
	↓
Map to StrategyResponse
	↓
Return List<StrategyResponse>
	↓
Display in UI
```

---

## 🐛 Troubleshooting

### Problem: "401 Unauthorized"
**Przyczyna:** Brak lub wygasły token JWT
**Rozwiązanie:** Zaloguj się ponownie, uzyskaj nowy token

### Problem: "404 Strategy not found"
**Przyczyna:** Strategia o podanym ID nie istnieje lub należy do innego użytkownika
**Rozwiązanie:** Sprawdź ID, upewnij się że to twoja strategia

### Problem: "RSI niski musi być mniejszy niż RSI wysoki"
**Przyczyna:** Wartość RsiLow >= RsiHigh
**Rozwiązanie:** Ustaw np. RsiLow=30, RsiHigh=70

### Problem: Baza danych nie istnieje
**Przyczyna:** Migracje nie zostały uruchomione
**Rozwiązanie:** 
```bash
dotnet ef database update
```

---

## 🚀 Deployment

### Backend
```bash
# Build
dotnet publish -c Release

# Deploy na Azure / Docker
docker build -t winwigapp:latest .
docker run -d -p 5000:5000 winwigapp:latest
```

### Frontend
```bash
# Build
npm run build

# Deploy na vercel / netlify
npm install -g vercel
vercel
```

---

Ostatnia aktualizacja: 2026-05-04
Wersja: 1.0
