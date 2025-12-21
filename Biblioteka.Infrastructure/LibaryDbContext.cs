using Biblioteka.Domain;
using Biblioteka.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Infrastructure
{
    public sealed class LibraryDbContext : IdentityDbContext<AppUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookCategory> BookCategories => Set<BookCategory>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<Notification> Notifications => Set<Notification>();



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // KONFIGURACJA KSIĄŻKI
            modelBuilder.Entity<Book>(b =>
            {
                b.ToTable("Books");
                b.HasKey(x => x.Id);

                b.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(x => x.Author)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(x => x.YearPublished)
                    .IsRequired()
                    .HasDefaultValue(2000);

                b.Property(x => x.IsAvailable)
                    .HasDefaultValue(true);
                
                b.Property(x => x.IsArchived)
                    .HasDefaultValue(false);

                b.Property(x => x.ReservedUntil)
                    .IsRequired(false);

                // 🔗 Relacja z kategorią (wiele książek → jedna kategoria)
                b.HasOne(x => x.Category)
                    .WithMany(c => c.Books)
                    .HasForeignKey(x => x.BookCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // KONFIGURACJA KATEGORII
            modelBuilder.Entity<BookCategory>(c =>
            {
                c.ToTable("BookCategories");
                c.HasKey(x => x.Id);

                c.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Unikalna nazwa kategorii (żeby nie mieć dwóch "Fantasy")
                c.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Reservation>(r =>
            {
                r.ToTable("Reservations");

                r.HasKey(x => x.Id);

                r.Property(x => x.UserId)
                    .IsRequired()
                    .HasMaxLength(450); // tyle ma domyślnie Id użytkownika Identity

                r.Property(x => x.CreatedAt)
                    .IsRequired();

                r.Property(x => x.ReservedUntil)
                    .IsRequired();

                r.Property(x => x.Status)
                    .IsRequired();

                r.Property(x => x.CancellationReason)
                    .HasMaxLength(500)
                    .IsRequired(false);

                // relacja z Book: jedna książka może mieć wiele rezerwacji
                r.HasOne(x => x.Book)
                    .WithMany(b => b.Reservations)
                    .HasForeignKey(x => x.BookId)
                    .OnDelete(DeleteBehavior.Restrict); // nie kasujemy rezerwacji przy usunięciu książki

            });

            modelBuilder.Entity<Notification>(n =>
            {
                n.ToTable("Notifications");
                n.HasKey(x => x.Id);

                n.Property(x => x.UserId)
                    .IsRequired()
                    .HasMaxLength(450); // tyle ma standardowo Id w AspNetUsers

                n.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(500); // na razie wystarczy

                n.Property(x => x.Type)   // ⬅️ NOWE
                    .IsRequired();

                n.Property(x => x.IsRead)
                    .HasDefaultValue(false);

                n.Property(x => x.CreatedAt)
                    .IsRequired();
            });



        }
    }
}
