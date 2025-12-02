namespace Application.Abstractions;

public interface IExcelExportService<in TEntity>
{
    Task<byte[]> ExportToExcelAsync(IEnumerable<TEntity> data);
}