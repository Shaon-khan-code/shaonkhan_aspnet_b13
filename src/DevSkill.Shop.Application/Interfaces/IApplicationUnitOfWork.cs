namespace DevSkill.Shop.Application.Interfaces;

public interface IApplicationUnitOfWork
{
    ITeamRepository Teams { get; }

    void Save();
}