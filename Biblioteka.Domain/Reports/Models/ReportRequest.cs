using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Reports.Models
{
    public sealed class ReportRequest
    {
        public DateTime GeneratedAt { get; init; } = DateTime.Now; // lokalnie, do wyświetlenia w raporcie
       

        public string? Query { get; init; }
    }
}
