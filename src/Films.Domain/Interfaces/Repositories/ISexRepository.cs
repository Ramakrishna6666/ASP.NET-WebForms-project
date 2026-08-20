using Films.Domain.Entities;

namespace Films.Domain.Interfaces.Repositories;

public interface ISexRepository
{
    Task<IEnumerable<Sex>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sex?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Sex> AddAsync(Sex sex, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sex sex, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
