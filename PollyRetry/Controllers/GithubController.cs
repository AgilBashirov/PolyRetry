using Microsoft.AspNetCore.Mvc;
using PollyRetry.Contracts;

namespace PollyRetry.Controllers;

public class GithubController: ControllerBase
{
    private readonly IGithubService _githubService;

    public GithubController(IGithubService githubService)
    {
        _githubService = githubService;
    }

    [HttpGet("user/{userName}")]
    public async Task<IActionResult> GetUserByUsername(string userName)
    {
        var user = await _githubService.GetUserByUsernameAsync(userName);
        return user != null ? (IActionResult)Ok(user) : NotFound();
    }
    
    [HttpGet("orgs/{orgName}")]
    public async Task<IActionResult> GetUsersInOrg(string orgName)
    {
        var user = await _githubService.GetUsersFromOrgAsync(orgName);
        return user != null ? (IActionResult)Ok(user) : NotFound();
    }
}