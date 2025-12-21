using Biblioteka.Domain.Reports.Abstractions;
using Biblioteka.Domain.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Biblioteka.Infrastructure.Reports;

public sealed class PdfReportGenerator : IReportGenerator
{
    public ReportFormat Format => ReportFormat.Pdf;

    public Task<ReportFile> GenerateAsync(
        IReadOnlyList<BookReportRow> rows,
        ReportRequest request,
        CancellationToken ct = default)
    {
        var generatedAtText = request.GeneratedAt.ToString("dd.MM.yyyy HH:mm");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontFamily("DejaVuSans").FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("RAPORT KSIĄŻEK").FontSize(16).Bold();
                    col.Item().Text($"Raport z dnia: {generatedAtText}");
                    if (!string.IsNullOrWhiteSpace(request.Query))
                        col.Item().Text($"Filtr: {request.Query}");
                    col.Item().PaddingTop(5).LineHorizontal(1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Tytuł
                        columns.RelativeColumn(2); // Autor
                        columns.RelativeColumn(2); // Kategoria
                        columns.RelativeColumn(2); // Status
                        columns.RelativeColumn(2); // Rezerwacja do
                    });

                    // nagłówek
                    table.Header(header =>
                    {
                        header.Cell().Element(CellHeader).Text("Tytuł");
                        header.Cell().Element(CellHeader).Text("Autor");
                        header.Cell().Element(CellHeader).Text("Kategoria");
                        header.Cell().Element(CellHeader).Text("Status");
                        header.Cell().Element(CellHeader).Text("Rezerwacja do");
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Element(CellBody).Text(r.Title);
                        table.Cell().Element(CellBody).Text(r.Author);
                        table.Cell().Element(CellBody).Text(r.Category);
                        table.Cell().Element(CellBody).Text(r.Status);
                        table.Cell().Element(CellBody).Text(r.ReservedUntilText ?? "-");
                    }

                    static IContainer CellHeader(IContainer c) =>
                        c.DefaultTextStyle(x => x.Bold())
                         .Background(Colors.Grey.Lighten3)
                         .PaddingVertical(6)
                         .PaddingHorizontal(4)
                         .Border(1)
                         .BorderColor(Colors.Grey.Lighten1);

                    static IContainer CellBody(IContainer c) =>
                        c.PaddingVertical(4)
                         .PaddingHorizontal(4)
                         .BorderBottom(1)
                         .BorderColor(Colors.Grey.Lighten2);
                });

                page.Footer().AlignRight().Text(txt =>
                {
                    txt.Span("Strona ");
                    txt.CurrentPageNumber();
                    txt.Span(" / ");
                    txt.TotalPages();
                });
            });
        });

        var pdfBytes = document.GeneratePdf();

        return Task.FromResult(new ReportFile
        {
            FileName = $"raport-ksiazek-{request.GeneratedAt:yyyyMMdd}.pdf",
            ContentType = "application/pdf",
            Content = pdfBytes
        });
    }
}
