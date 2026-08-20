using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Films.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Films.Infrastructure.Repositories;

public class DirectedByRepository : IDirectedByRepository
{
    private readonly FilmsDbContext _context;
    private readonly ILogger<DirectedByRepository> _logger;

    public DirectedByRepository(FilmsDbContext context, ILogger<DirectedByRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<DirectedBy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Directors
                .AsNoTracking()
                .Include(d => d.Sex)
                .Where(d => d.IsActive)
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all directors");
            throw;
        }
    }

    public async Task<DirectedBy?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Directors
                .AsNoTracking()
                .Include(d => d.Sex)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving director with id {Id}", id);
            throw;
        }
    }

    public async Task<DirectedBy> AddAsync(DirectedBy director, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Directors.AddAsync(director, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return director;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding director");
            throw;
        }
    }

    public async Task UpdateAsync(DirectedBy director, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Directors.Update(director);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating director with id {Id}", director.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var director = await _context.Directors.FindAsync(new object[] { id }, cancellationToken);
            if (director != null)
            {
                director.IsActive = false;
                director.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting director with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Directors.AnyAsync(d => d.Id == id && d.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if director exists with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<DirectedBy>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Directors
                .AsNoTracking()
                .Include(d => d.Sex)
                .Where(d => d.IsActive && 
                    (d.FirstName.Contains(searchTerm) || 
                     d.LastName.Contains(searchTerm) ||
                     (d.Nationality != null && d.Nationality.Contains(searchTerm))))
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching directors with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
