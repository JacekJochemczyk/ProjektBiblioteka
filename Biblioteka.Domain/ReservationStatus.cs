using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public enum ReservationStatus
    {
        Created = 0,      // czytelnik zarezerwował
        Prepared = 1,     // pracownik przygotował książkę
        PickedUp = 2,     // czytelnik odebrał
        Cancelled = 3,     // anulowana (brak odbioru / ręcznie)
        Returned = 4      // książka zwrócona do biblioteki
    }
}
