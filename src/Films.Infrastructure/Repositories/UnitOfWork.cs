using Films.Domain.Entities;
using Films.Domain.Interfaces;
using Films.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Films.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing database transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly FilmsDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Film>? _films;
    private IRepository<Actor>? _actors;
    private IRepository<Director>? _directors;
    private IRepository<User>? _users;
    private IRepository<Sex>? _sexes;
    private IRepository<TypeUser>? _typeUsers;
    private IRepository<Right>? _rights;
    private IRepository<UserRight>? _userRights;
    private IRepository<RefAF>? _actorFilms;
    private IRepository<RefDAF>? _directorFilms;

    public UnitOfWork(FilmsDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IRepository<Film> Films => _films ??= new Repository<Film>(_context);
    public IRepository<Actor> Actors => _actors ??= new Repository<Actor>(_context);
    public IRepository<Director> Directors => _directors ??= new Repository<Director>(_context);
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Sex> Sexes => _sexes ??= new Repository<Sex>(_context);
    public IRepository<TypeUser> TypeUsers => _typeUsers ??= new Repository<TypeUser>(_context);
    public IRepository<Right> Rights => _rights ??= new Repository<Right>(_context);
    public IRepository<UserRight> UserRights => _userRights ??= new Repository<UserRight>(_context);
    public IRepository<RefAF> ActorFilms => _actorFilms ??= new Repository<RefAF>(_context);
    public IRepository<RefDAF> DirectorFilms => _directorFilms ??= new Repository<RefDAF>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
