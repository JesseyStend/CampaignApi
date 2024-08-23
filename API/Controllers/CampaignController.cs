using Microsoft.AspNetCore.Mvc;
using CampaignApi.DTO;
using CampaignApi.Context;
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
    public async Task<ActionResult<IEnumerable<CampaignResponse>>> ListAllCampaigns()
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaigns = await _context.Campaigns
            .Where(campaign => campaign.Customer.Id == me.Id)
            .Select(customer => new CampaignResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                StartDate = customer.StartDate,
                EndDate = customer.EndDate,
            })
            .ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("ListOnlineCampaigns")]
    public async Task<ActionResult<IEnumerable<CampaignResponse>>> ListOnlineCampaigns()
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaigns = await _context.Campaigns
            .Where(campaign =>
                campaign.Customer.Id == me.Id &&
                campaign.StartDate <= DateTime.Now &&
                campaign.EndDate >= DateTime.Now
            ).Select(customer => new CampaignResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                StartDate = customer.StartDate,
                EndDate = customer.EndDate,
            }).ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("CampaignDetails/{id}")]
    public async Task<ActionResult<CampaignDetailedResponse>> CampaignDetails(Guid id)
    {
        var APIKey = Request.Headers["APIKey"];
        var me = await _context.Customers.Where(customer => customer.APIKey.ToString() == APIKey).FirstOrDefaultAsync();

        if (me == null)
        {
            return Unauthorized();
        }

        var campaign = await _context.Campaigns
            .Select(campaign => new CampaignDetailedResponse
            {
                Id = campaign.Id,
                Name = campaign.Name,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                Impressions = campaign.Impressions,
                Clicks = campaign.Clicks,
                Interactions = campaign.Interactions,
                Applications = campaign.Applications,
                Customer = campaign.Customer
            })
            .FirstOrDefaultAsync(campaign =>
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
