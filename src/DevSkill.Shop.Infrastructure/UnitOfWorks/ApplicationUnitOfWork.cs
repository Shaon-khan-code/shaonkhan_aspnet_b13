using DevSkill.Shop.Application.Interfaces;
using DevSkill.Shop.Infrastructure.Data;

namespace DevSkill.Shop.Infrastructure.UnitOfWorks;

public class ApplicationUnitOfWork : IApplicationUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public ITeamRepository Teams { get; private set; }

    public ApplicationUnitOfWork(
        ApplicationDbContext dbContext,
        ITeamRepository teamRepository)
    {
        _dbContext = dbContext;
        Teams = teamRepository;
    }

    public void Save()
    {
        _dbContext.SaveChanges();
    }
}