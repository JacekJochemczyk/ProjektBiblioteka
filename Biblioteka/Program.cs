using Biblioteka.Components;
using Biblioteka.Infrastructure;
using Biblioteka.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Biblioteka.Domain;
using Biblioteka.Infrastructure.Services;



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
                // Na start poluzujmy wymagania hase³, ¿eby szybciej testowaæ
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

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.LoginPath = "/login";     // gdzie przekierowaæ, gdy brak zalogowania
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/";   // na razie
                });

            builder.Services.AddAuthorization(); // polityki dodamy póŸniej

            // Blazor potrzebuje AuthenticationState w drzewie komponentów
            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddSingleton<ILibraryRules, LibraryRules>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Migracje + seed ksi¹¿ek
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
                        Console.WriteLine("B³¹d przy tworzeniu konta pracownika:");
                        foreach (var e in result.Errors)
                            Console.WriteLine($"   - {e.Description}");
                    }
                }
            }


            // Middleware autoryzacji i uwierzytelniania
            app.UseAuthentication();
            app.UseAuthorization();

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
                    // wracamy na /login z prost¹ informacj¹ w query string
                    return Results.LocalRedirect("/login?error=1");
                }

                // sukces -> na stronê g³ówn¹
                return Results.LocalRedirect("/");
            }).DisableAntiforgery(); // logowanie wy³¹czamy z antyforgery

            app.MapPost("/auth/logout", async (
                SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.LocalRedirect("/");
            }).DisableAntiforgery();


            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
