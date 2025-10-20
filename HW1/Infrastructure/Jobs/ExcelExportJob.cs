using Application.Abstractions;
using Core.Abstractions;
using Core.Entities;
using Infrastructure.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.Jobs;

public class ExcelExportJob(
    ILogger<ExcelExportJob> logger,
    IExcelExportService<User> userExcelExportService,
    IUserRepository userRepository,
    IOptions<ExportSettings> options) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var exportDir = options.Value?.OutputDirectory ?? Directory.GetCurrentDirectory();
            Directory.CreateDirectory(exportDir);
        
            var users = await userRepository.GetAllAsync();
        
            var excelFile = await userExcelExportService.ExportToExcelAsync(users);
            var fileName = $"users_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var filePath = Path.Combine(exportDir, fileName);

            await File.WriteAllBytesAsync(filePath, excelFile);

            logger.LogInformation("Excel file saved successfully to {Path}", filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during excel export");
            throw;
        }
       
    }
}