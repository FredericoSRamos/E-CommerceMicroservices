using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly UsersContext _context;
    
    public UserRepository(UsersContext context)
    {
        _context = context;
    }

    public async Task<User> Create(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> Read(int id)
    {
        var user = await _context.Users.FindAsync(id);

        return user;
    }

    public async Task<User?> Read(Expression<Func<User, bool>> predicate)
    {
        var user = await _context.Users.FirstOrDefaultAsync(predicate);

        return user;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> Update(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> Delete(int id)
    {
        var user = await Read(id);

        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}