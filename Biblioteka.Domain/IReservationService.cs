using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public interface IReservationService
    {
        /// <summary>
        /// Tworzy rezerwację dla danej książki i użytkownika.
        /// Zwraca rezerwację lub null, jeśli książka nie jest dostępna / nie istnieje.
        /// </summary>
        Task<Reservation?> CreateReservationAsync(
            int bookId,
            string userId,
            TimeSpan duration,
            CancellationToken ct = default);

        /// <summary>
        /// Zwraca wszystkie rezerwacje danego użytkownika.
        /// </summary>
        Task<List<Reservation>> GetUserReservationsAsync(
            string userId,
            CancellationToken ct = default);

        /// <summary>
        /// Zwraca wszystkie rezerwacje (dla pracownika).
        /// </summary>
        Task<List<Reservation>> GetAllReservationsAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Zmienia status rezerwacji (np. Prepared, PickedUp, Cancelled).
        /// Zwraca false, jeśli rezerwacja nie istnieje.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int reservationId,
            ReservationStatus newStatus,
            string? cancellationReason = null,
            CancellationToken ct = default);

        /// <summary>
        /// Usuwa rezerwację z systemu. Zwraca false, jeśli nie znaleziono.
        /// </summary>
        Task<bool> DeleteReservationAsync(
            int reservationId,
            CancellationToken ct = default);
    }
}
