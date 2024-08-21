using Microsoft.AspNetCore.Mvc;
using CampaignApi.DTO;

namespace CampaignApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public CampaignController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet("ListAllCampaigns")]
    public IActionResult ListAllCampaigns()
    {
        // TODO: Implement logic to retrieve all campaigns
        // and return them as a response
        return Ok();
    }

    [HttpGet("ListOnlineCampaigns")]
    public IActionResult ListOnlineCampaigns()
    {
        // TODO: Implement logic to retrieve online campaigns
        // and return them as a response
        return Ok();
    }

    [HttpGet("CampaignDetails/{id}")]
    public IActionResult CampaignDetails(int id)
    {
        // TODO: Implement logic to retrieve campaign details
        // for the specified id and return them as a response
        return Ok();
    }
}
