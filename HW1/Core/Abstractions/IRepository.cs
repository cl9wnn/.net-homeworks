namespace Core.Abstractions;

public interface IRepository<in TKey, TEntity>
{
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity?> GetAsync(TKey id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task<TEntity?> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TKey id);
    Task<bool> IsExistsAsync(TKey id);
}