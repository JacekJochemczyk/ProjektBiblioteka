using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public static class ReservationFactory
    {
        public static Reservation Create(int bookId, string userId, TimeSpan duration, DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;

            return new Reservation
            {
                BookId = bookId,
                UserId = userId,
                CreatedAt = now,
                ReservedUntil = now.Add(duration),
                Status = ReservationStatus.Created,
                CancellationReason = null
            };
        }
    }
}
