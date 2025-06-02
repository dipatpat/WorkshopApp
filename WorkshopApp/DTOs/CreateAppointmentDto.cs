namespace WorkshopApp.DTOs;

public class CreateAppointmentDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public List<ServiceInputDto> Services { get; set; } = new();
}