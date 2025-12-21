using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Reports.Models
{
    public sealed class BookReportRow
    {
        public string Title { get; init; } = "";
        public string Author { get; init; } = "";
        public string Category { get; init; } = "";
        public int YearPublished { get; init; }
        public string Status { get; init; } = "";          // np. "Dostępna", "Zarezerwowana", "Zarchiwizowana"
        public string? ReservedUntilText { get; init; }        // string, bo w CSV/PDF i tak finalnie tekst
    }
}
