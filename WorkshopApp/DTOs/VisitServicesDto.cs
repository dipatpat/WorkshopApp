using System.ComponentModel.DataAnnotations;

namespace WorkshopApp.DTOs;

public class VisitServicesDto
{
    [MaxLength(100)]
    public string name { get; set; }
    public decimal service_fee { get; set; }
}