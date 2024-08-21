
using CampaignApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CampaignApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Campaign> Campaigns { get; set; }
}