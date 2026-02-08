# Biblioteka – system rezerwacji książek (Blazor / ASP.NET Core)

Projekt przedstawia prosty system biblioteczny: użytkownicy mogą przeglądać książki i je rezerwować, a pracownik (konto „Employee”) zarządza książkami, rezerwacjami oraz generuje raporty.

Celem projektu było **zrobienie działającej aplikacji webowej**, a jednocześnie **spełnienie konkretnych wymagań projektowych** (logowanie, baza danych, wzorce projektowe itd.).

---

## Technologie

- **C# / .NET**
- **ASP.NET Core + Blazor Server (InteractiveServer)**
- **Entity Framework Core + SQLite + migracje**
- **ASP.NET Identity (logowanie, role, cookies)**
- **QuestPDF** (generowanie raportu PDF)

---

## Role i najważniejsze funkcje

### Czytelnik (rola: `User`)
- przegląd książek
- filtrowanie listy książek
- rezerwacja książki (z potwierdzeniem)

### Pracownik (rola: `Employee`)
- dodawanie / edycja / usuwanie książek (z zabezpieczeniami)
- archiwizacja książek (nie są widoczne na liście dla nikogo)
- widok rezerwacji pracownika
- powiadomienia o nowych zdarzeniach (dzwonek)
- **panel raportów**: filtrowanie + generowanie raportu **PDF/CSV**

---

### CI/CD

Projekt wykorzystuje GitHub Actions do automatycznego:
- budowania obrazu Docker
- publikacji do GitHub Container Registry
- wdrażania aplikacji na serwer Oracle Cloud przy użyciu Docker Compose

## 1) Logowanie i rejestracja użytkowników ✅

Aplikacja posiada logowanie i rejestrację użytkownika.

- Formularze:  
  - `Components/Pages/Login.razor`  
  - `Components/Pages/Register.razor`
- Wylogowanie:  
  - `Components/Pages/Logout.razor` + endpoint `/auth/logout`
- Mechanizm logowania działa na cookies (Identity).

W `Program.cs` są skonfigurowane:
- Identity + Role
- Authentication/Authorization
- seed ról `User`, `Employee` i konto pracownika

---

## 2) Walidacja danych na serwerze ✅

Walidacja danych jest realizowana na modelach formularzy poprzez adnotacje (`[Required]`, `[MaxLength]`, `[Range]`).

Przykłady:
- Dodawanie/edycja książek i kategorii: `Components/Pages/Books.razor` (modele `NewBookModel`, `EditBookModel`, `CategoryEditModel`)
- Formularze działają w oparciu o `EditForm` + `DataAnnotationsValidator`.

Dodatkowo logika serwisów sprawdza warunki biznesowe (np. czy książka dostępna).

---

## 3) Entity Framework + migracje ✅

Projekt korzysta z EF Core oraz migracji.

- DbContext: `Biblioteka.Infrastructure/LibraryDbContext.cs`
- Konfiguracja encji: `OnModelCreating`
- Migracje: folder `Biblioteka.Infrastructure/Migrations`
- Baza SQLite: `App_Data/library.db` (lokalnie)

---

## 4) Wstrzykiwanie zależności (DI / serwisy) ✅

W projekcie używane jest DI do serwisów i logiki biznesowej.

Rejestracje w `Program.cs` m.in.:
- `IReservationService`
- `INotificationService`
- `IReservationMediator`
- `IReportFactory` + generatory raportów
- `ILibraryRules` (Singleton)

W komponentach wstrzykiwanie np.:
- `@inject INotificationService ...`
- `@inject IReservationMediator ...`

---

## 5) Ekrany na co najmniej 5 typach danych ✅

W aplikacji są ekrany pracujące na wielu typach danych (w bazie i w UI). Przykładowy zestaw:

1. **Book** – lista książek + CRUD (pracownik)  
   `Components/Pages/Books.razor`

2. **BookCategory** – zarządzanie kategoriami  
   `Components/Pages/Books.razor`

3. **Reservation** – rezerwacje użytkownika i pracownika  
   `Components/Pages/MyReservations.razor`  
   `Components/Pages/ReservationsAdmin.razor`

4. **Notification** – powiadomienia w layout (dzwonek)  
   `Components/Layout/MainLayout.razor`  
   + serwis `Infrastructure/Services/NotificationService.cs`

5. **Identity / AppUser (użytkownicy)** – logowanie/rejestracja/role  
   `Infrastructure/Auth/AppUser.cs` + Identity w `Program.cs`

To spełnia wymóg „min. 5 typów danych”.

---

## 6) Simple Factory ✅

