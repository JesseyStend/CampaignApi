using Microsoft.AspNetCore.Mvc;
using CampaignApi.DTO;
using CampaignApi.Context;
using CampaignApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CampaignApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly AppDbContext _context;

    public CampaignController(ILogger<HealthController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("ListAllCampaigns")]
    public IActionResult ListAllCampaigns()
    {
        var query = _context.Customers.FirstOrDefaultAsync(customer => customer.APIKey == Request.Headers["APIKey"]);

        if (query?.Result == null)
        {
            return Unauthorized();
        }

        Customer me = query.Result;

        var campaigns = _context.Campaigns.Where(campaign => campaign.Customer.Id == me.Id).ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("ListOnlineCampaigns")]
    public IActionResult ListOnlineCampaigns()
    {
        var query = _context.Customers.FirstOrDefaultAsync(customer => customer.APIKey == Request.Headers["APIKey"]);

        if (query?.Result == null)
        {
            return Unauthorized();
        }

        Customer me = query.Result;

        var campaigns = _context.Campaigns.Where(campaign =>
            campaign.Customer.Id == me.Id &&
            campaign.StartDate <= DateTime.Now &&
            campaign.EndDate >= DateTime.Now
        ).ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("CampaignDetails/{id}")]
    public IActionResult CampaignDetails(Guid id)
    {
        var query = _context.Customers.FirstOrDefaultAsync(customer => customer.APIKey == Request.Headers["APIKey"]);

        if (query?.Result == null)
        {
            return Unauthorized();
        }

        Customer me = query.Result;

        var campaign = _context.Campaigns.FirstOrDefaultAsync(campaign =>
            campaign.Customer.Id == me.Id &&
            campaign.Id == id
        );

        if (campaign?.Result == null)
        {
            return NotFound();
        }

        return Ok(campaign);
    }
}
