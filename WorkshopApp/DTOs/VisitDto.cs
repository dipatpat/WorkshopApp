namespace WorkshopApp.DTOs;

public class VisitDto
{
    public DateTime date { get; set; }
    public ClientDto client { get; set; }
    public MechanicDto mechanic { get; set; }
    public List<VisitServicesDto> visitServices { get; set; } = new();
}