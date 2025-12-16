using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public static class ReservationFactory
    {
        public static Reservation Create(int bookId, string userId, DateTime reservedUntil)
        {
            return new Reservation
            {
                BookId = bookId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ReservedUntil = reservedUntil,
                Status = ReservationStatus.Created,
                CancellationReason = null
            };
        }
    }
}
