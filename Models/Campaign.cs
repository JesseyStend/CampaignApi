
namespace CampaignApi.Models;
using System.ComponentModel.DataAnnotations;
using CampaignApi.Attributes;

public class Campaign
{
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required int Impressions { get; set; }
    public int Clicks { get; set; }
    public int Interactions { get; set; }
    public int Applications { get; set; }
    [Required]
    [StartDate]
    public required DateTime StartDate { get; set; }
    [EndDate]
    public DateTime? EndDate { get; set; }
    [Required]
    public required Guid CustomerId { get; set; }
    public required Customer Customer { get; set; }
}