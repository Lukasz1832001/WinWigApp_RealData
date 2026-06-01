# 🎨 Wizualizacja Interfejsu - Strategie Inwestycyjne

## 📸 Screenshoty UI

### 1. Strona główna Strategii (lista)

```
┌─────────────────────────────────────────────────┐
│                                                 │
│  Strategie inwestycyjne                    [+ Nowa strategia]
│  Twórz i testuj własne strategie              │
│                                                 │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌─────────────────────────┐                   │
│  │ Moja Strategia RSI      │ [▶ Aktywna]       │
│  │                         │                   │
│  │ 🎯 Cel: +10%           │                   │
│  │ 📅 Horyzont: 30 dni    │                   │
│  │ 📊 RSI: 30 - 70        │                   │
│  │                         │                   │
│  │ MACD kupno: Tak        │                   │
│  │ SMA 50 > 200: Nie      │                   │
│  │                         │                   │
│  │ [Zatrzymaj] [✎] [🗑]  │                   │
│  │                         │                   │
│  │ Utworzono: 04.05.2026  │                   │
│  └─────────────────────────┘                   │
│                                                 │
│  ┌─────────────────────────┐                   │
│  │ Test Strategy           │ [⏸ Nieaktywna]   │
│  │                         │                   │
│  │ 🎯 Cel: +15%           │                   │
│  │ 📅 Horyzont: 45 dni    │                   │
│  │ 📊 RSI: 25 - 75        │                   │
│  │                         │                   │
│  │ MACD kupno: Tak        │                   │
│  │ SMA 50 > 200: Tak      │                   │
│  │                         │                   │
│  │ [Uruchom] [✎] [🗑]     │                   │
│  │                         │                   │
│  │ Utworzono: 03.05.2026  │                   │
│  └─────────────────────────┘                   │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

## 🔧 Modal - Utwórz/Edytuj Strategię

```
╔═════════════════════════════════════════════════╗
║ Edytuj strategię                                ║
╠═════════════════════════════════════════════════╣
║                                                 ║
║ Nazwa strategii                                 ║
║ ┌─────────────────────────────────────────────┐ ║
║ │ Moja strategia RSI                          │ ║
║ └─────────────────────────────────────────────┘ ║
║                                                 ║
║ Planowana stopa zwrotu (%)    Horyzont (dni)  ║
║ ┌────────────────┐           ┌─────────────┐  ║
║ │ 10            │           │ 30          │  ║
║ └────────────────┘           └─────────────┘  ║
║                                                 ║
║ ┌─────────────────────────────────────────────┐ ║
║ │ Warunki strategii                           │ ║
║ │                                             │ ║
║ │ RSI niski (kupno)     RSI wysoki (sprzedaż)│ ║
║ │ ┌──────────────┐     ┌──────────────┐     │ ║
║ │ │ 30           │     │ 70           │     │ ║
║ │ └──────────────┘     └──────────────┘     │ ║
║ │                                             │ ║
║ │ ☑ Kupuj gdy MACD przecina linię sygnału    │ ║
║ │                                             │ ║
║ │ ☑ Wymagaj SMA 50 powyżej SMA 200           │ ║
║ │                                             │ ║
║ └─────────────────────────────────────────────┘ ║
║                                                 ║
║ [Anuluj]              [Zapisz zmiany]          ║
║                                                 ║
╚═════════════════════════════════════════════════╝
```

---

## 📋 Pusty Stan

```
┌─────────────────────────────────────────────────┐
│                                                 │
│  Strategie inwestycyjne                    [+ Nowa strategia]
│  Twórz i testuj własne strategie              │
│                                                 │
├─────────────────────────────────────────────────┤
│                                                 │
│                   📈                            │
│                                                 │
│       Nie masz jeszcze żadnych strategii       │
│                                                 │
│         [Utwórz pierwszą strategię]            │
│                                                 │
│                                                 │
└─────────────────────────────────────────────────┘
```

---

## 🔄 Data Flow

```
┌──────────────┐
│ User Action  │
│ (Create)     │
└──────┬───────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Validate Frontend            │
│ - Nazwa nie pusta           │
│ - RSI niski < wysoki        │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ POST /api/strategies         │
│ + JWT Token                 │
│ + CreateStrategyRequest     │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Backend: Validate DTO        │
│ - ModelState check          │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Backend: Validate Logic      │
│ - Check all parameters      │
│ - Verify user permissions   │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Create Strategy Entity       │
│ - Generate ID               │
│ - Set timestamp             │
│ - IsActive = false          │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Save to Database             │
│ SQLite + EF Core            │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Return StrategyResponse      │
│ (201 Created)               │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Update State (setStrategies)│
│ Add to list                 │
└──────┬──────────────────────┘
	   │
	   ▼
┌─────────────────────────────┐
│ Show Toast Success           │
│ "Utworzono strategię"       │
└──────┬──────────────────────┘
	   │
	   ▼
┌──────────────┐
│ UI Re-render │
│ New item     │
└──────────────┘
```

---

## 📊 Widok Szczegółów Karty

```
╔═══════════════════════════════════════╗
║      Moja strategia RSI               ║
║                        [▶] Aktywna    ║
╠═══════════════════════════════════════╣
║                                       ║
║  🎯 Cel:         +10%                ║
║  📅 Horyzont:    30 dni              ║
║  📊 RSI:         30 - 70             ║
║                                       ║
║  ┌────────────────────────────────┐  ║
║  │ MACD kupno:        ✓ Tak       │  ║
║  │ SMA 50 > SMA 200:  ✗ Nie       │  ║
║  └────────────────────────────────┘  ║
║                                       ║
║  [Zatrzymaj] [Edytuj] [Usuń]         ║
║                                       ║
║  Utworzono: 04.05.2026               ║
║                                       ║
╚═══════════════════════════════════════╝
```

---

## 🔄 Status Przejścia

```
┌────────────────┐
│  Nieaktywna    │
│  (Zatrzymana)  │
└────────┬───────┘
		 │ Kliknij "Uruchom"
		 ▼
