using System.Drawing;
using Application.Abstractions;
using Core.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Excel;

public class UserExcelExportService: IExcelExportService<UserRegistration>
{
    private static readonly string[] Headers =
    [
        "ID", "Username", "Email", "Registered At", "ReceivedAt At"
    ];
    
    public UserExcelExportService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    public async Task<byte[]> ExportToExcelAsync(IEnumerable<UserRegistration> users)
    {
        using var package = new ExcelPackage();
        
        var worksheet = package.Workbook.Worksheets.Add("Users");
        
        AddHeaders(worksheet);
        AddUserData(worksheet, users);

        worksheet.Cells.AutoFitColumns();

        return await package.GetAsByteArrayAsync();
    }

    private static void AddHeaders(ExcelWorksheet worksheet)
    {
        for (var i = 0; i < Headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = Headers[i];
        }

        using var headerRange = worksheet.Cells[1, 1, 1, Headers.Length];

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
    }

    private static void AddUserData(ExcelWorksheet worksheet, IEnumerable<UserRegistration> users)
    {
        var row = 2;
        foreach (var user in users)
        {
            worksheet.Cells[row, 1].Value = user.UserId;
            worksheet.Cells[row, 2].Value = user.Username;
            worksheet.Cells[row, 3].Value = user.Email;
            
            worksheet.Cells[row, 4].Value = user.RegisteredAt;
            worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd";

            row++;
        }
    }
}