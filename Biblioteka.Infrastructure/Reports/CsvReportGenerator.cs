using System.Text;
using Biblioteka.Domain.Reports.Abstractions;
using Biblioteka.Domain.Reports.Models;

namespace Biblioteka.Infrastructure.Reports
{
    public sealed class CsvReportGenerator : IReportGenerator
    {
        public ReportFormat Format => ReportFormat.Csv;

        public Task<ReportFile> GenerateAsync(
            IReadOnlyList<BookReportRow> rows,
            ReportRequest request,
            CancellationToken ct = default)
        {
            var sb = new StringBuilder();

            // Nagłówek raportu + data wygenerowania
            sb.AppendLine($"Raport z dnia: {request.GeneratedAt:dd.MM.yyyy}");
            sb.AppendLine();

            // CSV (separator ; żeby było wygodnie w polskim Excelu)
            sb.AppendLine("Tytuł;Autor;Kategoria;Status;Rezerwacja do");

            foreach (var r in rows)
            {
                sb.Append(CsvEscape(r.Title)).Append(';')
                  .Append(CsvEscape(r.Author)).Append(';')
                  .Append(CsvEscape(r.Category)).Append(';')
                  .Append(CsvEscape(r.Status)).Append(';')
                  .Append(CsvEscape(r.ReservedUntilText ?? "-"))
                  .AppendLine();
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            var file = new ReportFile
            {
                FileName = $"raport-ksiazek-{request.GeneratedAt:yyyyMMdd}.csv",
                ContentType = "text/csv; charset=utf-8",
                Content = bytes
            };

            return Task.FromResult(file);
        }

        private static string CsvEscape(string? value)
        {
            value ??= "";

            // Jeżeli są ; " lub enter — to zamykamy w cudzysłowie i podwajamy "
            var mustQuote = value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');

            if (value.Contains('"'))
                value = value.Replace("\"", "\"\"");

            return mustQuote ? $"\"{value}\"" : value;
        }
    }
}
