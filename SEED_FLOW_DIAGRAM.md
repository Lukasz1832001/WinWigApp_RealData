# 📊 Diagram Flow - Seeding Strategii

## 🔄 Workflow Rejestracji z Seedowaniem

```
┌─────────────────────────────────────────────────────────────────┐
│                      FRONTEND (React)                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Strona Rejestracji                                             │
│  ┌───────────────────────────────┐                              │
│  │ Imię:      [____________]     │                              │
│  │ Nazwisko:  [____________]     │                              │
│  │ Email:     [____________]     │                              │
│  │ Hasło:     [____________]     │                              │
│  │                               │                              │
│  │            [REGISTER]         │                              │
│  └───────────────────────────────┘                              │
│              │                                                   │
│              │ POST /api/auth/register                          │
│              │ { firstName, lastName, email, password }        │
│              │                                                   │
│              ▼                                                   │
└─────────────────────────────────────────────────────────────────┘
			  │
			  │ HTTP POST
			  │
			  ▼
┌─────────────────────────────────────────────────────────────────┐
│              BACKEND (ASP.NET Core / .NET 9)                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  AuthController.Register()                                      │
│  │                                                               │
│  └─▶ AuthService.RegisterAsync()                               │
│      │                                                           │
│      ├─ Sprawdź czy email istnieje                             │
│      │  └─ Jeśli tak → wyrzuć InvalidOperationException       │
│      │                                                           │
│      ├─ Utwórz nowy User object                                │
│      │  ├─ Id = Guid.NewGuid()                                 │
│      │  ├─ FirstName = "Jan"                                   │
│      │  ├─ LastName = "Kowalski"                               │
│      │  ├─ Email = "jan@example.com"                           │
│      │  ├─ PasswordHash = BCrypt(hasło)                        │
│      │  ├─ Balance = 0                                         │
│      │  └─ CreatedAt = DateTime.UtcNow                         │
│      │                                                           │
│      ├─ _context.Users.Add(user)                               │
│      │                                                           │
│      ├─ await _context.SaveChangesAsync() ← USER SAVED         │
│      │                                                           │
│      │  ┌─────────────────────────────────────────────────────┐ │
│      │  │ POINT: USER NOW EXISTS IN DATABASE                 │ │
│      │  └─────────────────────────────────────────────────────┘ │
│      │                                                           │
│      └─▶ ISeederService.SeedDefaultStrategiesAsync(user.Id) ← NEW
│          │                                                       │
│          └─ SeederService.SeedDefaultStrategiesAsync()         │
│             │                                                   │
│             ├─ Sprawdź: czy user.Id ma już strategie?        │
│             │  └─ SELECT COUNT(*) WHERE UserId = user.Id     │
│             │                                                   │
│             ├─ Jeśli HAS ANY → return (idempotent)           │
│             │                                                   │
│             └─ Jeśli NOT ANY → CREATE 3 STRATEGIES:          │
│                │                                               │
│                ├─ Strategy 1: "Bezpieczna dla Początkujących" │
│                │  ├─ Id = Guid.NewGuid()                     │
│                │  ├─ UserId = user.Id ← LINKED TO USER!      │
│                │  ├─ Name = "Bezpieczna..."                  │
│                │  ├─ TargetReturn = 8m                       │
│                │  ├─ InvestmentHorizon = 45                  │
│                │  ├─ RsiLow = 35m                            │
│                │  ├─ RsiHigh = 70m                           │
│                │  ├─ MacdBuy = true                          │
│                │  ├─ Sma50Above200 = true                    │
│                │  ├─ IsActive = false                        │
│                │  └─ CreatedAt = DateTime.UtcNow            │
│                │                                               │
│                ├─ Strategy 2: "Zbilansowana"                │
│                │  ├─ Id = Guid.NewGuid()                     │
│                │  ├─ UserId = user.Id ← LINKED TO USER!      │
│                │  ├─ Name = "Zbilansowana"                   │
│                │  ├─ TargetReturn = 12m                      │
│                │  ├─ InvestmentHorizon = 30                  │
│                │  ├─ RsiLow = 30m                            │
│                │  ├─ RsiHigh = 70m                           │
│                │  ├─ MacdBuy = true                          │
│                │  ├─ Sma50Above200 = true                    │
│                │  ├─ IsActive = false                        │
│                │  └─ CreatedAt = DateTime.UtcNow            │
│                │                                               │
│                ├─ Strategy 3: "Agresywna - Łap Upadające..."│
│                │  ├─ Id = Guid.NewGuid()                     │
│                │  ├─ UserId = user.Id ← LINKED TO USER!      │
│                │  ├─ Name = "Agresywna..."                   │
│                │  ├─ TargetReturn = 15m                      │
│                │  ├─ InvestmentHorizon = 20                  │
│                │  ├─ RsiLow = 20m                            │
│                │  ├─ RsiHigh = 75m                           │
│                │  ├─ MacdBuy = false                         │
│                │  ├─ Sma50Above200 = false                   │
│                │  ├─ IsActive = false                        │
│                │  └─ CreatedAt = DateTime.UtcNow            │
│                │                                               │
│                ├─ _context.Strategies.AddRange(strategies)    │
│                │                                               │
│                └─ await _context.SaveChangesAsync()           │
│                   ← 3 STRATEGIES SAVED!                        │
│                                                                 │
│      Generate JWT Token                                         │
│      └─ token = _tokenService.GenerateToken(user)             │
│                                                                 │
│      Return AuthResponse                                        │
│      └─ { token, user }                                        │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
			  │
			  │ HTTP 200 OK
			  │ { token: "eyJ...", user: {...} }
			  │
			  ▼
┌─────────────────────────────────────────────────────────────────┐
│                      FRONTEND (React)                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Response Handler:                                              │
│  ├─ Pobierz token                                              │
│  ├─ Zapisz w localStorage                                      │
│  ├─ Redirect na /dashboard                                     │
│  │                                                               │
│  └─ Toast: "Zalogowano pomyślnie!"                             │
│                                                                 │
│  Użytkownik widzi:                                              │
│  ├─ Dashboard                                                   │
│  └─ Menu: "Strategie inwestycyjne"                             │
│                                                                 │
│      Kliknął "Strategie inwestycyjne"                          │
│      │                                                           │
│      └─ GET /api/strategies                                    │
│         (z tokenem JWT w header)                              │
│         │                                                       │
│         ▼                                                       │
│      Backend:                                                   │
│      ├─ StrategiesController.GetStrategies()                  │
│      ├─ Extract UserId z JWT                                  │
│      ├─ StrategyService.GetUserStrategiesAsync(userId)        │
│      ├─ SELECT * FROM Strategies WHERE UserId = userId        │
│      ├─ ✅ ZWRACA 3 STRATEGIE!                               │
│      │                                                           │
│      └─ Response: [Strategy1, Strategy2, Strategy3]           │
│         │                                                       │
│         └─ Render 3 karty na UI:                              │
│            │                                                    │
│            ├─ ┌──────────────────────────────────────────┐    │
│            │  │ 🟢 Bezpieczna dla Początkujących        │    │
│            │  │                                          │    │
│            │  │ 🎯 Cel: +8%                            │    │
│            │  │ 📅 Horyzont: 45 dni                    │    │
│            │  │ 📊 RSI: 35 - 70                        │    │
│            │  │                                          │    │
│            │  │ ⏸ Nieaktywna                            │    │
│            │  │ [Uruchom] [Edytuj] [Usuń]              │    │
│            │  └──────────────────────────────────────────┘    │
│            │                                                    │
│            ├─ ┌──────────────────────────────────────────┐    │
│            │  │ 🟡 Zbilansowana                         │    │
│            │  │                                          │    │
│            │  │ 🎯 Cel: +12%                           │    │
│            │  │ 📅 Horyzont: 30 dni                    │    │
│            │  │ 📊 RSI: 30 - 70                        │    │
│            │  │                                          │    │
│            │  │ ⏸ Nieaktywna                            │    │
│            │  │ [Uruchom] [Edytuj] [Usuń]              │    │
│            │  └──────────────────────────────────────────┘    │
│            │                                                    │
│            └─ ┌──────────────────────────────────────────┐    │
│               │ 🔴 Agresywna - Łap Upadające Noże        │    │
│               │                                          │    │
│               │ 🎯 Cel: +15%                           │    │
│               │ 📅 Horyzont: 20 dni                    │    │
│               │ 📊 RSI: 20 - 75                        │    │
│               │                                          │    │
│               │ ⏸ Nieaktywna                            │    │
│               │ [Uruchom] [Edytuj] [Usuń]              │    │
│               └──────────────────────────────────────────┘    │
│                                                                 │
│  ✅ SUCCESS! Nowy użytkownik ma 3 domyślne strategie!         │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🗄️ Database Schema

```
PRZED Seedowaniem:
┌─────────────────────────────┐
│ Users                       │
├─────────────────────────────┤
│ Id: abc-123                 │
│ Email: jan@example.com      │
│ FirstName: Jan              │
│ LastName: Kowalski          │
│ Balance: 0                  │
│ CreatedAt: 2026-05-04       │
└─────────────────────────────┘

