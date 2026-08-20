using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Films.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Films.Infrastructure.Repositories;

public class TypeUserRepository : ITypeUserRepository
{
    private readonly FilmsDbContext _context;
    private readonly ILogger<TypeUserRepository> _logger;

    public TypeUserRepository(FilmsDbContext context, ILogger<TypeUserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TypeUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TypeUsers
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all type users");
            throw;
        }
    }

    public async Task<TypeUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TypeUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving type user with id {Id}", id);
            throw;
        }
    }

    public async Task<TypeUser> AddAsync(TypeUser typeUser, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.TypeUsers.AddAsync(typeUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return typeUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding type user");
            throw;
        }
    }

    public async Task UpdateAsync(TypeUser typeUser, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.TypeUsers.Update(typeUser);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating type user with id {Id}", typeUser.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var typeUser = await _context.TypeUsers.FindAsync(new object[] { id }, cancellationToken);
            if (typeUser != null)
            {
                typeUser.IsActive = false;
                typeUser.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting type user with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.TypeUsers.AnyAsync(t => t.Id == id && t.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if type user exists with id {Id}", id);
            throw;
        }
    }
}
