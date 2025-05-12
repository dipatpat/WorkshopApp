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

        var visit = await _visitsRepository.GetVisitByIdAsync(visitId, cancellationToken);
        if (visit == null)
        {
            throw new NotFoundException($"Visit with id {visitId} not found");
        }

        var clientId = visit.client_id;

        var client = await _clientRepository.GetClientByIdAsync(clientId, cancellationToken);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {clientId} not found");
        }

        var clientDto = new ClientDto
        {
            first_name = client.first_name,
            last_name = client.last_name,
            date_of_birth = client.date_of_birth,
        };

        var mechanicId = visit.mechanic_id;
        var mechanic = await _mechanicRepository.GetMechanicByIdAsync(mechanicId, cancellationToken);
        if (mechanic == null)
        {
            throw new NotFoundException($"Mechanic with id {mechanicId} not found");
        }

        var mechanicDto = new MechanicDto
        {
            mechanic_id = mechanic.mechanic_id,
            license_number = mechanic.license_number,
        };
        var visitDto = new VisitDto
        {
            date = visit.date,
            client = clientDto,
            mechanic = mechanicDto,
        };
        return visitDto;
    }
}