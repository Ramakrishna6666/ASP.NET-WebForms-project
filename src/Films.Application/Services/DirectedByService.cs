using AutoMapper;
using Films.Application.DTOs;
using Films.Application.Interfaces;
using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Films.Application.Services;

public class DirectedByService : IDirectedByService
{
    private readonly IDirectedByRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DirectedByService> _logger;

    public DirectedByService(IDirectedByRepository repository, IMapper mapper, ILogger<DirectedByService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<DirectedByDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all directors");
            var directors = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DirectedByDto>>(directors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all directors");
            throw;
        }
    }

    public async Task<DirectedByDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting director with id {Id}", id);
            var director = await _repository.GetByIdAsync(id, cancellationToken);
            return director != null ? _mapper.Map<DirectedByDto>(director) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting director with id {Id}", id);
            throw;
        }
    }

    public async Task<DirectedByDto> CreateAsync(DirectedByCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new director: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            var director = _mapper.Map<DirectedBy>(dto);
            var created = await _repository.AddAsync(director, cancellationToken);
            return _mapper.Map<DirectedByDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating director: {FirstName} {LastName}", dto.FirstName, dto.LastName);
            throw;
        }
    }

    public async Task UpdateAsync(int id, DirectedByUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating director with id {Id}", id);
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Director with id {id} not found");
            }

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating director with id {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting director with id {Id}", id);
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting director with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<DirectedByDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching directors with term: {SearchTerm}", searchTerm);
            var directors = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<DirectedByDto>>(directors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching directors with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
