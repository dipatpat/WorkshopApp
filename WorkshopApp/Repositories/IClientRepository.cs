using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public interface IClientRepository
{
    Task<Client?> GetClientByIdAsync(int clientId, CancellationToken cancellationToken);

}