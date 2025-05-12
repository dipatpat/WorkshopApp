using System.ComponentModel.DataAnnotations;

namespace WorkshopApp.Models;

public class Service
{
    public int service_id { get; set; }
    
    [MaxLength(100)]
    public string name { get; set; }
    public decimal base_fee { get; set; }
}