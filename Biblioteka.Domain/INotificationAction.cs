using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{

    // Jedna “akcja” którą wykonujemy po kliknięciu w powiadomienie.
    // Cel: UI nie zna szczegółów, tylko wywołuje Execute().
    public interface INotificationAction
    {
        // np. "/my-reservations" albo "/admin/reservations"
        string TargetUrl { get; }

        // opcjonalnie: dodatkowe info (np. id rezerwacji w przyszłości)
        // na razie nie komplikujemy, zostawiamy tylko URL.
    }

}
