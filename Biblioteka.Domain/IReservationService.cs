using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public interface IReservationService
    {
      
        // Tworzy rezerwację dla danej książki i użytkownika.
        // Zwraca rezerwację lub null, jeśli książka nie jest dostępna / nie istnieje.
       
        Task<Reservation?> CreateReservationAsync(
            int bookId,
            string userId,
            CancellationToken ct = default);

   
        // Zwraca wszystkie rezerwacje danego użytkownika.
     
        Task<List<Reservation>> GetUserReservationsAsync(
            string userId,
            CancellationToken ct = default);


        // Zwraca wszystkie rezerwacje (dla pracownika).
   
        Task<List<Reservation>> GetAllReservationsAsync(
            CancellationToken ct = default);

   
        // Zmienia status rezerwacji (np. Prepared, PickedUp, Cancelled).
        // Zwraca false, jeśli rezerwacja nie istnieje.

        Task<bool> ChangeStatusAsync(
            int reservationId,
            ReservationStatus newStatus,
            string? cancellationReason = null,
            CancellationToken ct = default);


        // Usuwa rezerwację z systemu. Zwraca false, jeśli nie znaleziono.

        Task<bool> DeleteReservationAsync(
            int reservationId,
            CancellationToken ct = default);
    }
}
