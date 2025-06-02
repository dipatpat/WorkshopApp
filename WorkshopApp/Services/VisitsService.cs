using WorkshopApp.DTOs;
using WorkshopApp.Exceptions;
using WorkshopApp.Models;
using WorkshopApp.Repositories;

namespace WorkshopApp.Services;

public class VisitsService : IVisitsService
{
    private readonly IClientRepository _clientRepository;
    private readonly IMechanicRepository _mechanicRepository;
    private readonly IVisitsRepository _visitsRepository;

    public VisitsService(IClientRepository clientRepository, IMechanicRepository mechanicRepository,
        IVisitsRepository visitsRepository)
    {
        _clientRepository = clientRepository;
        _mechanicRepository = mechanicRepository;
        _visitsRepository = visitsRepository;
    }
    

    public async Task<VisitDto> GetVisitByIdAsync(int visitId, CancellationToken cancellationToken)
    {
        if (visitId <= 0)
        {
            throw new BadRequestException("VisitId must be greater than zero.");
        }

        var visitDto = await _visitsRepository.GetFullVisitByIdAsync(visitId, cancellationToken);
        if (visitDto == null)
        {
            throw new NotFoundException($"Visit with id {visitId} not found");
        }

        return visitDto;
    }
    
    public async Task AddAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken)
    {
        if (dto.AppointmentId <= 0 || dto.PatientId <= 0 || string.IsNullOrWhiteSpace(dto.LicenseNumber))
            throw new BadRequestException("Invalid input data.");

        if (dto.Services == null || dto.Services.Count == 0)
            throw new BadRequestException("At least one service is required.");

        var existing = await _visitsRepository.GetVisitByIdAsync(dto.AppointmentId, cancellationToken);
        if (existing != null)
            throw new ConflictException($"Appointment with ID {dto.AppointmentId} already exists.");

        var patient = await _clientRepository.GetClientByIdAsync(dto.PatientId, cancellationToken);
        if (patient == null)
            throw new NotFoundException($"Patient with ID {dto.PatientId} not found.");

        var doctor = await _mechanicRepository.GetMechanicByLicenseAsync(dto.LicenseNumber, cancellationToken);
        if (doctor == null)
            throw new NotFoundException($"Doctor with License Number {dto.LicenseNumber} not found.");

        foreach (var s in dto.Services)
        {
            bool exists = await _visitsRepository.ServiceExistsAsync(s.ServiceName, cancellationToken);
            if (!exists)
                throw new NotFoundException($"Service '{s.ServiceName}' not found.");
        }

        await _visitsRepository.InsertAppointmentAsync(dto.AppointmentId, dto.PatientId, doctor.mechanic_id, dto.Services, cancellationToken);
    }
    
    


}