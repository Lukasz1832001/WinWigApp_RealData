# 🧪 Testowanie API Strategii - Poradnik Postman

## 📍 Konfiguracja Postman Environment

### Zmienne globalne
```json
{
  "baseUrl": "http://localhost:5000",
  "token": "YOUR_JWT_TOKEN_HERE",
  "userId": "YOUR_USER_ID_HERE",
  "strategyId": "CREATED_STRATEGY_ID"
}
```

---

## 🔐 Krok 1: Zaloguj się i uzyskaj token

### POST - Zaloguj się
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

**Response (Przykład):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Akcja w Postmanie:**
- Skopiuj token z response'u
- Wklej w `token` variable w environment
- Skopiuj userId i wklej w `userId` variable

---

## ✨ Krok 2: Utwórz strategię

### POST /api/strategies
```
POST {{baseUrl}}/api/strategies
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Moja testowa strategia",
  "targetReturn": 10.0,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false
}
```

**Response (201 Created):**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "name": "Moja testowa strategia",
  "targetReturn": 10.0,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false,
  "isActive": false,
  "createdAt": "2026-05-04T10:30:00Z"
}
```

**Akcja w Postmanie:**
- Skopiuj `id` z response'u
- Wklej w `strategyId` variable w environment
- Sprawdź że `isActive` jest `false` (nowe strategie domyślnie wyłączone)

---

## 📋 Krok 3: Pobierz wszystkie strategie

### GET /api/strategies
```
GET {{baseUrl}}/api/strategies
Authorization: Bearer {{token}}
```

**Response (200 OK):**
```json
[
  {
	"id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
	"name": "Moja testowa strategia",
	"targetReturn": 10.0,
	"investmentHorizon": 30,
	"rsiLow": 30,
	"rsiHigh": 70,
	"macdBuy": true,
	"sma50Above200": false,
	"isActive": false,
	"createdAt": "2026-05-04T10:30:00Z"
  }
]
```

**Sprawdzenie:**
- ✅ Lista zawiera właśnie utworzoną strategię
- ✅ Strategia ma prawidłowe wartości

---

## 🔍 Krok 4: Pobierz jedną strategię

### GET /api/strategies/{id}
```
GET {{baseUrl}}/api/strategies/{{strategyId}}
Authorization: Bearer {{token}}
```

**Response (200 OK):**
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "name": "Moja testowa strategia",
  "targetReturn": 10.0,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false,
  "isActive": false,
  "createdAt": "2026-05-04T10:30:00Z"
}
```

---

## ✏️ Krok 5: Zaktualizuj strategię

### PUT /api/strategies/{id}
```
PUT {{baseUrl}}/api/strategies/{{strategyId}}
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Moja testowa strategia - ZAKTUALIZOWANA",
  "targetReturn": 12.0,
  "investmentHorizon": 45,
  "rsiLow": 25,
  "rsiHigh": 75,
  "macdBuy": true,
  "sma50Above200": true
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Strategia zaktualizowana"
}
```

**Sprawdzenie:**
- ✅ `success` = true
- ✅ `message` zawiera potwierdzenie

**Weryfikacja:**
- Wykonaj GET /api/strategies/{{strategyId}} aby potwierdzić zmiany

---

## 🔄 Krok 6: Włącz strategię (Toggle)

### PUT /api/strategies/{id}/toggle
```
PUT {{baseUrl}}/api/strategies/{{strategyId}}/toggle
Authorization: Bearer {{token}}
```

**Response (200 OK):**
```json
{
  "success": true,
  "isActive": true,
  "message": "Strategia aktywowana"
}
```

**Sprawdzenie:**
- ✅ `success` = true
- ✅ `isActive` = true (zmienił się z false na true)

**Drugi raz (wyłączenie):**
- Uruchom ten sam endpoint jeszcze raz
- Powinna zwrócić `isActive` = false

```json
{
  "success": true,
  "isActive": false,
  "message": "Strategia dezaktywowana"
}
```

---

## 🗑️ Krok 7: Usuń strategię

### DELETE /api/strategies/{id}
```
DELETE {{baseUrl}}/api/strategies/{{strategyId}}
Authorization: Bearer {{token}}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Strategia usunięta"
}
```

**Sprawdzenie:**
- ✅ `success` = true
- ✅ `message` zawiera potwierdzenie

**Weryfikacja:**
- Wykonaj GET /api/strategies/{{strategyId}}
- Powinna zwrócić 404 Not Found

```json
{
  "message": "Strategia nie znaleziona"
}
```

---

## ⚠️ Testy Walidacji

### Test 1: Brak nazwy strategii

```
POST {{baseUrl}}/api/strategies
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "",
  "targetReturn": 10.0,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false
}
```

**Expected Response (400 Bad Request):**
```json
{
  "message": "Nazwa strategii jest wymagana"
}
```

---

### Test 2: RSI niski >= RSI wysoki

```
POST {{baseUrl}}/api/strategies
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Błędna strategia",
  "targetReturn": 10.0,
  "investmentHorizon": 30,
  "rsiLow": 70,
  "rsiHigh": 30,
  "macdBuy": true,
  "sma50Above200": false
}
```

**Expected Response (400 Bad Request):**
```json
{
  "message": "RSI niski musi być mniejszy niż RSI wysoki"
}
```

