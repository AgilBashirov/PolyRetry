using PollyRetry.Models;

namespace PollyRetry.Contracts;

public interface IGithubService
{
    Task<GithubUser> GetUserByUsernameAsync(string userName);
    Task<List<GithubUser>> GetUsersFromOrgAsync(string orgName);
}