W projekcie użyty jest wzorzec **Simple Factory** do tworzenia rezerwacji.

- `Biblioteka.Domain/ReservationFactory.cs`

Fabryka tworzy obiekt `Reservation` z poprawnie ustawionymi polami (status, daty itd.), dzięki czemu logika tworzenia nie jest rozproszona po wielu miejscach.

---

## 7) Interfejsy ✅

W projekcie używane są interfejsy do rozdzielenia logiki i łatwiejszego testowania/rozwoju.

Przykłady:
- `IReservationService`
- `INotificationService`
- `IReservationMediator`
- `ILibraryRules`
- Raporty: `IReportGenerator`, `IReportFactory`

---

## 8) Factory Method ✅

W projekcie jest wzorzec Factory Method w części odpowiedzialnej za **akcje po kliknięciu w powiadomienie**.

- w `Biblioteka.Domain/Notifications/`:
  - `INotificationAction` (interfejs)
  - `NotificationActionBase` (klasa bazowa)
  - konkretne akcje np. „przejdź do rezerwacji”
  - `NotificationActionFactory` – wybiera właściwą akcję na podstawie typu/targetu

Dzięki temu logika “co zrobić po kliknięciu” nie jest w `if/else`, tylko w jednym miejscu.

---

## 9) Abstract Factory ✅

Zastosowane w module raportów.

- Abstrakcje: `Biblioteka.Domain/Reports/Abstractions`
  - `IReportFactory`
  - `IReportGenerator`
- Implementacje: `Biblioteka.Infrastructure/Reports`
  - `PdfReportGenerator`
  - `CsvReportGenerator`
  - `ReportFactory` (tworzy generator na podstawie formatu)

W UI pracownik wybiera format (PDF/CSV), a fabryka zwraca właściwy generator.

---

## 10) Klasy bazowe i abstrakcyjne ✅

W projekcie występują klasy bazowe / abstrakcyjne, użyte sensownie:

- `BaseEntity` (wspólna baza encji domenowych)
- `NotificationActionBase` (abstrakcyjna baza dla działań powiadomień)

Dzięki temu wspólne elementy nie są dublowane w wielu klasach.

---

## 11) Interpreter ✅

W panelu raportów dla pracownika działa system filtrowania książek oparty o wzorzec Interpreter.

- `Biblioteka.Domain/Interpreter/`
  - `IBookFilterExpression`
  - `AllBooksExpression`
  - `TextSearchExpression`
  - `CategoryExpression`
  - `StatusExpression`
  - `AndExpression`

UI buduje „drzewko” filtrów na podstawie pól formularza, a interpreter zwraca przefiltrowaną listę.

Strona:
- `Components/Pages/BooksReportAdmin.razor`

---

## 12) Mediator ✅

Rezerwacje i powiadomienia są spięte przez wzorzec Mediator.

- `Biblioteka.Domain/IReservationMediator.cs`
- `Biblioteka.Infrastructure/Services/ReservationMediator.cs`

Mediator:
- wywołuje serwis rezerwacji (tworzy/zmienia status)
- dodaje powiadomienia (np. do pracowników przy nowej rezerwacji, do czytelnika przy „gotowe do odbioru”)

Dzięki temu UI nie musi znać wszystkich zależności.

---

## 13) Singleton ✅

Zasady działania biblioteki (dni robocze, godziny otwarcia, wyliczanie terminu odbioru) są zrobione jako Singleton.

- Interfejs: `Biblioteka.Domain/ILibraryRules.cs`
- Implementacja: `Biblioteka.Domain/LibraryRules.cs`
- Rejestracja DI: `Program.cs`
  - `AddSingleton<ILibraryRules, LibraryRules>()`

Z tego korzysta logika rezerwacji do wyliczania `ReservedUntil`:
- zamiast “72h”, termin jest liczony jako **do końca drugiego następnego dnia roboczego (18:00)**.

---

# Dodatkowe elementy funkcjonalne 

- Powiadomienia (dropdown w dzwonku) w `MainLayout.razor`
- Archiwizacja książek (alternatywa dla usuwania, gdy istnieje historia rezerwacji)
- Raporty PDF/CSV z datą wygenerowania

---

## Strony

- `/books` – książki (dla wszystkich, CRUD dla pracownika)
- `/my-reservations` – rezerwacje czytelnika
- `/admin/reservations` – rezerwacje pracownika
- `/admin/books-report` – raporty + filtrowanie + generowanie PDF/CSV (pracownik)
- `/login`, `/register`

---

## Autorzy

Jacek Jochemczyk / Michał Gandyk

Projekt wykonany jako aplikacja demonstracyjna
