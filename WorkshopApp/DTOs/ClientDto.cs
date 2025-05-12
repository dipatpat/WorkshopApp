using System.ComponentModel.DataAnnotations;

namespace WorkshopApp.DTOs;

public class ClientDto
{
    [MaxLength(100)]
    public string first_name { get; set; }
    
    [MaxLength(100)]
    public string last_name { get; set; }
    
    public DateTime date_of_birth { get; set; }
}