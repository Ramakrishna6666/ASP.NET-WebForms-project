using AutoMapper;
using Films.Application.DTOs;
using Films.Application.Interfaces;
using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Films.Application.Services;

public class FilmService : IFilmService
{
    private readonly IFilmRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<FilmService> _logger;

    public FilmService(IFilmRepository repository, IMapper mapper, ILogger<FilmService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<FilmDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all films");
            var films = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<FilmDto>>(films);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all films");
            throw;
        }
    }

    public async Task<FilmDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting film with id {Id}", id);
            var film = await _repository.GetByIdAsync(id, cancellationToken);
            return film != null ? _mapper.Map<FilmDto>(film) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting film with id {Id}", id);
            throw;
        }
    }

    public async Task<FilmDto> CreateAsync(FilmCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new film: {Title}", dto.Title);
            var film = _mapper.Map<Film>(dto);
            var created = await _repository.AddAsync(film, cancellationToken);
            return _mapper.Map<FilmDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating film: {Title}", dto.Title);
            throw;
        }
    }

    public async Task UpdateAsync(int id, FilmUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating film with id {Id}", id);
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Film with id {id} not found");
            }

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating film with id {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting film with id {Id}", id);
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting film with id {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<FilmDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching films with term: {SearchTerm}", searchTerm);
            var films = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<FilmDto>>(films);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching films with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
