using Microsoft.Extensions.DependencyInjection;
using StackShare.Infrastructure.Data;
using StackShare.IntegrationTests.Infrastructure;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using StackShare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;

namespace StackShare.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<StackShareWebApplicationFactory>, IAsyncLifetime
{
    protected readonly StackShareWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(StackShareWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task<string> GetAuthTokenAsync(string email = "test@example.com", string password = "Password123!")
    {
        // Register user first
        var registerRequest = new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password
        };

        var registerResponse = await Client.PostAsync("/api/auth/register", 
            new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

        if (!registerResponse.IsSuccessStatusCode)
        {
            var registerError = await registerResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to register user: {registerError}");
        }

        // Login to get token
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var loginResponse = await Client.PostAsync("/api/auth/login",
            new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        
        return loginResult.GetProperty("token").GetString()!;
    }

    protected void SetAuthToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<T>(content, options)!;
    }

    protected StringContent CreateJsonContent(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}