# ❓ FAQ - Strategie Inwestycyjne

## 📋 Pytania Ogólne

### P: Jak długo nowa strategia zajmuje aktywację?
**O:** Natychmiast! Kliknij "Uruchom" i strategia zmienia status na aktywną w tym momencie.

### P: Czy mogę mieć wiele aktywnych strategii jednocześnie?
**O:** Tak! Możesz mieć tyle aktywnych strategii ile chcesz. Każda będzie niezależnie monitorować rynek.

### P: Jakie jest minimum/maksimum wartości RSI?
**O:** RSI to oscylator od 0-100. W praktyce:
- RSI < 30 = wyprzedane (sygnał kupna)
- RSI > 70 = przekupione (sygnał sprzedaży)

### P: Czy mogę zmienić już aktywną strategię?
**O:** Nie można edytować aktywnej strategii. Musisz najpierw ją wyłączyć (Zatrzymaj), a potem edytować.

---

## 🔧 Pytania Techniczne

### P: Co to dokładnie robi opcja "MACD Kupno"?
**O:** 
- Gdy **zaznaczona**: Kupujesz TYLKO gdy MACD przecina linię sygnału od dołu (silny sygnał)
- Gdy **nie zaznaczona**: Kupujesz tylko na podstawie RSI, ignorujesz MACD
- **Zalecenie**: Zaznacz dla bardziej pewnych sygnałów

### P: Co znaczy "SMA 50 powyżej SMA 200"?
**O:**
- **SMA 50 > SMA 200** = trend wzrostowy (BYCZE, pozytywne)
- **SMA 50 < SMA 200** = trend spadkowy (NIEDŹWIEDZIE, negatywne)
- Gdy opcja zaznaczona: kupujesz TYLKO w trendzie wzrostowym
- Gdy nie zaznaczona: kupujesz niezależnie od trendu

### P: Jaka jest różnica między "Horyzont inwestycyjny" a "Planowaną stopą zwrotu"?
**O:**
- **Horyzont**: Jak długo chcesz trzymać akcję (np. 30 dni)
- **Stopa zwrotu**: Cel zysku % (np. +10%)
- Oba warunki są monitorowane - sprzedajesz gdy osiągniesz cel OR gdy upłynie czas

### P: Czy można ustawić RSI niski = 0 lub wysoki = 100?
**O:** Technicznie tak, ale to nie ma sensu:
- RSI nie osiąga prawie nigdy wartości < 5 lub > 95
- Strategia nigdy nie otrzyma sygnału
- Zalecenie: Pozostań w range 20-80

---

## 📊 Pytania o Wskaźniki

### P: Ile danych historycznych potrzebne do obliczenia RSI?
**O:** 
- RSI obliczany jest z ostatnich ~14 dni (standardowo)
- Dla dokładnych wartości potrzeba ~20-30 dni danych
- System automatycznie pobiera wystarczającą historię

### P: Co jeśli RSI zawsze pozostaje między 30 a 70?
**O:** To oznacza, że akcja jest w trendie bocznym (ani zbyt tanio ani zbyt drogo).
- Strategia RSI działa najlepiej w trendach
- W trendzie bocznym będą częste fałszywe sygnały
- **Rozwiązanie**: Włącz opcję "SMA 50 > SMA 200" aby filtrować trendy

### P: MACD zawsze daje sygnały kupna/sprzedaży?
**O:** Nie zawsze. MACD generuje sygnały gdy się zmienia:
- Gdy MACD przecina linię sygnału = sygnał
- Między crossoverami = brak sygnału
- W flat market = rzadkie sygnały

---

## 💰 Pytania o Ryzyko

### P: Jakie parametry strategii są najbezpieczniejsze?
**O:** Konserwatywna strategia:
```
RSI niski: 35
RSI wysoki: 70
MACD Kupno: ✓
SMA 50 > 200: ✓
Horyzont: 45 dni
Stopa zwrotu: 8%
```

### P: Czy mogę stracić więcej niż zainwestowałem?
**O:** To zależy od struktury Twojego broker-account:
- Zwykłe konto: Max strata = inwestycja
- Margin account: Możesz stracić więcej
- **Zawsze ustal stop-loss!**

### P: Jaka stopa zwrotu jest realistyczna?
**O:** Zależy od wielu czynników:
- Warunki rynkowe
- Volatility akcji
- Liczba sygnałów
- Sugestia: 5-15% per quarter to bardzo dobry wynik

### P: Co jeśli strategia nie znajdzie żadnych akcji do kupna?
**O:** To normalne. Oznacza:
- Brak akcji spełniających kryteria
- Rynek nie ma warunków dla Twojej strategii
- **To jest dobre** - chroni Cię przed złymi inwestycjami

---

## 🐛 Troubleshooting

