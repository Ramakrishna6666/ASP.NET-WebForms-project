using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Films.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Films.Infrastructure.Repositories;

public class SexRepository : ISexRepository
{
    private readonly FilmsDbContext _context;
    private readonly ILogger<SexRepository> _logger;

    public SexRepository(FilmsDbContext context, ILogger<SexRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Sex>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sexes
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all sexes");
            throw;
        }
    }

    public async Task<Sex?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sexes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sex with id {Id}", id);
            throw;
        }
    }

    public async Task<Sex> AddAsync(Sex sex, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Sexes.AddAsync(sex, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return sex;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding sex");
            throw;
        }
    }

    public async Task UpdateAsync(Sex sex, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Sexes.Update(sex);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sex with id {Id}", sex.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var sex = await _context.Sexes.FindAsync(new object[] { id }, cancellationToken);
            if (sex != null)
            {
                sex.IsActive = false;
                sex.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sex with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sexes.AnyAsync(s => s.Id == id && s.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if sex exists with id {Id}", id);
            throw;
        }
    }
}
