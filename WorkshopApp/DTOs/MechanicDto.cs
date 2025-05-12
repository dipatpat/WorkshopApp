using System.ComponentModel.DataAnnotations;

namespace WorkshopApp.DTOs;

public class MechanicDto
{
    public int mechanic_id { get; set; }
    
    [MaxLength(14)]
    public string license_number { get; set; }
}