### P: Strategia się nie uruchamia
**O:** Sprawdź:
- [ ] Czy masz internet?
- [ ] Czy backend serwer jest uruchomiony?
- [ ] Czy zalogowałeś się?
- [ ] Czy token nie wygasł? (zaloguj się ponownie)

### P: Otrzymuję błąd "RSI niski musi być mniejszy niż RSI wysoki"
**O:** Sprawdzisz wartości:
```
❌ BŁĘD: RSI niski = 70, RSI wysoki = 30
✅ POPRAW: RSI niski = 30, RSI wysoki = 70
```

### P: Usunąłem strategię przez accident, mogę ją odzyskać?
**O:** Nie. Usuwanie jest permanentne. 
- **Porada**: Przed usunięciem zrób screenshot parametrów

### P: Strategia daje tylko sygnały sprzedaży, nigdy kupna
**O:** Powody:
- RSI nigdy nie spada poniżej ustawionego progu
- Trend rynku nie pasuje do strategii
- MACD nigdy nie przecina linii (jeśli włączone)
- **Rozwiązanie**: Zmień parametry na mniej restrykcyjne

### P: Wolne wczytywanie strategii
**O:** To normalne gdy:
- Duża liczba strategii
- Niski internet
- Server jest obciążony
- Zazwyczaj mniej niż 2 sekundy

---

## 🎓 Strategie dla Początkujących

### P: Jaka jest najlepsza pierwsza strategia?
**O:** Zacznij od tego:
```
Nazwa: "Moja pierwsza strategia"
Stopa zwrotu: 10%
Horyzont: 30 dni
RSI niski: 35
RSI wysoki: 70
MACD: ✓
SMA 50 > 200: ✓
```

### P: Jak testować strategię bez ryzyka?
**O:**
1. Zapamiętaj parametry strategii
2. Spróbuj znaleźć akcje manualnie na rynku
3. Sprawdzaj czy strategy dała by sygnał
4. Po kilku testach spróbuj na małej kwocie

### P: Czy mogę zduplikować istniejącą strategię?
**O:** Nie ma funkcji duplikacji, ale możesz:
1. Zapamiętaj parametry
2. Utwórz nową strategię
3. Wpisz te same parametry
4. Zmień tylko nazwę

---

## 📱 Pytania o UI/UX

### P: Jak zmienić kolejność wyświetlania strategii?
**O:** Strategie zawsze sortowane są od najnowszych do najstarszych.

### P: Czy mogę sortować strategie inaczej?
**O:** Teraz nie, ale to jest zaplanowana feature. Zapamiętaj gdzie są Twoje strategie.

### P: Jaka jest maksymalna długość nazwy strategii?
**O:** Technicznie do 255 znaków, ale dla czytelności limit do ~50 znaków jest zalecany.

### P: Czy mogę eksportować strategie?
**O:** Teraz nie, ale możesz:
1. Screenshot każdej strategii
2. Ręcznie zapisać parametry
3. Zapamiętać w notesie

---

## 🔐 Pytania o Bezpieczeństwo

### P: Czy moje strategie są prywatne?
**O:** Tak! Każdy użytkownik widzi tylko swoje strategie. Backend to pilnuje.

### P: Czy mogę dzielić się strategiami z innymi?
**O:** Teraz nie, ale możesz:
1. Opowiedzieć komuś o parametrach
2. Podzielić się nameą strategii
3. Pozwolić innym ją stworzyć

### P: Czy moje dane strategii są szyfrowane?
**O:** Tak - komunikacja z API jest przez HTTPS/SSL.

### P: Kto ma dostęp do moich strategii?
**O:** Tylko Ty (poprzez JWT token). Admin serwera może zobaczyć technicznie, ale nie powinien.

---

## 🔄 API/Developer Pytania

### P: Czy mogę integrować API strategii z moją aplikacją?
**O:** Tak! API jest dostępne na:
```
POST /api/strategies - Tworzenie
GET /api/strategies - Lista
GET /api/strategies/{id} - Pobieranie
PUT /api/strategies/{id} - Edycja
DELETE /api/strategies/{id} - Usuwanie
PUT /api/strategies/{id}/toggle - Toggle
```

### P: Jaki format danych API?
**O:** JSON z JWT auth w header:
```
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

### P: Jakie są limity API?
**O:** Brak oficjalnych limitów, ale:
- Szanuj serwer (nie spam'uj)
- Max 1000 strategii per user powinno być ok
- Czasy response < 1 sec zwykle

---

## 📚 Gdzie Szukać Pomocy?

### Dokumentacja
1. **Dla użytkowników**: `STRATEGIES_DOCUMENTATION.md`
2. **Dla developerów**: `STRATEGIES_IMPLEMENTATION.md`
3. **Do testowania**: `POSTMAN_TESTING.md`

### Support
- GitHub Issues: Zgłaszaj bugi
- Discord/Slack: Czat z zespołem
- Email: support@winwigapp.com

---

Ostatnia aktualizacja: 2026-05-04  
Wersja: 1.0
