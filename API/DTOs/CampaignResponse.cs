namespace CampaignApi.DTO;

public class CampaignResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsOnline()
    {
        return StartDate <= DateTime.Now && (EndDate == null || EndDate >= DateTime.Now);
    }
}