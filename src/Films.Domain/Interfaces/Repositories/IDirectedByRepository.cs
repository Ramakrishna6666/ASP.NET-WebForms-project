using Films.Domain.Entities;

namespace Films.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for DirectedBy entity
/// </summary>
public interface IDirectedByRepository
{
    Task<IEnumerable<DirectedBy>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DirectedBy?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DirectedBy> AddAsync(DirectedBy director, CancellationToken cancellationToken = default);
    Task UpdateAsync(DirectedBy director, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DirectedBy>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
