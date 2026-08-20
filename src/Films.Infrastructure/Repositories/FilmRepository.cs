using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Films.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Films.Infrastructure.Repositories;

public class FilmRepository : IFilmRepository
{
    private readonly FilmsDbContext _context;
    private readonly ILogger<FilmRepository> _logger;

    public FilmRepository(FilmsDbContext context, ILogger<FilmRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Film>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Films
                .AsNoTracking()
                .Where(f => f.IsActive)
                .OrderBy(f => f.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all films");
            throw;
        }
    }

    public async Task<Film?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Films
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id && f.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving film with id {Id}", id);
            throw;
        }
    }

    public async Task<Film> AddAsync(Film film, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Films.AddAsync(film, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return film;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding film");
            throw;
        }
    }

    public async Task UpdateAsync(Film film, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Films.Update(film);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating film with id {Id}", film.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var film = await _context.Films.FindAsync(new object[] { id }, cancellationToken);
            if (film != null)
            {
                film.IsActive = false;
                film.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting film with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Films.AnyAsync(f => f.Id == id && f.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if film exists with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Film>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Films
                .AsNoTracking()
                .Where(f => f.IsActive && 
                    (f.Title.Contains(searchTerm) || 
                     (f.Description != null && f.Description.Contains(searchTerm)) ||
                     (f.Genre != null && f.Genre.Contains(searchTerm))))
                .OrderBy(f => f.Title)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching films with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
