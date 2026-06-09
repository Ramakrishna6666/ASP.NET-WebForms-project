using Films.Application.DTOs;
using Films.Domain.Entities;
using Films.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Films.Application.Services;

/// <summary>
/// Service implementation for film operations
/// </summary>
public class FilmService : IFilmService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FilmService> _logger;

    public FilmService(IUnitOfWork unitOfWork, ILogger<FilmService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<FilmDto>> GetAllFilmsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all films");
            var films = await _unitOfWork.Films.GetAllAsync(cancellationToken);
            return films.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all films");
            throw;
        }
    }

    public async Task<FilmDto?> GetFilmByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving film with ID: {FilmId}", id);
            var film = await _unitOfWork.Films.GetByIdAsync(id, cancellationToken);
            return film != null ? MapToDto(film) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving film with ID: {FilmId}", id);
            throw;
        }
    }

    public async Task<FilmDto> CreateFilmAsync(CreateFilmDto createFilmDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new film: {Title}", createFilmDto.Title);
            
            var film = new Film
            {
                Title = createFilmDto.Title,
                Description = createFilmDto.Description,
                Year = createFilmDto.Year,
                Genre = createFilmDto.Genre,
                CreatedDate = DateTime.UtcNow
            };

            var createdFilm = await _unitOfWork.Films.AddAsync(film, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Film created successfully with ID: {FilmId}", createdFilm.Id);
            return MapToDto(createdFilm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating film: {Title}", createFilmDto.Title);
            throw;
        }
    }

    public async Task<FilmDto> UpdateFilmAsync(UpdateFilmDto updateFilmDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating film with ID: {FilmId}", updateFilmDto.Id);
            
            var film = await _unitOfWork.Films.GetByIdAsync(updateFilmDto.Id, cancellationToken);
            if (film == null)
            {
                throw new InvalidOperationException($"Film with ID {updateFilmDto.Id} not found");
            }

            film.Title = updateFilmDto.Title;
            film.Description = updateFilmDto.Description;
            film.Year = updateFilmDto.Year;
            film.Genre = updateFilmDto.Genre;
            film.ModifiedDate = DateTime.UtcNow;

            await _unitOfWork.Films.UpdateAsync(film, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Film updated successfully with ID: {FilmId}", film.Id);
            return MapToDto(film);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating film with ID: {FilmId}", updateFilmDto.Id);
            throw;
        }
    }

    public async Task DeleteFilmAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting film with ID: {FilmId}", id);
            
            var exists = await _unitOfWork.Films.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException($"Film with ID {id} not found");
            }

            await _unitOfWork.Films.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Film deleted successfully with ID: {FilmId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting film with ID: {FilmId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<FilmDto>> SearchFilmsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching films with term: {SearchTerm}", searchTerm);
            
            var films = await _unitOfWork.Films.FindAsync(
                f => f.Title.Contains(searchTerm) || 
                     (f.Description != null && f.Description.Contains(searchTerm)),
                cancellationToken);

            return films.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching films with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    private static FilmDto MapToDto(Film film)
    {
        return new FilmDto
        {
            Id = film.Id,
            Title = film.Title,
            Description = film.Description,
            Year = film.Year,
            Genre = film.Genre,
            CreatedDate = film.CreatedDate,
            ModifiedDate = film.ModifiedDate
        };
    }
}
