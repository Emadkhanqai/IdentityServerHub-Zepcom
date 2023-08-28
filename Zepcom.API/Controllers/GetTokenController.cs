using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class GetTokenController : ControllerBase
{
    [HttpGet("get-token")]
    public async Task<IActionResult> GetToken()
    {
        var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7114");
        if (disco.IsError)
        {
            return BadRequest(disco.Error);
        }

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "api",
            ClientSecret = "apisecret",
            Scope = "api1"
        });

        if (tokenResponse.IsError)
        {
            return BadRequest(tokenResponse.Error);
        }

        var apiClient = new HttpClient();
        apiClient.SetBearerToken(tokenResponse.AccessToken);

        var response = await apiClient.GetAsync("https://localhost:7263/api/products");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var content = await response.Content.ReadAsStringAsync();
        return Ok(content);
    }
}