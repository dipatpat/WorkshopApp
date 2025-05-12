using Microsoft.AspNetCore.Mvc;
using WorkshopApp.Exceptions;
using WorkshopApp.Services;

namespace WorkshopApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VisitsController : ControllerBase
{
    private readonly IVisitsService _visitsService;   

    public VisitsController(IVisitsService visitsService)
    {
        _visitsService = visitsService;
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVisitByIdAsync(int id, CancellationToken cancellationToken)
    {
        var visit = await _visitsService.GetVisitByIdAsync(id, cancellationToken);
        return Ok(visit);
    }

    
}
