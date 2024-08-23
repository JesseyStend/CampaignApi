
using CampaignApi.Models;

namespace CampaignApi.DTO;
public class CampaignDetailedResponse : CampaignResponse
{
    public int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Interactions { get; set; }
    public int Applications { get; set; }
    public Customer Customer { get; set; }
}