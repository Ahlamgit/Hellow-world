using ClosedXML.Excel;
using Khadamati.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Khadamati.Infrastructure.Services;

public class AdminExportService : IAdminExportService
{
    public AdminExportService() => QuestPDF.Settings.License = LicenseType.Community;

    public byte[] ToExcel(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(sheetName.Length > 31 ? sheetName[..31] : sheetName);
        for (var c = 0; c < headers.Count; c++)
            ws.Cell(1, c + 1).Value = headers[c];
        for (var r = 0; r < rows.Count; r++)
            for (var c = 0; c < headers.Count; c++)
                ws.Cell(r + 2, c + 1).Value = c < rows[r].Count ? rows[r][c] ?? "" : "";
        ws.Row(1).Style.Font.Bold = true;
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ToPdf(string title, IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string?>> rows)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text(title).FontSize(16).Bold();
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        for (var i = 0; i < headers.Count; i++) cols.RelativeColumn();
                    });
                    table.Header(h =>
                    {
                        foreach (var header in headers)
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(header).FontSize(9).Bold();
                    });
                    foreach (var row in rows)
                    {
                        for (var c = 0; c < headers.Count; c++)
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4)
                                .Text(c < row.Count ? row[c] ?? "" : "").FontSize(8);
                    }
                });
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("KHADAMATI Admin Export — ");
                    t.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"));
                });
            });
        }).GeneratePdf();
    }
}
