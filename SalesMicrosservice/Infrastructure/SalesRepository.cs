using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class SalesRepository : IRepository<Sale>
{
    private readonly SalesContext _context;
    
    public SalesRepository(SalesContext context)
    {
        _context = context;
    }
    
    public async Task<Sale> Create(Sale item)
    {
        await _context.Sales.AddAsync(item);
        await _context.SaveChangesAsync();
        
        return item;
    }

    public async Task<Sale?> Read(int id)
    {
        return await _context.Sales.FindAsync(id);
    }

    public async Task<Sale?> Get(Expression<Func<Sale, bool>> predicate)
    {
        return await _context.Sales.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<Sale>> GetAll()
    {
        return await _context.Sales.ToListAsync();
    }

    public async Task<Sale> Update(Sale item)
    {
        _context.Sales.Update(item);
        await _context.SaveChangesAsync();
        
        return item;
    }

    public async Task Delete(int id)
    {
        var sale = await Read(id);

        _context.Sales.Remove(sale);
    }
}