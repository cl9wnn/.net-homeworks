using Application.Abstractions;
using Core.Abstractions;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Infrastructure.Jobs;

public class ExcelExportJob(
    ILogger<ExcelExportJob> logger,
    IExcelExportService<UserRegistration> userExcelExportService,
    IServiceScopeFactory scopeFactory,
    IOptions<ExportSettings> options) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var exportDir = options.Value?.OutputDirectory ?? Directory.GetCurrentDirectory();
            Directory.CreateDirectory(exportDir);

            var today = DateTime.UtcNow.Date;
            var endTime = today.AddDays(1).AddTicks(-1);
            
            using var scope = scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRegistrationRepository>();
            
            var registrations = await repository.GetAllByTimeInterval(today, endTime);
            
            var excelFile = await userExcelExportService.ExportToExcelAsync(registrations);
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