┌─────────────────────────────┐
│ Strategies                  │
├─────────────────────────────┤
│ (puste)                     │
└─────────────────────────────┘


PO Seedowaniu:
┌─────────────────────────────┐
│ Users                       │
├─────────────────────────────┤
│ Id: abc-123                 │
│ Email: jan@example.com      │
│ FirstName: Jan              │
│ LastName: Kowalski          │
│ Balance: 0                  │
│ CreatedAt: 2026-05-04       │
└─────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ Strategies                                                   │
├──────────────────────────────────────────────────────────────┤
│ Id: uuid-1 | UserId: abc-123 | Name: Bezpieczna...         │
│ Id: uuid-2 | UserId: abc-123 | Name: Zbilansowana          │
│ Id: uuid-3 | UserId: abc-123 | Name: Agresywna...          │
└──────────────────────────────────────────────────────────────┘
	 ▲                         ▲
	 │                         │
	 └─────────────────────────┘
	   ONE-TO-MANY: 1 User : 3 Strategies
```

---

## 🔍 Kod Trwania Zapytania

### SQL Generated:

```sql
-- KROK 1: INSERT USER
INSERT INTO Users (Id, FirstName, LastName, Email, PasswordHash, Balance, CreatedAt)
VALUES ('abc-123-...', 'Jan', 'Kowalski', 'jan@example.com', 'BCrypt_Hash...', 0, '2026-05-04T10:30:00Z');

