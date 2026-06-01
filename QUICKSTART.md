# 🚀 QUICKSTART - Strategie Inwestycyjne

## ⚡ 5 Minut do Uruchomienia

### Krok 1: Backend (2 min)

```bash
# 1. Przejdź do folderu
cd WinWigApp.Server

# 2. Kompiluj
dotnet build

# 3. Uruchom
dotnet run
```

**Oczekiwany output:**
```
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5000
```

---

### Krok 2: Frontend (2 min)

```bash
# 1. Nowe okno Terminal
cd winwigapp.client

# 2. Zainstaluj (jeśli potrzeba)
npm install

# 3. Start dev server
npm start
```

**Oczekiwany output:**
```
Compiled successfully!
You can now view the application in the browser.
```

---

### Krok 3: Testowanie (1 min)

1. Otwórz http://localhost:3000
2. Zaloguj się
3. Przejdź do "Strategie inwestycyjne"
4. Kliknij "+ Nowa strategia"
5. Wypełnij formularz:
   ```
   Nazwa: Test Strategy
   Stopa zwrotu: 10
   Horyzont: 30
   RSI niski: 30
   RSI wysoki: 70
   ☑ MACD Kupno
   ☑ SMA 50 > 200
   ```
6. Kliknij "Utwórz strategię"

---

## 🧪 Szybkie Testowanie w Postmanie

### 1. Zaloguj się
```
POST http://localhost:5000/api/auth/login
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Skopiuj `token` z response'u

### 2. Utwórz strategię
```
POST http://localhost:5000/api/strategies
Authorization: Bearer {TOKEN}
Content-Type: application/json

{
  "name": "Test",
  "targetReturn": 10,
  "investmentHorizon": 30,
  "rsiLow": 30,
  "rsiHigh": 70,
  "macdBuy": true,
  "sma50Above200": false
}
```

### 3. Pobierz listę
```
GET http://localhost:5000/api/strategies
Authorization: Bearer {TOKEN}
```

### 4. Włącz/wyłącz
```
PUT http://localhost:5000/api/strategies/{id}/toggle
Authorization: Bearer {TOKEN}
```

---

## 📊 Pliki do Przeczytania

### Najpierw przeczytaj (5 min):
1. **README_STRATEGIES.md** - Overview
2. **IMPLEMENTATION_SUMMARY.md** - Co zostało zrobione

### Potem szczegóły (15 min):
3. **STRATEGIES_DOCUMENTATION.md** - Jak używać
4. **STRATEGIES_IMPLEMENTATION.md** - Jak to działa (dev)

### Do testowania (10 min):
5. **POSTMAN_TESTING.md** - Testy API
6. **FAQ_STRATEGIES.md** - Odpowiedzi na pytania

### Wizualizacje:
7. **WIREFRAME_UI.md** - Screenshoty UI

---

## ✅ Checklist Konfiguracji

```
Backend:
- [ ] dotnet build - OK
- [ ] dotnet run - OK (słuchaj na 5000)
- [ ] Program.cs ma DI - Sprawdzono ✓

Frontend:
- [ ] npm install - Done
- [ ] npm start - OK (localhost:3000)
- [ ] strategiesApi.ts - Imported ✓
- [ ] Strategies.tsx - Updated ✓

Database:
- [ ] Strategy model - Istnieje ✓
- [ ] DbContext - Configured ✓
- [ ] Migrations - Updated (jeśli trzeba)

