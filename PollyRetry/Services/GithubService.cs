using System.Net;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using PollyRetry.Contracts;
using PollyRetry.Models;

namespace PollyRetry.Services;

public class GithubService: IGithubService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly int MaxRetries = 3;
    private static readonly Random Random = new Random();
    private readonly AsyncRetryPolicy _retryPolicy;

    public GithubService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _retryPolicy = Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(MaxRetries, times => 
                TimeSpan.FromSeconds(times * 100));
            //.RetryAsync(MaxRetries);
    }
    public async Task<GithubUser> GetUserByUsernameAsync(string username)
    {
        
        var client = _httpClientFactory.CreateClient("GitHub");

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var random = Random.Next(1, 3);
            if (random == 1) throw new HttpRequestException("This is a fake request exception");
            var result = await client.GetAsync($"users/{username}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var resultString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GithubUser>(resultString);
        });
    }
    
    public async Task<List<GithubUser>> GetUsersFromOrgAsync(string orgName)
    {
        var client = _httpClientFactory.CreateClient("GitHub");

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var result = await client.GetAsync($"orgs/{orgName}");
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var resultString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<GithubUser>>(resultString)!;
        });
    }

}