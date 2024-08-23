using Xunit;
using CampaignApi.Fixtures;
using CampaignApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using CampaignApi.Models;

namespace CampaignApi.Tests;

public class CampaignTests : IClassFixture<AppFixtures>
{
    private readonly AppFixtures _fixture;

    public CampaignTests(AppFixtures fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void CanNotUseInvalidAPIKey()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var controller = new CampaignController(_fixture.Context);
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
        var request = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var Rutger = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "Rutger Groot");
        var controller = new CampaignController(_fixture.Context);
        request.Request.Headers["APIKey"] = Rutger?.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };
        var john = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "John Doe");

        // Act
        var query = await controller.ListAllCampaigns();

        // Assert
        var result = Assert.IsType<OkObjectResult>(query.Result);
        var campaigns = Assert.IsType<List<Campaign>>(result.Value);
        Assert.DoesNotContain(campaigns, c => c.CustomerId == john?.Id);
        Assert.Contains(campaigns, c => c.CustomerId == Rutger?.Id);
    }

    [Fact]
    public async void CanListOnlineCampaigns()
    {
        // Arrange
        var request = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var Rutger = _fixture.Context.Customers.FirstOrDefault(c => c.Name == "Rutger Groot");
        var controller = new CampaignController(_fixture.Context);
        request.Request.Headers["APIKey"] = Rutger?.APIKey.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = request
        };

        // Act
        var query = await controller.ListOnlineCampaigns();
        var queryAll = await controller.ListAllCampaigns();
        var offlineCampaign =
            ((queryAll.Result as OkObjectResult).Value as List<Campaign>).FirstOrDefault(c => !c.IsOnline());

        // Assert
        var result = Assert.IsType<OkObjectResult>(query.Result);
        var campaigns = Assert.IsType<List<Campaign>>(result.Value);
        Assert.IsType<Campaign>(offlineCampaign);
        Assert.All(campaigns, c => c.IsOnline());
        Assert.DoesNotContain(campaigns, c => c.Id == offlineCampaign.Id);
    }
}