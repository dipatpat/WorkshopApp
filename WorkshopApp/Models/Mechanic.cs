using System.ComponentModel.DataAnnotations;

namespace WorkshopApp.Models;

public class Mechanic
{
    public int mechanic_id { get; set; }
    [MaxLength(100)]
    public string first_name { get; set; }
    
    [MaxLength(100)]
    public string last_name { get; set; }
    
    [MaxLength(14)]
    public string license_number { get; set; }
}