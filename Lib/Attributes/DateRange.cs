using System.ComponentModel.DataAnnotations;

namespace CampaignApi.Attributes;

public class StartDateAttribute : RangeAttribute
{
    public StartDateAttribute()
        : base(typeof(DateTime), DateTime.MinValue.ToShortDateString(), DateTime.Now.ToShortDateString()) { }
}

public class EndDateAttribute : RangeAttribute
{
    public EndDateAttribute()
        : base(typeof(DateTime), DateTime.Now.ToShortDateString(), DateTime.MaxValue.ToShortDateString()) { }
}