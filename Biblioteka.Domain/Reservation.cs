using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain
{
    public sealed class Reservation : BaseEntity
    {
        
        // powiązanie z książką
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        // użytkownik (Identity user) – trzymamy tylko Id jako string
        public string UserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime ReservedUntil { get; set; }

        public ReservationStatus Status { get; set; }

        public string? CancellationReason { get; set; }

        // mała wygodna właściwość – przyda się później
        public bool IsActive =>
            Status == ReservationStatus.Created ||
            Status == ReservationStatus.Prepared;
    }
}
