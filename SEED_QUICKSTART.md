# ⚡ Quick Guide - Domyślne Strategie

## 🎯 Co się Zmieniło?

Nowi użytkownicy **automatycznie** otrzymują **3 domyślne strategie** zaraz po rejestracji:

```
✅ Bezpieczna dla Początkujących (8%, 45 dni, RSI 35-70)
✅ Zbilansowana (12%, 30 dni, RSI 30-70)
✅ Agresywna - Łap Upadające Noże (15%, 20 dni, RSI 20-75)
```

---

## 🔧 Jak To Działa?

### 1. Rejestracja Użytkownika
```
User: "Zarejestruj się"
```

### 2. Backend Tworzy Konto
```
AuthService:
├─ Utwórz użytkownika
├─ Zapisz do bazy
└─ Wywołaj SeederService ← NOWE
```

### 3. SeederService Dodaje Strategie
```
SeederService:
├─ Sprawdź czy ma już strategie
├─ Utwórz 3 domyślne
└─ Dodaj do bazy
```

### 4. Zwróć Token
```
AuthResponse z tokenem JWT
```

### 5. Frontend - Zalogowanie
```
User: "Zaloguj się"
GET /api/strategies
└─ ✅ 3 strategie na liście!
```

---

## 📊 Parametry Strategii

| Strategia | Cel | Horyzont | RSI | MACD | SMA | Ryzyko |
|-----------|-----|----------|-----|------|-----|--------|
| Bezpieczna | 8% | 45 dni | 35-70 | ✓ | ✓ | Niskie |
| Zbilansowana | 12% | 30 dni | 30-70 | ✓ | ✓ | Średnie |
| Agresywna | 15% | 20 dni | 20-75 | ✗ | ✗ | Wysokie |

---

## ✅ Checklist Testowania

```bash
# Terminal 1 - Backend
cd WinWigApp.Server
dotnet build
dotnet run

# Terminal 2 - Postman/cURL
```

### Test 1: Rejestracja
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"firstName": "Test",
	"lastName": "User",
	"email": "test@example.com",
	"password": "Password123!"
  }'
```

**Response:** Token JWT

### Test 2: Pobierz Strategie
```bash
curl -X GET http://localhost:5000/api/strategies \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Response:** 3 strategie ✅

---

## 📁 Nowe/Zmienione Pliki

### Nowe:
- `Services/ISeederService.cs` - Seeder service

### Zmienione:
- `Services/AuthService.cs` - Dodany seeder call
- `Program.cs` - Dodane DI dla ISeederService

### Dokumentacja:
- `SEED_DATA_STRATEGIES.md` - Pełna dokumentacja

---

## 🎓 Workflow Nowego Użytkownika

```
1. Wchodzi do aplikacji
2. Kliknął "Rejestruj się"
3. Wypełnił formularz
   ├─ Imię: Jan
   ├─ Nazwisko: Kowalski
   ├─ Email: jan@example.com
   └─ Hasło: Password123!
4. Kliknął "Register"
5. ✅ Backend automatycznie dodał 3 strategie
6. Zwrócił token i zalogował
7. Weszło do aplikacji
8. Przeszło do "Strategie inwestycyjne"
9. ✅ WIDZI 3 DOMYŚLNE STRATEGIE! 🎉
10. Może kliknąć "Uruchom" na którejkolwiek
```

---

## 🛡️ Ważne Cechy

✅ **Idempotentny** - Nie tworzy duplikatów  
✅ **User-specific** - Każdy ma własne kopie  
✅ **Nieaktywny domyślnie** - Musi ręcznie włączyć  
✅ **Logowany** - Wszystkie operacje logowane  
✅ **Error handling** - Graceful degradation  

---

## 🚀 Następne Kroki

1. **Uruchom backend** - `dotnet run`
2. **Przetestuj rejestrację** - Utwórz nowe konto
3. **Sprawdź strategie** - GET /api/strategies
4. **Włącz strategię** - PUT .../toggle
5. **Raduj się!** - Strategie działają! 🎉

---

## 📞 Pytania?

- **Dokumentacja:** `SEED_DATA_STRATEGIES.md`
- **Kod:** `Services/ISeederService.cs`
- **Tests:** `POSTMAN_TESTING.md`

---

**Status:** ✅ Gotowe do Produkcji
