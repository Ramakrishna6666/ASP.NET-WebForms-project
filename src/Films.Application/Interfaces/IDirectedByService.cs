using Films.Application.DTOs;

namespace Films.Application.Interfaces;

public interface IDirectedByService
{
    Task<IEnumerable<DirectedByDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DirectedByDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DirectedByDto> CreateAsync(DirectedByCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, DirectedByUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DirectedByDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
