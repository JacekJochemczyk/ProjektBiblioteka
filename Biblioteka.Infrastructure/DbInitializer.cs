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

            // KATEGORIE – jeśli brak, dodaj zestaw
            if (!await db.BookCategories.AnyAsync(ct))
            {
                db.BookCategories.AddRange(
                    new BookCategory { Name = "Lektura szkolna" },
                    new BookCategory { Name = "Literatura polska" },
                    new BookCategory { Name = "Powieść" },
                    new BookCategory { Name = "Fantasy" },
                    new BookCategory { Name = "Kryminał" },
                    new BookCategory { Name = "Thriller" },
                    new BookCategory { Name = "Sci-Fi" },
                    new BookCategory { Name = "Reportaż" },
                    new BookCategory { Name = "Biografia" },
                    new BookCategory { Name = "Biznes i rozwój" },
                    new BookCategory { Name = "Popularnonaukowe" }
                );

                await db.SaveChangesAsync(ct);
            }

            // KSIĄŻKI – jeśli już są, nic nie robimy
            if (await db.Books.AnyAsync(ct))
                return;

            // mapa kategorii: Name -> Id
            var catMap = await db.BookCategories
                .AsNoTracking()
                .ToDictionaryAsync(c => c.Name, c => c.Id, ct);

            int Cat(string name) => catMap.TryGetValue(name, out var id) ? id : catMap.Values.First();

            // 50 sensownych pozycji + lata wydań współczesnych (wydania, nie “pierwsze wydanie”)
            var books = new List<Book>
            {
                // Lektury / klasyka 
                new() { Title="Pan Tadeusz", Author="Adam Mickiewicz", YearPublished=2018, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Lalka", Author="Bolesław Prus", YearPublished=2020, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Kamienie na szaniec", Author="Aleksander Kamiński", YearPublished=2021, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Dziady", Author="Adam Mickiewicz", YearPublished=2019, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Zemsta", Author="Aleksander Fredro", YearPublished=2017, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Wesele", Author="Stanisław Wyspiański", YearPublished=2016, BookCategoryId=Cat("Lektura szkolna"), IsAvailable=true },
                new() { Title="Quo Vadis", Author="Henryk Sienkiewicz", YearPublished=2022, BookCategoryId=Cat("Literatura polska"), IsAvailable=true },
                new() { Title="Krzyżacy", Author="Henryk Sienkiewicz", YearPublished=2015, BookCategoryId=Cat("Literatura polska"), IsAvailable=true },
                new() { Title="Chłopi", Author="Władysław Reymont", YearPublished=2020, BookCategoryId=Cat("Literatura polska"), IsAvailable=true },
                new() { Title="Ferdydurke", Author="Witold Gombrowicz", YearPublished=2014, BookCategoryId=Cat("Literatura polska"), IsAvailable=true },

                // Fantasy
                new() { Title="Wiedźmin: Ostatnie życzenie", Author="Andrzej Sapkowski", YearPublished=2020, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Miecz przeznaczenia", Author="Andrzej Sapkowski", YearPublished=2020, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Krew elfów", Author="Andrzej Sapkowski", YearPublished=2019, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Czas pogardy", Author="Andrzej Sapkowski", YearPublished=2019, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Chrzest ognia", Author="Andrzej Sapkowski", YearPublished=2018, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Wieża Jaskółki", Author="Andrzej Sapkowski", YearPublished=2018, BookCategoryId=Cat("Fantasy"), IsAvailable=true },
                new() { Title="Wiedźmin: Pani Jeziora", Author="Andrzej Sapkowski", YearPublished=2018, BookCategoryId=Cat("Fantasy"), IsAvailable=true },

                // Kryminał / thriller (PL i zagraniczne)
                new() { Title="Mock", Author="Marek Krajewski", YearPublished=2015, BookCategoryId=Cat("Kryminał"), IsAvailable=true },
                new() { Title="Behawiorysta", Author="Remigiusz Mróz", YearPublished=2018, BookCategoryId=Cat("Thriller"), IsAvailable=true },
                new() { Title="Kasacja", Author="Remigiusz Mróz", YearPublished=2019, BookCategoryId=Cat("Kryminał"), IsAvailable=true },
                new() { Title="Reina Roja", Author="Juan Gómez-Jurado", YearPublished=2021, BookCategoryId=Cat("Thriller"), IsAvailable=true },
                new() { Title="Pacjent", Author="Sebastian Fitzek", YearPublished=2020, BookCategoryId=Cat("Thriller"), IsAvailable=true },
                new() { Title="Dziewczyna z pociągu", Author="Paula Hawkins", YearPublished=2016, BookCategoryId=Cat("Thriller"), IsAvailable=true },
                new() { Title="Zaginiona dziewczyna", Author="Gillian Flynn", YearPublished=2015, BookCategoryId=Cat("Thriller"), IsAvailable=true },

                // Sci-Fi
                new() { Title="Problem trzech ciał", Author="Liu Cixin", YearPublished=2017, BookCategoryId=Cat("Sci-Fi"), IsAvailable=true },
                new() { Title="Ciemny las", Author="Liu Cixin", YearPublished=2018, BookCategoryId=Cat("Sci-Fi"), IsAvailable=true },
                new() { Title="Koniec śmierci", Author="Liu Cixin", YearPublished=2019, BookCategoryId=Cat("Sci-Fi"), IsAvailable=true },
                new() { Title="Marsjanin", Author="Andy Weir", YearPublished=2016, BookCategoryId=Cat("Sci-Fi"), IsAvailable=true },
                new() { Title="Artemis", Author="Andy Weir", YearPublished=2018, BookCategoryId=Cat("Sci-Fi"), IsAvailable=true },

                // Reportaż / biografia / popularnonaukowe
                new() { Title="Cesarz", Author="Ryszard Kapuściński", YearPublished=2014, BookCategoryId=Cat("Reportaż"), IsAvailable=true },
                new() { Title="Imperium", Author="Ryszard Kapuściński", YearPublished=2016, BookCategoryId=Cat("Reportaż"), IsAvailable=true },
                new() { Title="Sapiens", Author="Yuval Noah Harari", YearPublished=2018, BookCategoryId=Cat("Popularnonaukowe"), IsAvailable=true },
                new() { Title="Homo Deus", Author="Yuval Noah Harari", YearPublished=2019, BookCategoryId=Cat("Popularnonaukowe"), IsAvailable=true },
                new() { Title="Factfulness", Author="Hans Rosling", YearPublished=2019, BookCategoryId=Cat("Popularnonaukowe"), IsAvailable=true },
                new() { Title="Steve Jobs", Author="Walter Isaacson", YearPublished=2016, BookCategoryId=Cat("Biografia"), IsAvailable=true },
                new() { Title="Elon Musk", Author="Ashlee Vance", YearPublished=2017, BookCategoryId=Cat("Biografia"), IsAvailable=true },

                // Biznes / rozwój
                new() { Title="Atomowe nawyki", Author="James Clear", YearPublished=2020, BookCategoryId=Cat("Biznes i rozwój"), IsAvailable=true },
                new() { Title="Głęboka praca", Author="Cal Newport", YearPublished=2019, BookCategoryId=Cat("Biznes i rozwój"), IsAvailable=true },
                new() { Title="Esencjalista", Author="Greg McKeown", YearPublished=2018, BookCategoryId=Cat("Biznes i rozwój"), IsAvailable=true },
                new() { Title="Od zera do jedynki", Author="Peter Thiel", YearPublished=2017, BookCategoryId=Cat("Biznes i rozwój"), IsAvailable=true },

                // Powieść
                new() { Title="Normalni ludzie", Author="Sally Rooney", YearPublished=2020, BookCategoryId=Cat("Powieść"), IsAvailable=true },
                new() { Title="Zanim wystygnie kawa", Author="Toshikazu Kawaguchi", YearPublished=2021, BookCategoryId=Cat("Powieść"), IsAvailable=true },
                new() { Title="Gdzie śpiewają raki", Author="Delia Owens", YearPublished=2020, BookCategoryId=Cat("Powieść"), IsAvailable=true },
                new() { Title="Shantaram", Author="Gregory David Roberts", YearPublished=2015, BookCategoryId=Cat("Powieść"), IsAvailable=true },
                new() { Title="Małe życie", Author="Hanya Yanagihara", YearPublished=2017, BookCategoryId=Cat("Powieść"), IsAvailable=true },

                // Kryminał więcej
                new() { Title="Millennium: Mężczyźni, którzy nienawidzą kobiet", Author="Stieg Larsson", YearPublished=2015, BookCategoryId=Cat("Kryminał"), IsAvailable=true },
                new() { Title="Millennium: Dziewczyna, która igrała z ogniem", Author="Stieg Larsson", YearPublished=2016, BookCategoryId=Cat("Kryminał"), IsAvailable=true },
                new() { Title="Millennium: Zamek z piasku", Author="Stieg Larsson", YearPublished=2016, BookCategoryId=Cat("Kryminał"), IsAvailable=true },

                // Popularnonaukowe / reportaż więcej
                new() { Title="Krótka historia prawie wszystkiego", Author="Bill Bryson", YearPublished=2018, BookCategoryId=Cat("Popularnonaukowe"), IsAvailable=true },
                new() { Title="21 lekcji na XXI wiek", Author="Yuval Noah Harari", YearPublished=2020, BookCategoryId=Cat("Popularnonaukowe"), IsAvailable=true },
                new() { Title="Ludowa historia Polski", Author="Adam Leszczyński", YearPublished=2020, BookCategoryId=Cat("Reportaż"), IsAvailable=true },
                new() { Title="Zawód: reporter", Author="Ryszard Kapuściński", YearPublished=2014, BookCategoryId=Cat("Reportaż"), IsAvailable=true },
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync(ct);
        }
    }
}
