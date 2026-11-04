using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services;

public class ExcelExportService
{
    public byte[] ExportEmployeeSearchResultsToExcel(IEnumerable<EmployeeSearchResultDto> employees, string? filterSummary = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Employee Search Results");

        int currentRow = 1;

        // Title Header
        worksheet.Cell(currentRow, 1).Value = "Peoria County Sheriff's Office - Personnel List";
        var titleRange = worksheet.Range(currentRow, 1, currentRow, 8);
        titleRange.Merge();
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 14;
        titleRange.Style.Font.FontColor = XLColor.White;
        titleRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B365D"); // Dark Navy
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Row(currentRow).Height = 30;

        currentRow++;

        // Metadata / Filter Summary
        if (!string.IsNullOrWhiteSpace(filterSummary))
        {
            worksheet.Cell(currentRow, 1).Value = $"Search Criteria: {filterSummary}";
            var filterRange = worksheet.Range(currentRow, 1, currentRow, 8);
            filterRange.Merge();
            filterRange.Style.Font.Italic = true;
            filterRange.Style.Font.FontSize = 10;
            filterRange.Style.Font.FontColor = XLColor.DarkGray;
            currentRow++;
        }

        worksheet.Cell(currentRow, 1).Value = $"Exported: {DateTime.Now:MM/dd/yyyy hh:mm tt} | Total Records: {employees?.Count() ?? 0}";
        var infoRange = worksheet.Range(currentRow, 1, currentRow, 8);
        infoRange.Merge();
        infoRange.Style.Font.Italic = true;
        infoRange.Style.Font.FontSize = 10;
        infoRange.Style.Font.FontColor = XLColor.DarkGray;
        currentRow += 2; // Extra blank row before table

        // Table Header
        int headerRow = currentRow;
        string[] headers = new[]
        {
            "Name",
            "Status",
            "Badge Number",
            "Hire Date",
            "Program",
            "Unit",
            "Inactive Date",
            "Sheriff Employee"
        };

        for (int col = 0; col < headers.Length; col++)
        {
            var cell = worksheet.Cell(headerRow, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2C3E50");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        worksheet.Row(headerRow).Height = 24;

        // Data Rows
        if (employees != null && employees.Any())
        {
            foreach (var item in employees)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = item.FormattedName ?? string.Empty;
                worksheet.Cell(currentRow, 2).Value = item.Status ?? string.Empty;
                worksheet.Cell(currentRow, 3).Value = item.BadgeNumber ?? string.Empty;

                if (item.HireDate.HasValue)
                {
                    worksheet.Cell(currentRow, 4).Value = item.HireDate.Value.Date;
                    worksheet.Cell(currentRow, 4).Style.DateFormat.Format = "MM/dd/yyyy";
                }
                else
                {
                    worksheet.Cell(currentRow, 4).Value = string.Empty;
                }

                worksheet.Cell(currentRow, 5).Value = item.Program ?? string.Empty;
                worksheet.Cell(currentRow, 6).Value = item.Unit ?? string.Empty;

                if (item.InactiveDate.HasValue)
                {
                    worksheet.Cell(currentRow, 7).Value = item.InactiveDate.Value.Date;
                    worksheet.Cell(currentRow, 7).Style.DateFormat.Format = "MM/dd/yyyy";
                }
                else
                {
                    worksheet.Cell(currentRow, 7).Value = string.Empty;
                }

                worksheet.Cell(currentRow, 8).Value = item.EmployeeFlag ? "Yes" : "No";

                // Alignments
                worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Alternate row background shading
                if ((currentRow - headerRow) % 2 == 0)
                {
                    worksheet.Range(currentRow, 1, currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8F9FA");
                }
            }

            // Apply borders to table
            var tableRange = worksheet.Range(headerRow, 1, currentRow, 8);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.OutsideBorderColor = XLColor.LightGray;
            tableRange.Style.Border.InsideBorderColor = XLColor.LightGray;
        }

        // Adjust column widths automatically
        worksheet.Columns().AdjustToContents(10.0, 50.0);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

