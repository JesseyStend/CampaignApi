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
    private readonly AppDbContext _context;

    public CampaignController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("ListAllCampaigns")]
    public async Task<ActionResult<IEnumerable<Campaign>>> ListAllCampaigns()
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaigns = await _context.Campaigns
            .Where(campaign => campaign.Customer.Id == me.Id)
            .ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("ListOnlineCampaigns")]
    public async Task<ActionResult<IEnumerable<Campaign>>> ListOnlineCampaigns()
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaigns = await _context.Campaigns.Where(campaign =>
            campaign.Customer.Id == me.Id &&
            campaign.StartDate <= DateTime.Now &&
            campaign.EndDate >= DateTime.Now
        ).ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("CampaignDetails/{id}")]
    public async Task<ActionResult<Campaign>> CampaignDetails(Guid id)
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaign = await _context.Campaigns.FirstOrDefaultAsync(campaign =>
            campaign.Id == id
        );

        if (campaign == null)
        {
            return NotFound();
        }

        if (campaign.Customer.Id != me.Id)
        {
            return Unauthorized("You are not allowed to view this campaign");
        }

        return Ok(campaign);
    }
}
