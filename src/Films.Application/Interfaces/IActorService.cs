using Films.Application.DTOs;

namespace Films.Application.Interfaces;

public interface IActorService
{
    Task<IEnumerable<ActorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ActorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ActorDto> CreateAsync(ActorCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, ActorUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActorDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
