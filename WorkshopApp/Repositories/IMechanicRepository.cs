using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public interface IMechanicRepository
{
    Task<Mechanic?> GetMechanicByIdAsync(int clientId, CancellationToken cancellationToken);
    Task<Mechanic?> GetMechanicByLicenseAsync(string license, CancellationToken cancellationToken);


}