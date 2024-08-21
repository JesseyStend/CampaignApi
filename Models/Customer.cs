
namespace CampaignApi.Models;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public required Guid APIKey { get; set; }
    public ICollection<Campaign> Campaigns { get; } = new List<Campaign>();
}