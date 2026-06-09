using Films.Domain.Entities;

namespace Films.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<Film> Films { get; }
    IRepository<Actor> Actors { get; }
    IRepository<Director> Directors { get; }
    IRepository<User> Users { get; }
    IRepository<Sex> Sexes { get; }
    IRepository<TypeUser> TypeUsers { get; }
    IRepository<Right> Rights { get; }
    IRepository<UserRight> UserRights { get; }
    IRepository<RefAF> ActorFilms { get; }
    IRepository<RefDAF> DirectorFilms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
