# UserPanelMvcAuth

Aplikacja ASP.NET Core MVC przygotowana na ćwiczenie 10 z APBD.

## Jak uruchomić aplikację

1. Sklonuj repozytorium:

```bash
git clone https://github.com/s29640/apbd-cw10-s29640.git
cd apbd-cw10-s20640/UserPanelMvcAuth
```

2. Sprawdź connection string w pliku:

```text
appsettings.json
```

Przykładowa konfiguracja dla MS SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UserPanelMvcAuthDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Wykonaj migracje:

```bash
dotnet ef database update
```

4. Uruchom aplikację:

```bash
dotnet run
```

5. Otwórz aplikację w przeglądarce pod adresem pokazanym w konsoli, np.:

```text
https://localhost:7177
```

## Jak utworzyć użytkownika testowego

Zwykłego użytkownika można utworzyć przez formularz rejestracji:

```text
/Account/Register
```

Podczas rejestracji należy podać:

```text
Email
Password
```

Hasło musi mieć co najmniej 8 znaków.

Każdy nowo zarejestrowany użytkownik otrzymuje domyślną rolę:

```text
User
```

## Jak zalogować się jako Admin

Aplikacja zawiera seed użytkownika administracyjnego.

Dane testowego administratora:

```text
Email: admin@example.com
Password: admin
Role: Admin
```

Seeder znajduje się w pliku:

```text
Data/DbSeeder.cs
```

Użytkownik admin jest tworzony automatycznie, jeśli w bazie nie istnieje jeszcze użytkownik o roli:

```text
Admin
```

Po zalogowaniu jako Admin można wejść na stronę:

```text
/Admin
```

## Gdzie jest kod hashowania hasła

Kod hashowania hasła znajduje się w:

```text
Controllers/AccountController.cs
```

Do hashowania używany jest mechanizm:

```csharp
PasswordHasher<AppUser>
```

Hasło jest hashowane podczas rejestracji:

```csharp
user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
```

Hasło jest weryfikowane podczas logowania:

```csharp
var result = _passwordHasher.VerifyHashedPassword(
    user,
    user.PasswordHash,
    model.Password);
```

Do bazy danych zapisywany jest tylko `PasswordHash`.
Hasło użytkownika nie jest zapisywane w postaci jawnej.

## Gdzie jest konfiguracja uwierzytelniania

Konfiguracja uwierzytelniania znajduje się w pliku:

```text
Program.cs
```

Aplikacja używa Cookie Authentication:

```csharp
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "UserPanelMvcAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
```

W tym samym pliku włączone są middleware:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## Które akcje są zabezpieczone przez [Authorize]

### DashboardController

Plik:

```text
Controllers/DashboardController.cs
```

Cały kontroler jest zabezpieczony atrybutem:

```csharp
[Authorize]
public class DashboardController : Controller
```

Oznacza to, że dostęp do poniższych akcji mają tylko zalogowani użytkownicy:

```text
GET  /Dashboard
POST /Dashboard/AddNote
```

Niezalogowany użytkownik zostanie przekierowany do:

```text
/Account/Login
```

### AdminController

Plik:

```text
Controllers/AdminController.cs
```

Cały kontroler jest zabezpieczony rolą `Admin`:

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
```

Oznacza to, że dostęp do strony:

```text
/Admin
```

mają tylko użytkownicy z rolą:

```text
Admin
```

Zwykły użytkownik z rolą `User` zostanie przekierowany do:

```text
/Account/AccessDenied
```

---

# Pytania do README

## Dlaczego nie wolno przechowywać haseł w postaci jawnej?

Jeżeli baza danych wycieknie, atakujący od razu pozna wszystkie hasła użytkowników. Użytkownicy często używają tych samych haseł w wielu serwisach, więc wyciek jednego systemu może prowadzić do przejęcia innych kont.

## Dlaczego sam SHA-256 nie jest dobrym wyborem do haseł?

SHA-256 jest bardzo szybki, co ułatwia ataki słownikowe i brute force. Do przechowywania haseł należy używać wolnych algorytmów przeznaczonych specjalnie do tego celu, takich jak PBKDF2, bcrypt, scrypt lub Argon2.

## Po co używa się soli?

Sól (salt) jest losową wartością dodawaną do hasła przed hashowaniem. Powoduje, że identyczne hasła mają różne hashe oraz chroni przed wykorzystaniem tęczowych tablic (rainbow tables).

## Czym różni się sól od pieprzu?

Sól (salt) jest unikalna dla każdego użytkownika i zwykle jest przechowywana razem z hashem w bazie danych. Pieprz (pepper) jest wspólnym sekretem całej aplikacji i powinien być przechowywany poza bazą danych, np. w konfiguracji lub User Secrets.

## Czym różni się uwierzytelnienie od autoryzacji?

Uwierzytelnienie (authentication) odpowiada na pytanie „kim jesteś?” i polega na potwierdzeniu tożsamości użytkownika. Autoryzacja (authorization) odpowiada na pytanie „co możesz zrobić?” i określa dostęp użytkownika do zasobów oraz operacji.

## Dlaczego ukrycie linku w widoku nie wystarcza jako zabezpieczenie?

Użytkownik może ręcznie wpisać adres URL w przeglądarce. Prawdziwe zabezpieczenie musi znajdować się po stronie serwera, np. poprzez atrybuty `[Authorize]` i `[Authorize(Roles = "Admin")]`.

## Dlaczego komunikat „nie ma takiego użytkownika” przy logowaniu może być problemem?

Pozwala atakującemu sprawdzić, które adresy e-mail istnieją w systemie. Ułatwia to ataki polegające na enumeracji użytkowników. Bezpieczniej jest zwracać jeden ogólny komunikat, np. „Nieprawidłowy email lub hasło”.