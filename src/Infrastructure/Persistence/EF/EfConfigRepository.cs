using Application.Config.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public sealed class EfConfigRepository : IConfigRepository
{
    private readonly AppDbContext _context;

    public EfConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<GameConfig> List() =>
        _context.Configs
            .AsNoTracking()
            .AsEnumerable()
            .Select(e => e.ToDomain())
            .ToList()
            .AsReadOnly();

    public void Save(GameConfig config)
    {
        var existing = _context.Configs.Find(config.Name);
        if (existing is not null)
            _context.Configs.Remove(existing);

        _context.Configs.Add(ConfigEntity.FromDomain(config));
        _context.SaveChanges();
    }

    public GameConfig? Load(string name)
    {
        var entity = _context.Configs.Find(name);
        return entity?.ToDomain();
    }

    public void Delete(string name)
    {
        var entity = _context.Configs.Find(name);
        if (entity is not null)
        {
            _context.Configs.Remove(entity);
            _context.SaveChanges();
        }
    }
}
