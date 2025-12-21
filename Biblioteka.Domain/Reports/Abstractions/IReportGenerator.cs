using Biblioteka.Domain.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain.Reports.Abstractions
{
    public interface IReportGenerator
    {
        ReportFormat Format { get; }
        Task<ReportFile> GenerateAsync(
            IReadOnlyList<BookReportRow> rows,
            ReportRequest request,
            CancellationToken ct = default);
    }
}
