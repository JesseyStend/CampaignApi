using Microsoft.AspNetCore.Mvc;
using CampaignApi.DTO;

namespace CampaignApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public Task<HealthStatusResponse> Get()
    {
        return Task.FromResult(new HealthStatusResponse { Status = "Healthy" });
    }
}
