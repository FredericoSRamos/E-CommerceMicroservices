using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductsRepository : IRepository<Product>
{
    private ProductContext _context { get; set; }

    public ProductsRepository(ProductContext context)
    {
        _context = context;
    }
    
    public async Task<Product> Create(Product entity)
    {
        await _context.Products.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Product?> Read(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product?> Get(Expression<Func<Product, bool>> predicate)
    {
        return await _context.Products.FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<Product>> GetAll()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IList<Product>> GetAll(Expression<Func<Product, bool>> predicate)
    {
        return await _context.Products.Where(predicate).ToListAsync();
    }

    public async Task Update(Product entity)
    {
        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var entity = await _context.Products.FindAsync(id);
        _context.Products.Remove(entity);
        await _context.SaveChangesAsync();
    }
}