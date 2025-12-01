using System.Linq.Expressions;

namespace Infrastructure.Contracts;

public interface IRepository<T>
{
    Task<T> Create(T type);
    Task<T?> Read(int id);
    Task<T?> Read(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAll();
    Task<T> Update(T type);
    Task<bool> Delete(int id);
}