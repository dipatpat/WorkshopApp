using WorkshopApp.DTOs;

namespace WorkshopApp.Services;

public interface IVisitsService
{
    Task<VisitDto> GetVisitByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken);



}