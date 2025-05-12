using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public interface IVisitsRepository
{
    Task<Visit?> GetVisitByIdAsync(int id, CancellationToken cancellationToken);

}