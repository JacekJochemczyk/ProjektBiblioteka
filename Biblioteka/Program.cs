using Biblioteka.Components;
using Biblioteka.Domain;
using Biblioteka.Domain.Interpreter;
using Biblioteka.Domain.Reports.Abstractions;
using Biblioteka.Domain.Reports.Models;
using Biblioteka.Infrastructure;
using Biblioteka.Infrastructure.Auth;
using Biblioteka.Infrastructure.Reports;
using Biblioteka.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;



namespace Biblioteka
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // ---- KONFIGURACJA BAZY DANYCH ----
            var connection = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlite(connection));

            builder.Services.AddScoped<IReservationService, ReservationService>();

            // Identity + Role + Cookies
            builder.Services.AddIdentityCore<AppUser>(options =>
            {
                // Na start poluzujmy wymagania haseł, żeby szybciej testować
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<LibraryDbContext>()
            .AddSignInManager();

            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IReservationMediator, ReservationMediator>();

            builder.Services.AddScoped<IReportGenerator, PdfReportGenerator>();
            builder.Services.AddScoped<IReportGenerator, CsvReportGenerator>();
            builder.Services.AddScoped<IReportFactory, ReportFactory>();

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = "/login";     // gdzie przekierować, gdy brak zalogowania
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/";   // na razie
                });

            builder.Services.AddAuthorization(); // polityki dodamy później

            // Blazor potrzebuje AuthenticationState w drzewie komponentów
            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddSingleton<ILibraryRules, LibraryRules>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Migracje + seed książek
                var db = services.GetRequiredService<LibraryDbContext>();
                await DbInitializer.InitializeAsync(db);

                // Tworzenie ról i konta pracownika
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                string[] roles = new[] { "User", "Employee" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                const string adminEmail = "pracownik@biblioteka.local";
                const string adminPassword = "Pass123!"; // Na dev OK 

                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin is null)
                {
                    admin = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Employee");
                    }
                    else
                    {
                        Console.WriteLine("Błąd przy tworzeniu konta pracownika:");
                        foreach (var e in result.Errors)
                            Console.WriteLine($"   - {e.Description}");
                    }
                }
            }


            // Middleware autoryzacji i uwierzytelniania
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/admin/books-report/download", async (
                string? q,
                int? categoryId,
                string status,
                ReportFormat format,
                LibraryDbContext db,
                IReportFactory reportFactory,
                CancellationToken ct) =>
            {
                // 1) pobierz książki
                var books = await db.Books
                    .Include(b => b.Category)
                    .OrderBy(b => b.Author)
                    .ThenBy(b => b.Title)
                    .ToListAsync(ct);

                // 2) Interpreter – dokładnie jak na stronie
                IBookFilterExpression expr = new AllBooksExpression();
                expr = new AndExpression(expr, new TextSearchExpression(q ?? ""));
                expr = new AndExpression(expr, new CategoryExpression(categoryId));
                expr = new AndExpression(expr, new StatusExpression(status ?? "all"));

                var filtered = expr.Interpret(books).ToList();

                // 3) mapowanie do wierszy raportu
                var rows = filtered.Select(b => new BookReportRow
                {
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category?.Name ?? "-",
                    YearPublished = b.YearPublished,
                    Status = b.IsArchived
                        ? "Zarchiwizowana"
                        : (b.IsAvailable ? "Dostępna" : "Zarezerwowana"),
                    ReservedUntilText = b.ReservedUntil.HasValue
                        ? b.ReservedUntil.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                        : "-"
                }).ToList();

                // 4) request – data raportu + opis filtra
                var request = new ReportRequest
                {
                    GeneratedAt = DateTime.Now,
                    Query = $"q={q ?? ""}, categoryId={categoryId?.ToString() ?? "-"}, status={status}, format={format}"
                };

                // 5) Abstract Factory → generator → plik
                var generator = reportFactory.Create(format);
                var file = await generator.GenerateAsync(rows, request, ct);

                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Employee" });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            // Endpoint logowania - klasyczny POST HTTP
            app.MapPost("/auth/login", async (
                HttpContext httpContext,
                SignInManager<AppUser> signInManager) =>
            {
                var form = await httpContext.Request.ReadFormAsync();

                var email = form["Email"].ToString();
                var password = form["Password"].ToString();

                var result = await signInManager.PasswordSignInAsync(
                    email,
                    password,
                    isPersistent: true,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    // wracamy na /login z prostą informacją w query string
                    return Results.LocalRedirect("/login?error=1");
                }

                // sukces -> na stronę główną
                return Results.LocalRedirect("/");
            }).DisableAntiforgery(); // logowanie wyłączamy z antyforgery

            app.MapPost("/auth/logout", async (
                SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.LocalRedirect("/");
            }).DisableAntiforgery();


            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // QuestPDF: licencja community (wymagane przez QuestPDF)
            QuestPDF.Settings.License = LicenseType.Community;

            // Rejestracja fontu z wwwroot/fonts/DejaVuSans.ttf
            var fontPath = Path.Combine(app.Environment.WebRootPath, "fonts", "DejaVuSans.ttf");
            if (File.Exists(fontPath))
            {
                using var fs = File.OpenRead(fontPath);
                FontManager.RegisterFont(fs);
            }


            app.Run();
        }
    }
}
