using AutoMapper;
using Films.Application.DTOs;
using Films.Application.Interfaces;
using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Films.Application.Services;

public class ActorService : IActorService
{
    private readonly IActorRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<ActorService> _logger;

    public ActorService(IActorRepository repository, IMapper mapper, ILogger<ActorService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ActorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all actors");
            var actors = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ActorDto>>(actors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all actors");
            throw;
        }
    }

    public async Task<ActorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting actor with id {Id}", id);
            var actor = await _repository.GetByIdAsync(id, cancellationToken);
            return actor != null ? _mapper.Map<ActorDto>(actor) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting actor with id {Id}", id);
            throw;
        }
    }

    public async Task<ActorDto> CreateAsync(ActorCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new actor: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            var actor = _mapper.Map<Actor>(dto);
            var created = await _repository.AddAsync(actor, cancellationToken);
            return _mapper.Map<ActorDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating actor: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            throw;
        }
    }

    public async Task UpdateAsync(int id, ActorUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating actor with id {Id}", id);
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Actor with id {id} not found");
            }

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating actor with id {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting actor with id {Id}", id);
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting actor with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<ActorDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching actors with term: {SearchTerm}", searchTerm);
            var actors = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<ActorDto>>(actors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching actors with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