-- KROK 2: COMMIT TRANSACTION
COMMIT;

-- KROK 3: CHECK EXISTING STRATEGIES
SELECT COUNT(*) FROM Strategies WHERE UserId = 'abc-123-...';
-- Result: 0 (nie ma żadnych)

-- KROK 4: INSERT 3 STRATEGIES
INSERT INTO Strategies (Id, UserId, Name, TargetReturn, InvestmentHorizon, RsiLow, RsiHigh, MacdBuy, Sma50Above200, IsActive, CreatedAt)
VALUES 
  ('uuid-1-...', 'abc-123-...', 'Bezpieczna dla Początkujących', 8.0, 45, 35.0, 70.0, 1, 1, 0, '2026-05-04T10:30:00Z'),
  ('uuid-2-...', 'abc-123-...', 'Zbilansowana', 12.0, 30, 30.0, 70.0, 1, 1, 0, '2026-05-04T10:30:00Z'),
  ('uuid-3-...', 'abc-123-...', 'Agresywna - Łap Upadające Noże', 15.0, 20, 20.0, 75.0, 0, 0, 0, '2026-05-04T10:30:00Z');

-- KROK 5: COMMIT TRANSACTION
COMMIT;

-- KROK 6: GET STRATEGIES
SELECT * FROM Strategies WHERE UserId = 'abc-123-...' ORDER BY CreatedAt DESC;
-- Result: 3 rows (3 strategie)
```

---

## 📈 Timeline

```
T=0s     User clicks "Register"
T=0.1s   Frontend sends POST request
T=0.2s   Backend receives request
T=0.3s   AuthService creates user
T=0.4s   User saved to database
T=0.5s   SeederService called
T=0.6s   3 Strategies created
T=0.7s   Strategies saved to database
T=0.8s   Token generated
T=0.9s   Response sent to frontend
T=1.0s   Frontend receives response
T=1.1s   Token saved in localStorage
T=1.2s   User redirected to dashboard
T=1.3s   User navigates to Strategies page
T=1.4s   Frontend requests GET /api/strategies
T=1.5s   Backend queries Strategies table
T=1.6s   Backend returns 3 strategies
T=1.7s   Frontend renders 3 cards
T=1.8s   ✅ USER SEES 3 DEFAULT STRATEGIES!
```

---

## 🔄 Idempotency Check

```
Scenariusz: Użytkownik przypadkowo zaloguje się dwa razy

Rejestracja #1:
├─ Utwórz użytkownika: ID = abc-123
├─ Seeduj strategie ✅
└─ SUCCESS

Rejestracja #2 (ten sam email):
├─ Sprawdzenie: czy email istnieje?
│  └─ YES! abc-123 jest w bazie
├─ Wyrzuć: InvalidOperationException
│  "Użytkownik z tym emailem już istnieje"
└─ NO DUPLICATE REGISTRATION

Bezpieczeństwo: ✅ SAFE!
```

```
Scenariusz: Seeder uruchomił się dwa razy

Seeduj #1:
├─ SELECT COUNT(*) WHERE UserId = abc-123
│  └─ COUNT = 0
├─ Utwórz 3 strategie ✅
└─ SUCCESS

Seeduj #2 (retry):
├─ SELECT COUNT(*) WHERE UserId = abc-123
│  └─ COUNT = 3
├─ Pomiń seeding
└─ NO DUPLICATE STRATEGIES

Bezpieczeństwo: ✅ SAFE!
```

---

Ostatnia aktualizacja: 2026-05-04  
Status: ✅ Implementacja Kompletna