Authentication:
- [ ] JWT Token - Works ✓
- [ ] User Isolation - Implemented ✓
```

---

## 🎯 Główne Feature'y

### CRUD Operacje
- ✅ Utwórz strategię
- ✅ Pobierz listę
- ✅ Wyświetl szczegóły
- ✅ Edytuj parametry
- ✅ Usuń strategię

### Zarządzanie
- ✅ Włącz/wyłącz strategię
- ✅ Status aktywności
- ✅ Timestamp utworzenia
- ✅ User isolation

### Walidacja
- ✅ Nazwa wymagana
- ✅ RSI niski < wysoki
- ✅ Wartości > 0
- ✅ RSI w range 0-100

### Security
- ✅ JWT Authorization
- ✅ User isolation
- ✅ Permission checks
- ✅ Error handling

---

## 📱 UI Przewodnik

### Strona Strategii
```
[+ Nowa strategia] button
│
├─ Lista kart (grid 3-4 na desktop)
│  ├─ Nazwa strategii
│  ├─ Status (Aktywna/Nieaktywna)
│  ├─ Parametry (CEL, HORYZONT, RSI)
│  ├─ Warunki (MACD, SMA)
│  ├─ Akcje (Uruchom/Zatrzymaj, Edytuj, Usuń)
│  └─ Data utworzenia
│
└─ Pusty stan (gdy brak strategii)
```

### Modal Formularz
```
Pola:
├─ Nazwa (text input)
├─ Stopa zwrotu (number input)
├─ Horyzont (number input)
├─ RSI niski (number 0-100)
├─ RSI wysoki (number 0-100)
├─ MACD Kupno (checkbox)
├─ SMA 50>200 (checkbox)
│
Buttons:
├─ Anuluj
└─ Utwórz/Zapisz
```

---

## 🔍 Troubleshooting 2 Min

### Problem: "Cannot find module..."
```bash
# Rozwiązanie:
npm install
```

### Problem: "Connection refused: localhost:5000"
```bash
# Rozwiązanie:
# Sprawdź czy backend działa
# Zaloguj się do VS i restartuj debugger
```

### Problem: "401 Unauthorized"
```bash
# Rozwiązanie:
# Zaloguj się ponownie
# Token wygasł - uzyskaj nowy
```

### Problem: "Strategy not found"
```bash
# Rozwiązanie:
# Sprawdź czy to Twoja strategia
# Spróbuj odświeżyć stronę (F5)
```

### Problem: "RSI niski musi być mniejszy..."
```bash
# Rozwiązanie:
# Sprawdź wartości:
# ✓ RSI niski = 30, RSI wysoki = 70
# ✗ RSI niski = 70, RSI wysoki = 30
```

---

## 🎓 Przykład Użycia - Krok Po Kroku

### 1. Zaloguj się
```
Email: user@example.com
Password: Password123!
Kliknij: Login
```

### 2. Przejdź do Strategii
```
Menu → "Strategie inwestycyjne"
```

### 3. Utwórz Nową
```
Kliknij: [+ Nowa strategia]
```

### 4. Wypełnij Formularz
```
Nazwa: Moja Konserwatywna Strategia
Stopa zwrotu: 8
Horyzont: 45
RSI niski: 35
RSI wysoki: 70
☑ MACD: Tak
☑ SMA50>200: Tak
```

### 5. Zapisz
```
Kliknij: [Utwórz strategię]
```

### 6. Aktywuj
```
Kliknij: [Urunchom]
```

### 7. Monitoruj
```
Strategia jest teraz AKTYWNA
Czeka na sygnały rynkowe
```

---

## 📊 Porównanie Strategii

| Parametr | Konserwatywna | Zbilansowana | Agresywna |
|----------|---------------|-------------|-----------|
| RSI niski | 35 | 30 | 20 |
| RSI wysoki | 70 | 70 | 75 |
| MACD | ✓ | ✓ | ✗ |
| SMA50>200 | ✓ | ✓ | ✗ |
| Horyzont | 45 dni | 30 dni | 20 dni |
| Stopa zwrotu | 8% | 12% | 15% |
| **Ryzyko** | Niskie | Średnie | Wysokie |

---

## 🎯 Następne Kroki

### Tuż Po Uruchomieniu:
1. ✅ Zaloguj się
2. ✅ Utwórz test strategię
3. ✅ Przetestuj CRUD operacje
4. ✅ Włącz/wyłącz strategię

### W Kilka Minut:
5. ⏳ Przeczytaj STRATEGIES_DOCUMENTATION.md
6. ⏳ Testuj w Postmanie (opcjonalnie)
7. ⏳ Spróbuj różnych parametrów

### Następnie:
8. 🚀 Deploy na produkcję
9. 🚀 Monitoruj performance
10. 🚀 Zbieraj user feedback

---

## 📞 Potrzebujesz Pomocy?

### Dokumentacja
- 📖 README_STRATEGIES.md
- 📖 STRATEGIES_DOCUMENTATION.md
- 📖 STRATEGIES_IMPLEMENTATION.md
- 📖 FAQ_STRATEGIES.md

### Support
- 💬 GitHub Issues
- 💬 Discord
- 📧 support@winwigapp.com

---

## ✨ Gotów Zacząć?

```bash
# Terminal 1 - Backend
cd WinWigApp.Server && dotnet run

# Terminal 2 - Frontend
cd winwigapp.client && npm start

# Terminal 3 - Postman (opcjonalnie)
# Otwórz Postmana i zaloguj się
```

**Wszystko powinno działać teraz! 🎉**

---

Ostatnia aktualizacja: 2026-05-04  
Czas do pełnej pracy: **~5 minut**  
Trudność: **⭐⭐ Łatwe**
