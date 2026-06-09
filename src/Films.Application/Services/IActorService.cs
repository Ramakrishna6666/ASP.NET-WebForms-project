using Films.Application.DTOs;

namespace Films.Application.Services;

/// <summary>
/// Service interface for actor operations
/// </summary>
public interface IActorService
{
    Task<IEnumerable<ActorDto>> GetAllActorsAsync(CancellationToken cancellationToken = default);
    Task<ActorDto?> GetActorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto, CancellationToken cancellationToken = default);
    Task<ActorDto> UpdateActorAsync(UpdateActorDto updateActorDto, CancellationToken cancellationToken = default);
    Task DeleteActorAsync(int id, CancellationToken cancellationToken = default);
}
