using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{

    // Mediator koordynujący logikę rezerwacji:
    // rezerwacje + powiadomienia, bezpośrednio używany przez UI.

    public interface IReservationMediator
    {

        // Tworzy rezerwację książki dla użytkownika + powiadamia pracowników.

        Task<Reservation?> ReserveAsync(int bookId, string userId, CancellationToken ct = default);


        // Pracownik oznacza rezerwację jako "Gotowa do odbioru" + powiadamia czytelnika.

        Task<bool> MarkPreparedAsync(int reservationId, CancellationToken ct = default);

        Task<bool> MarkPickedUpAsync(int reservationId, CancellationToken ct = default);

        Task<bool> MarkReturnedAsync(int reservationId, CancellationToken ct = default);


        // Użytkownik anuluje własną rezerwację.

        Task<bool> CancelByUserAsync(int reservationId, string userId, CancellationToken ct = default);


        // Pracownik anuluje rezerwację z podanym powodem + powiadamia czytelnika.

        Task<bool> CancelByEmployeeAsync(int reservationId, string reason, CancellationToken ct = default);
    }
}
