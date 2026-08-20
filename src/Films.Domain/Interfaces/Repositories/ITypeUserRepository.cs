using Films.Domain.Entities;

namespace Films.Domain.Interfaces.Repositories;

public interface ITypeUserRepository
{
    Task<IEnumerable<TypeUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TypeUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TypeUser> AddAsync(TypeUser typeUser, CancellationToken cancellationToken = default);
    Task UpdateAsync(TypeUser typeUser, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
