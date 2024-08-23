using CampaignApi.Context;
using CampaignApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CampaignApi.Fixtures;

public class AppFixtures : IDisposable
{
    public AppDbContext Context { get; private set; }

    public AppFixtures()
    {
        // Set up the database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDataBase")
            .Options;

        Context = new AppDbContext(options);

        Console.WriteLine("Seeding database with customers");
        var john = Context.Customers.Add(new Customer { Name = "John Doe", APIKey = Guid.NewGuid() });
        var rutger = Context.Customers.Add(new Customer { Name = "Rutger Groot", APIKey = Guid.NewGuid() });
        Context.SaveChanges();

        Console.WriteLine("Seeding database with campaigns");
        Context.Campaigns.Add(new Campaign
        {
            Name = "Campaign 1",
            CustomerId = rutger.Entity.Id,
            StartDate = DateTime.Now,
            Impressions = 1000
        });

        Context.Campaigns.Add(new Campaign
        {
            Name = "Campaign 2 - is offline",
            CustomerId = rutger.Entity.Id,
            StartDate = DateTime.Now.AddMonths(-3),
            EndDate = DateTime.Now.AddMonths(-1),
            Impressions = 20
        });

        Context.Campaigns.Add(new Campaign
        {
            Name = "Campaign 3 - has additional metrics",
            CustomerId = rutger.Entity.Id,
            StartDate = DateTime.Now,
            Impressions = 300,
            Clicks = 100,
            Interactions = 10,
            Applications = 1
        });

        Context.Campaigns.Add(new Campaign
        {
            Name = "Campaign 4 - from another user",
            CustomerId = john.Entity.Id,
            StartDate = DateTime.Now,
            Impressions = 5,
        });

        Context.SaveChanges();
    }

    public void Dispose()
    {
        // Clean up the database
        Context.Dispose();
    }
}
