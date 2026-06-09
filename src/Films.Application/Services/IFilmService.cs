using Films.Application.DTOs;

namespace Films.Application.Services;

/// <summary>
/// Service interface for film operations
/// </summary>
public interface IFilmService
{
    Task<IEnumerable<FilmDto>> GetAllFilmsAsync(CancellationToken cancellationToken = default);
    Task<FilmDto?> GetFilmByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FilmDto> CreateFilmAsync(CreateFilmDto createFilmDto, CancellationToken cancellationToken = default);
    Task<FilmDto> UpdateFilmAsync(UpdateFilmDto updateFilmDto, CancellationToken cancellationToken = default);
    Task DeleteFilmAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FilmDto>> SearchFilmsAsync(string searchTerm, CancellationToken cancellationToken = default);
}
