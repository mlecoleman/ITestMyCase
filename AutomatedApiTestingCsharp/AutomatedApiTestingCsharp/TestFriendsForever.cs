using FluentAssertions;
using Xunit.Abstractions;
using FluentAssertions.Execution;

namespace AutomatedApiTestingCsharp;

public class TestFriendsForever
{
    private ITestOutputHelper _logger;
    public TestFriendsForever(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    // Class Example GET
    [Fact]
    public async Task SteamStatsTest()
    {
        string url = "https://www.valvesoftware.com/about/stats";
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://www.valvesoftware.com");

        var result = await client.GetAsync("/about/stats");
        var content = await result.Content.ReadAsStringAsync();
        _logger.WriteLine(content);

        var stats = System.Text.Json.JsonSerializer.Deserialize<SteamStatsResponse>(content);

        using (new AssertionScope())
        {
            int.Parse(stats.users_online, System.Globalization.NumberStyles.AllowThousands).Should().BeGreaterThanOrEqualTo(0);
            //int.Parse(stats.users_online.Replace(",", "")).Should().BeGreaterThanOrEqualTo(0);
            result.IsSuccessStatusCode.Should().BeTrue();
        }

    }
    // GET request with query parameters that uses a Love Calculator API to determine if "Pants" + "Class Example" = Love
    [Fact]
    public async Task IsPantsAGoodClassExampleTest()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://love-calculator.p.rapidapi.com/getPercentage?sname=Class%20Example&fname=pants"),
            Headers =
            {
                { "X-RapidAPI-Key", "ca4b23e227msh1d31a44600af94dp128ccdjsn2df60625cea5" },
                { "X-RapidAPI-Host", "love-calculator.p.rapidapi.com" },
            },
        };
        var result = await client.SendAsync(request);
        var content = await result.Content.ReadAsStringAsync();

        var match = System.Text.Json.JsonSerializer.Deserialize<LoveCalcResponse>(content);

        using (new AssertionScope())
        {
            match.result.Should().Be("Congratulations! Good choice");
            int.Parse(match.percentage).Should().Be(81);
        }
    }

    // Class Example POST
    [Fact]
    public async Task RequestResultTest()
    {
        HttpClient client = new()
        {
            BaseAddress = new("https://reqres.in/")
        };

        RegisterPostModel postBody = new RegisterPostModel()
        {
            username = "eve.holt@reqres.in",
            email = "eve.holt@reqres.in",
            password = "pistol"
        };

        var serialized = System.Text.Json.JsonSerializer.Serialize(postBody);
        var reponse = await client.PostAsync("/api/register",
            new StringContent(serialized, encoding: System.Text.Encoding.UTF8, "application/json"));

        var responseContent = await reponse.Content.ReadAsStringAsync();
        //responseContent.Should().BeNullOrEmpty();  -->  Assertion for Delete
        var responseAsModel = System.Text.Json.JsonSerializer.Deserialize<RegisterResponseModel>(responseContent);

        using (new AssertionScope())
        {
            reponse.IsSuccessStatusCode.Should().BeTrue();
            responseAsModel.id.Should().Be(4);
            responseAsModel.token.Should().NotBeNullOrEmpty();
        }
    }

    // PUT request for Yoda Translator API
    [Fact]
    public async Task YodaTest()
    {
        HttpClient client = new()
        {
            BaseAddress = new("https://api.funtranslations.com/")
        };

        YodaPostModel postBody = new YodaPostModel()
        {
            text = "The inside joke from the QA cohort is pants."
        };

        var serialized = System.Text.Json.JsonSerializer.Serialize(postBody);
        var reponse = await client.PostAsync("/translate/yoda", new StringContent(serialized, encoding: System.Text.Encoding.UTF8, "application/json"));

        var responseContent = await reponse.Content.ReadAsStringAsync();
        var responseAsModel = System.Text.Json.JsonSerializer.Deserialize<Root>(responseContent);

        using (new AssertionScope())
        {
            reponse.IsSuccessStatusCode.Should().BeTrue();
            responseAsModel.contents.text.Should().Be("The inside joke from the QA cohort is pants.");
            responseAsModel.contents.translated.Should().Be("Pants,  the inside joke from the qa cohort is.");
        }
    }

    // DELETE request for reqres fake API
    [Fact]
    public async Task DeleteUserTest()
    {
        HttpClient client = new()
        {
            BaseAddress = new("https://reqres.in/")
        };

        var result = await client.DeleteAsync("/api/users/2");
        var responseContent = await result.Content.ReadAsStringAsync();

        responseContent.Should().BeNullOrEmpty();
    }

    // Negative Test - Sending an empty query does not return a successful status code
    [Fact]
    public async Task WhereInTheWorldIsCarmenSandiegoTest()
    {
        HttpClient client = new()
        {
            BaseAddress = new("https://api.nationalize.io")
        };

        var result = await client.GetAsync("");
        var content = await result.Content.ReadAsStringAsync();


        var response = System.Text.Json.JsonSerializer.Deserialize<NationalResponseModel>(content);
        _logger.WriteLine(content);

        result.IsSuccessStatusCode.Should().BeFalse();
    }
}