using System.Linq.Expressions;

namespace Infrastructure.Contracts;

public interface IRepository<T>
{
    Task<T> Create(T item);
    Task<T?> Read(int id);
    Task<T?> Get(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAll();
    Task<T> Update(T item);
    Task Delete(int id);
}