using System.Linq.Expressions;

namespace Infrastructure.Contracts;

public interface IRepository<T>
{
    Task<T> Create(T entity);
    Task<T?> Read(int id);
    Task<T?> Get(Expression<Func<T, bool>> predicate);
    Task<IList<T>> GetAll();
    Task<IList<T>> GetAll(Expression<Func<T, bool>> predicate);
    Task Update(T entity);
    Task Delete(int id);
}