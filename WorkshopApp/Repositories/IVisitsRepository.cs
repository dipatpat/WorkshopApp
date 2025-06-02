using WorkshopApp.DTOs;
using WorkshopApp.Models;

namespace WorkshopApp.Repositories;

public interface IVisitsRepository
{
    Task<Visit?> GetVisitByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetVisitsServices(int id, CancellationToken cancellationToken);
    Task<VisitDto?> GetFullVisitByIdAsync(int visitId, CancellationToken cancellationToken);
    Task<bool> ServiceExistsAsync(string serviceName, CancellationToken cancellationToken);

    Task InsertAppointmentAsync(int visitId, int clientId, int mechanicId, List<ServiceInputDto> services, CancellationToken cancellationToken);



}