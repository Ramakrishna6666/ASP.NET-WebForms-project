using Films.Application.DTOs;

namespace Films.Application.Interfaces;

public interface IFilmService
{
    Task<IEnumerable<FilmDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FilmDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FilmDto> CreateAsync(FilmCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, FilmUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FilmDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
