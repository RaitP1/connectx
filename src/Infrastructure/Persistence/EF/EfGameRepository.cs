using Application.Game.Dto;
using Application.Game.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public sealed class EfGameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public EfGameRepository(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<GameStateDto> List() =>
        _context.GameStates
            .AsNoTracking()
            .AsEnumerable()
            .Select(e => e.ToDomain())
            .ToList()
            .AsReadOnly();

    public void Save(GameStateDto state)
    {
        var existing = _context.GameStates.Find(state.Name);
        if (existing is not null)
            _context.GameStates.Remove(existing);

        _context.GameStates.Add(GameStateEntity.FromDomain(state));
        _context.SaveChanges();
    }

    public GameStateDto? Load(string name)
    {
        var entity = _context.GameStates.Find(name);
        return entity?.ToDomain();
    }

    public void Delete(string name)
    {
        var entity = _context.GameStates.Find(name);
        if (entity is not null)
        {
            _context.GameStates.Remove(entity);
            _context.SaveChanges();
        }
    }
}
