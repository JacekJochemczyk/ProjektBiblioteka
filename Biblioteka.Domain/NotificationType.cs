using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public enum NotificationType
    {
        General = 0,
        ReservationCreated = 1,
        ReservationPrepared = 2,
        ReservationCancelled = 3,
        ReservationPickedUp = 4,
        ReservationReturned = 5
    }
}
