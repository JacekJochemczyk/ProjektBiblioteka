using Biblioteka.Domain;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Infrastructure
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(LibraryDbContext db, CancellationToken ct = default)
        {
            // Migracje
            await db.Database.MigrateAsync(ct);

            // KATEGORIE – jeśli brak, to dodaj kilka przykładowych
            if (!await db.BookCategories.AnyAsync(ct))
            {
                db.BookCategories.AddRange(
                    new BookCategory { Name = "Literatura polska" },
                    new BookCategory { Name = "Lektura szkolna" },
                    new BookCategory { Name = "Powieść" }
                );

                await db.SaveChangesAsync(ct);
            }

            // KSIĄŻKI – jeśli jakiekolwiek już są, nic nie robimy
            if (await db.Books.AnyAsync(ct))
                return;

            // Pobieramy kategorie, żeby przypisać je do książek
            var litPolska = await db.BookCategories
                .FirstOrDefaultAsync(c => c.Name == "Literatura polska", ct);
            var lektura = await db.BookCategories
                .FirstOrDefaultAsync(c => c.Name == "Lektura szkolna", ct);
            var powiesc = await db.BookCategories
                .FirstOrDefaultAsync(c => c.Name == "Powieść", ct);

            // Dodajemy książki z przypisanymi kategoriami
            db.Books.AddRange(
                new Book
                {
                    Title = "Pan Tadeusz",
                    Author = "Adam Mickiewicz",
                    YearPublished = 1834,
                    BookCategoryId = litPolska?.Id ?? lektura?.Id
                },
                new Book
                {
                    Title = "Lalka",
                    Author = "Bolesław Prus",
                    YearPublished = 1890,
                    BookCategoryId = litPolska?.Id ?? powiesc?.Id
                },
                new Book
                {
                    Title = "Kamienie na szaniec",
                    Author = "Aleksander Kamiński",
                    YearPublished = 1943,
                    BookCategoryId = lektura?.Id
                }
            );

            await db.SaveChangesAsync(ct);
        }
    }
}
