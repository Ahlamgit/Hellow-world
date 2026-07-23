using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Khadamati.Application.Common;

namespace Khadamati.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<KhadamatiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(KhadamatiWebApplicationFactory factory) =>
        _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/api/v1/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("healthy");
    }

    [Fact]
    public async Task HealthReady_ReturnsReadyWithDatabase()
    {
        var response = await _client.GetAsync("/api/v1/health/ready");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("ready");
        body.Should().Contain("connected");
    }

    [Fact]
    public async Task AdminDashboard_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/admin/dashboard");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithSeededAdmin_ReturnsTokenWithPermissions()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@khadamati.com",
            password = "Admin@123456",
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.GetProperty("accessToken").GetString().Should().NotBeNullOrWhiteSpace();
        payload.Data.GetProperty("user").GetProperty("permissions").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AdminDashboard_WithAdminToken_ReturnsOk()
    {
        var token = await LoginAsync("admin@khadamati.com", "Admin@123456");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin/dashboard");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminAnalytics_WithoutReportsPermission_ReturnsForbidden()
    {
        var token = await LoginAsync("craftsman1@khadamati.com", "Craftsman@123");
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin/analytics/data");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NearbyCraftsmen_WithCoordinates_ReturnsOrderedResults()
    {
        var servicesResponse = await _client.GetAsync("/api/v1/services");
        servicesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var servicesPayload = await servicesResponse.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();
        var serviceId = servicesPayload!.Data.EnumerateArray()
            .First(s => s.GetProperty("nameEn").GetString() == "Leak Repair")
            .GetProperty("id").GetString();
        serviceId.Should().NotBeNullOrWhiteSpace();

        var response = await _client.GetAsync(
            $"/api/v1/bookings/craftsmen/nearby?serviceId={serviceId}&latitude=33.8938&longitude=35.5018&radiusKm=50");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();
        payload!.Success.Should().BeTrue();
        payload.Data.GetArrayLength().Should().BeGreaterThan(0);
    }

    private async Task<string> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<JsonElement>>();
        return payload!.Data.GetProperty("accessToken").GetString()!;
    }
}
