using Biblioteka.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Infrastructure.Services
{
   
    // Implementacja wzorca Mediator.
    // Łączy IReservationService (logika rezerwacji) z INotificationService (powiadomienia)
    // i DbContextem (żeby znać tytuły książek, użytkowników itd.).

    public sealed class ReservationMediator : IReservationMediator
    {
        private readonly IReservationService _reservationService;
        private readonly INotificationService _notificationService;
        private readonly LibraryDbContext _db;

        public ReservationMediator(
            IReservationService reservationService,
            INotificationService notificationService,
            LibraryDbContext db)
        {
            _reservationService = reservationService;
            _notificationService = notificationService;
            _db = db;
        }

        public async Task<Reservation?> ReserveAsync(
            int bookId,
            string userId,
            CancellationToken ct = default)
        {
            // 1. Tworzymy rezerwację – cała logika w ReservationService.
            var reservation = await _reservationService.CreateReservationAsync(
                bookId,
                userId,
                TimeSpan.FromHours(72), // później łatwo tu podpiąć Singleton z ustawieniami
                ct);

            if (reservation is null)
                return null;

            // 2. Pobieramy tytuł książki do treści powiadomienia
            var book = await _db.Books
                .Where(b => b.Id == reservation.BookId)
                .Select(b => new { b.Title })
                .FirstOrDefaultAsync(ct);

            var title = book?.Title ?? "nieznana książka";

            // 3. Powiadamiamy wszystkich pracowników
            await _notificationService.AddForEmployeesAsync(
                $"Nowa rezerwacja książki „{title}”.",
                ct);

            return reservation;
        }

        public async Task<bool> MarkPreparedAsync(int reservationId, CancellationToken ct = default)
        {
            // pobieramy rezerwację z książką (żeby znać tytuł)
            var reservation = await _db.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == reservationId, ct);

            if (reservation is null)
                return false;

            // zmiana statusu – delegujemy do ReservationService
            var ok = await _reservationService.ChangeStatusAsync(
                reservationId,
                ReservationStatus.Prepared,
                null,
                ct);

            if (!ok)
                return false;

            var title = reservation.Book?.Title ?? "nieznana książka";

            // powiadamiamy czytelnika, że książka jest gotowa do odbioru
            await _notificationService.AddAsync(
                reservation.UserId,
                $"Książka „{title}” jest gotowa do odbioru w bibliotece.",
                ct);

            return true;
        }

        public Task<bool> MarkPickedUpAsync(int reservationId, CancellationToken ct = default)
        {
            return _reservationService.ChangeStatusAsync(
                reservationId,
                ReservationStatus.PickedUp,
                null,
                ct);
        }

        public Task<bool> MarkReturnedAsync(int reservationId, CancellationToken ct = default)
        {
            return _reservationService.ChangeStatusAsync(
                reservationId,
                ReservationStatus.Returned,
                null,
                ct);
        }

        public async Task<bool> CancelByUserAsync(
            int reservationId,
            string userId,
            CancellationToken ct = default)
        {
            // użytkownik może anulować tylko swoją rezerwację
            var reservation = await _db.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId, ct);

            if (reservation is null)
                return false;

            return await _reservationService.ChangeStatusAsync(
                reservationId,
                ReservationStatus.Cancelled,
                "Anulowane przez użytkownika",
                ct);
        }

        public async Task<bool> CancelByEmployeeAsync(
            int reservationId,
            string reason,
            CancellationToken ct = default)
        {
            var reservation = await _db.Reservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == reservationId, ct);

            if (reservation is null)
                return false;

            var ok = await _reservationService.ChangeStatusAsync(
                reservationId,
                ReservationStatus.Cancelled,
                reason,
                ct);

            if (!ok)
                return false;

            var title = reservation.Book?.Title ?? "nieznana książka";

            // powiadamiamy czytelnika, że jego rezerwacja została anulowana
            await _notificationService.AddAsync(
                reservation.UserId,
                $"Twoja rezerwacja książki „{title}” została anulowana. Powód: {reason}",
                ct);

            return true;
        }
    }
}