┌────────────────┐
│  Aktywna       │ ← Monitoruje rynek
│  (Działająca)  │
└────────┬───────┘
		 │ Kliknij "Zatrzymaj"
		 ▼
┌────────────────┐
│  Nieaktywna    │
│  (Zatrzymana)  │
└────────────────┘

┌────────────────┐
│  Każda karta   │ ← Można edytować 
│  (Dowolna)     │   gdy nieaktywna
└────────────────┘
```

---

## 🎨 Legendy Kolorów

```
┌─────────────────────────────────┐
│  Elementy UI                    │
├─────────────────────────────────┤
│  🟢 Zielony    - Aktywny/Sukces │
│  🔘 Szary      - Wyłączony      │
│  🔴 Czerwony   - Error/Delete   │
│  🟦 Niebieski  - Info/Loading   │
│  ⚫ Czarny     - Background     │
│  ⚪ Biały      - Tekst         │
└─────────────────────────────────┘
```

---

## ⌨️ Skróty Klawiszowe

```
┌────────────────────────────┐
│ Akcje                      │
├────────────────────────────┤
│ Enter      - Zapisz        │
│ Escape     - Anuluj        │
│ Tab        - Następne pole │
│ Shift+Tab  - Poprzednie    │
└────────────────────────────┘
```

---

## 📱 Responsive Design

### Desktop (1920px+)
```
[Strategia 1] [Strategia 2] [Strategia 3]
[Strategia 4] [Strategia 5] [Strategia 6]
```

### Tablet (768px-1024px)
```
[Strategia 1] [Strategia 2]
[Strategia 3] [Strategia 4]
[Strategia 5] [Strategia 6]
```

### Mobile (< 768px)
```
[Strategia 1]
[Strategia 2]
[Strategia 3]
```

---

## 🔔 Notyfikacje

### ✅ Sukces
```
✓ Utworzono strategię
✓ Zaktualizowano strategię
✓ Usunięto strategię
✓ Strategia aktywowana
✓ Strategia dezaktywowana
```

### ❌ Błąd
```
✗ Podaj nazwę strategii
✗ RSI niski musi być mniejszy niż RSI wysoki
✗ Nie udało się załadować strategii
✗ Błąd podczas zapisywania
```

### ⓘ Info
```
ⓘ Zaloguj się aby zobaczyć strategie
ⓘ Ładowanie...
```

---

## 🎯 User Journey

```
1. Zaloguj się
   │
2. Przejdź do Strategie
   │
3. Wybierz:
   ├─ Utwórz nową [+ Nowa strategia]
   ├─ Edytuj istniejącą [✎]
   ├─ Włącz/wyłącz [▶/⏸]
   └─ Usuń [🗑]
   │
4. Wypełnij formularz
   │
5. Zapisz zmiany
   │
6. Zobaczy strategię na liście
   │
7. Strategia jest gotowa do użycia
```

---

## 📊 Status Lądowania

```
Przed implementacją:
┌─────────────────┐
│  Puste          │
│  Brak API       │
│  localStorage   │
│  Nie aktualnie  │
└─────────────────┘

Po implementacji:
┌─────────────────┐
│  ✓ Pełne CRUD   │
│  ✓ API Backend  │
│  ✓ Database     │
│  ✓ JWT Auth     │
│  ✓ Validacja    │
│  ✓ Error hand.  │
│  ✓ Logging      │
│  ✓ Dokumentacja │
└─────────────────┘
```

---

## 🎬 Animacje

### Efekty Przejścia:
- Fade In: Nowe karty strategii
- Slide Down: Modal otwierający się
- Spin: Loading indicator
- Bounce: Toast notifications
- Scale: Button hover

---

## 🎓 Przykład Wizualizacji - "Konserwatywna"

```
┌─────────────────────────────┐
│ Konserwatywna               │
│                        [⏸]  │
├─────────────────────────────┤
│                             │
│ Cel:    8% (niski)         │
│ Hor:    45 dni (długi)     │
│ RSI:    35-70 (szeroki)    │
│                             │
│ MACD:   ✓ Wymagany         │
│ SMA:    ✓ Trend wzrost.    │
│                             │
│ ✓ Mało sygnałów           │
│ ✓ Wysokie hit rate        │
│ ✓ Niskie ryzyko           │
│ ✓ Dla początkujących      │
│                             │
└─────────────────────────────┘
```

---

## 🎓 Przykład Wizualizacji - "Agresywna"

```
┌─────────────────────────────┐
│ Agresywna                   │
│                        [▶]  │
├─────────────────────────────┤
│                             │
│ Cel:    15% (wysoki)       │
│ Hor:    20 dni (krótki)    │
│ RSI:    20-75 (szeroki)    │
│                             │
│ MACD:   ✗ Brak wymogu      │
│ SMA:    ✗ Niezależnie      │
│                             │
│ ✓ Wiele sygnałów          │
│ ⚠ Niższe hit rate         │
│ ⚠ Wyższe ryzyko           │
│ ⚠ Dla doświadczonych      │
│                             │
└─────────────────────────────┘
```

---

Ostatnia aktualizacja: 2026-05-04  
Wersja: 1.0
