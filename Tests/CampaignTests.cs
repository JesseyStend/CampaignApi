using Xunit;
using CampaignApi.Fixtures;
using CampaignApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using CampaignApi.Models;
using CampaignApi.DTO;
using Microsoft.AspNetCore.Http;

namespace CampaignApi.Tests;

public class CampaignTests : IClassFixture<AppFixtures>
{
    private readonly AppFixtures _fixture;
    private DefaultHttpContext request;
    private CampaignController controller;

    private Customer rutger;
    private Campaign? offlineCampaign;

    public CampaignTests(AppFixtures fixture)
    {
        _fixture = fixture;
        request = new DefaultHttpContext();

        controller = new CampaignController(_fixture.Context);
        rutger = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "Rutger Groot")!;
        offlineCampaign = _fixture.Context.Campaigns.Where(c =>
            c.StartDate <= DateTime.Now &&
            (c.EndDate == null || c.EndDate >= DateTime.Now)
        ).FirstOrDefault();
    }

    [Fact]
    public async void CanNotUseInvalidAPIKey()
    {
        // Arrange
        request.Request.Headers["APIKey"] = Guid.NewGuid().ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };

        // Act
        var query = await controller.ListAllCampaigns();

        // Assert
        Assert.IsType<UnauthorizedResult>(query.Result);
    }

    [Fact]
    public async void CanNotSeeCampaignsFromOtherUsers()
    {
        // Arrange
        var controller = new CampaignController(_fixture.Context);
        request.Request.Headers["APIKey"] = rutger.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };
        var john = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "John Doe");
        var johnsCampaign = _fixture.Context.Campaigns.FirstOrDefault(c => c.CustomerId == john!.Id);

        // Act
        var query = await controller.ListAllCampaigns();

        // Assert
        var result = Assert.IsType<OkObjectResult>(query.Result);
        var campaigns = Assert.IsType<List<CampaignResponse>>(result.Value);
        Assert.Equal(3, campaigns.Count);
        Assert.DoesNotContain(campaigns, c => c.Id == johnsCampaign!.Id);
    }

    [Fact]
    public async void CanListOnlineCampaigns()
    {
        // Arrange
        var controller = new CampaignController(_fixture.Context);
        request.Request.Headers["APIKey"] = rutger?.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };

        // Act
        var query = await controller.ListOnlineCampaigns();
        var queryAll = await controller.ListAllCampaigns();

        // Assert
        var result = Assert.IsType<OkObjectResult>(query.Result);
        var campaigns = Assert.IsType<List<CampaignResponse>>(result.Value);
        Assert.IsType<Campaign>(offlineCampaign);
        Assert.All(campaigns, c => c.IsOnline());
        Assert.DoesNotContain(campaigns, c => c.Id == offlineCampaign.Id);
    }

    [Fact]
    public async void CanViewDetailsFromCampaign()
    {
        // Arrange
        request.Request.Headers["APIKey"] = rutger?.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };
        var campaign = _fixture.Context.Campaigns.FirstOrDefault(campaign => campaign.Name.Contains("has additional metrics"));

        // Act
        var query = await controller.CampaignDetails(campaign!.Id);

        // Assert
        var result = Assert.IsType<OkObjectResult>(query.Result);
        var campaignWithDetails = Assert.IsType<CampaignDetailedResponse>(result.Value);
        Assert.Equal(campaignWithDetails.Id, campaign.Id);
        Assert.True(campaignWithDetails.Impressions > 0);
        Assert.NotNull(campaignWithDetails.Customer);
        Assert.True(campaignWithDetails.IsOnline());
    }

    [Fact]
    public async void CanNowViewDetailsFromCampaignOfOthers()
    {
        // Arrange
        request.Request.Headers["APIKey"] = rutger?.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };
        var john = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "John Doe");
        var johnsCampaign = _fixture.Context.Campaigns.FirstOrDefault(c => c.CustomerId == john!.Id);

        // Act
        var query = await controller.CampaignDetails(johnsCampaign!.Id);

        // Assert
        var result = Assert.IsType<UnauthorizedObjectResult>(query.Result);
        Assert.Equal("You are not allowed to view this campaign", result.Value);
    }

}