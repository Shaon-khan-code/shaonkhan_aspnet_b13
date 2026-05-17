using DevSkill.Shop.Application.Interfaces;
using DevSkill.Shop.Domain.Entities;
using DevSkill.Shop.Infrastructure.Data;

namespace DevSkill.Shop.Infrastructure.Repositories;

public class TeamRepository : Repository<Team>, ITeamRepository
{
    public TeamRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {

    }
}