---

### Test 3: Ujemna stopa zwrotu

```
POST {{baseUrl}}/api/strategies
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Błędna strategia",
  "targetReturn": -10.0,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false
}
```

**Expected Response (400 Bad Request):**
```json
{
  "message": "Planowana stopa zwrotu musi być większa od zera"
}
```

---

### Test 4: Brak autoryzacji

```
POST {{baseUrl}}/api/strategies
Content-Type: application/json

{
  "name": "Strategia bez tokena",
  ...
}
```

**Expected Response (401 Unauthorized):**
```json
{}
```

---

### Test 5: Nieprawidłowy token

```
POST {{baseUrl}}/api/strategies
Authorization: Bearer invalid.token.here
Content-Type: application/json

{
  "name": "Strategia ze złym tokenem",
  ...
}
```

**Expected Response (401 Unauthorized):**

---

### Test 6: Strategia innego użytkownika

```
GET {{baseUrl}}/api/strategies/f47ac10b-58cc-4372-a567-0e02b2c3d479
Authorization: Bearer {{token_inny_uzytkownik}}
```

**Expected Response (404 Not Found):**
```json
{
  "message": "Strategia nie znaleziona"
}
```

---

## 📊 Szybka Kollekcja Testów - Kopia do Postmana

```json
{
  "info": {
	"name": "WinWigApp - Strategies API",
	"description": "Kolekcja testów dla API Strategii",
	"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "variable": [
	{
	  "key": "baseUrl",
	  "value": "http://localhost:5000"
	},
	{
	  "key": "token",
	  "value": ""
	},
	{
	  "key": "strategyId",
	  "value": ""
	}
  ],
  "item": [
	{
	  "name": "1. Zaloguj się",
	  "request": {
		"method": "POST",
		"header": [
		  {
			"key": "Content-Type",
			"value": "application/json"
		  }
		],
		"body": {
		  "mode": "raw",
		  "raw": "{\n  \"email\": \"user@example.com\",\n  \"password\": \"Password123!\"\n}"
		},
		"url": {
		  "raw": "{{baseUrl}}/api/auth/login",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "auth", "login"]
		}
	  }
	},
	{
	  "name": "2. Utwórz strategię",
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
		  "raw": "{\n  \"name\": \"Test Strategy\",\n  \"targetReturn\": 10.0,\n  \"investmentHorizon\": 30,\n  \"rsiLow\": 30,\n  \"rsiHigh\": 70,\n  \"macdBuy\": true,\n  \"sma50Above200\": false\n}"
		},
		"url": {
		  "raw": "{{baseUrl}}/api/strategies",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies"]
		}
	  }
	},
	{
	  "name": "3. Pobierz wszystkie strategie",
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
	  "name": "4. Pobierz jedną strategię",
	  "request": {
		"method": "GET",
		"header": [
		  {
			"key": "Authorization",
			"value": "Bearer {{token}}"
		  }
		],
		"url": {
		  "raw": "{{baseUrl}}/api/strategies/{{strategyId}}",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies", "{{strategyId}}"]
		}
	  }
	},
	{
	  "name": "5. Zaktualizuj strategię",
	  "request": {
		"method": "PUT",
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
		  "raw": "{\n  \"name\": \"Updated Strategy\",\n  \"targetReturn\": 15.0,\n  \"investmentHorizon\": 45,\n  \"rsiLow\": 25,\n  \"rsiHigh\": 75,\n  \"macdBuy\": true,\n  \"sma50Above200\": true\n}"
		},
		"url": {
		  "raw": "{{baseUrl}}/api/strategies/{{strategyId}}",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies", "{{strategyId}}"]
		}
	  }
	},
	{
	  "name": "6. Włącz/Wyłącz strategię",
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
	},
	{
	  "name": "7. Usuń strategię",
	  "request": {
		"method": "DELETE",
		"header": [
		  {
			"key": "Authorization",
			"value": "Bearer {{token}}"
		  }
		],
		"url": {
		  "raw": "{{baseUrl}}/api/strategies/{{strategyId}}",
		  "host": ["{{baseUrl}}"],
		  "path": ["api", "strategies", "{{strategyId}}"]
		}
	  }
	}
  ]
}
```

---

## 🎯 Checklist Testów

- [ ] Login zwraca token
- [ ] POST /strategies tworzy strategię z `isActive: false`
- [ ] GET /strategies pobiera listę
- [ ] GET /strategies/{id} pobiera pojedynczą strategię
- [ ] PUT /strategies/{id} aktualizuje strategię
- [ ] PUT /strategies/{id}/toggle zmienia stan aktywności
- [ ] DELETE /strategies/{id} usuwa strategię
- [ ] DELETE na usuniętą strategię zwraca 404
- [ ] Walidacja: Pusta nazwa zwraca błąd
- [ ] Walidacja: RSI niski >= wysoki zwraca błąd
- [ ] Walidacja: Ujemna stopa zwrotu zwraca błąd
- [ ] Brak tokena zwraca 401
- [ ] Złą token zwraca 401
- [ ] Strategia innego użytkownika jest niedostępna (404)

---

Ostatnia aktualizacja: 2026-05-04
Wersja: 1.0
