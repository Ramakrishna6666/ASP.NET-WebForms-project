using Films.Application.DTOs;
using Films.Domain.Entities;
using Films.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Films.Application.Services;

/// <summary>
/// Service implementation for actor operations
/// </summary>
public class ActorService : IActorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActorService> _logger;

    public ActorService(IUnitOfWork unitOfWork, ILogger<ActorService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ActorDto>> GetAllActorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all actors");
            var actors = await _unitOfWork.Actors.GetAllAsync(cancellationToken);
            return actors.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all actors");
            throw;
        }
    }

    public async Task<ActorDto?> GetActorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving actor with ID: {ActorId}", id);
            var actor = await _unitOfWork.Actors.GetByIdAsync(id, cancellationToken);
            return actor != null ? MapToDto(actor) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving actor with ID: {ActorId}", id);
            throw;
        }
    }

    public async Task<ActorDto> CreateActorAsync(CreateActorDto createActorDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new actor: {FirstName} {LastName}", createActorDto.FirstName, createActorDto.LastName);
            
            var actor = new Actor
            {
                FirstName = createActorDto.FirstName,
                LastName = createActorDto.LastName,
                BirthDate = createActorDto.BirthDate,
                SexId = createActorDto.SexId,
                CreatedDate = DateTime.UtcNow
            };

            var createdActor = await _unitOfWork.Actors.AddAsync(actor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Actor created successfully with ID: {ActorId}", createdActor.Id);
            return MapToDto(createdActor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating actor: {FirstName} {LastName}", createActorDto.FirstName, createActorDto.LastName);
            throw;
        }
    }

    public async Task<ActorDto> UpdateActorAsync(UpdateActorDto updateActorDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating actor with ID: {ActorId}", updateActorDto.Id);
            
            var actor = await _unitOfWork.Actors.GetByIdAsync(updateActorDto.Id, cancellationToken);
            if (actor == null)
            {
                throw new InvalidOperationException($"Actor with ID {updateActorDto.Id} not found");
            }

            actor.FirstName = updateActorDto.FirstName;
            actor.LastName = updateActorDto.LastName;
            actor.BirthDate = updateActorDto.BirthDate;
            actor.SexId = updateActorDto.SexId;
            actor.ModifiedDate = DateTime.UtcNow;

            await _unitOfWork.Actors.UpdateAsync(actor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Actor updated successfully with ID: {ActorId}", actor.Id);
            return MapToDto(actor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating actor with ID: {ActorId}", updateActorDto.Id);
            throw;
        }
    }

    public async Task DeleteActorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting actor with ID: {ActorId}", id);
            
            var exists = await _unitOfWork.Actors.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException($"Actor with ID {id} not found");
            }

            await _unitOfWork.Actors.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Actor deleted successfully with ID: {ActorId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting actor with ID: {ActorId}", id);
            throw;
        }
    }

    private static ActorDto MapToDto(Actor actor)
    {
        return new ActorDto
        {
            Id = actor.Id,
            FirstName = actor.FirstName,
            LastName = actor.LastName,
            BirthDate = actor.BirthDate,
            SexId = actor.SexId,
            SexName = actor.Sex?.Name
        };
    }
}
