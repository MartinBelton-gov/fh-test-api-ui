using System.Net.Http.Headers;
using System.Net;
using FamilyHub.Test.WeatherForcastUI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

//https://stackoverflow.com/questions/56204350/how-to-refresh-a-token-using-ihttpclientfactory

namespace FamilyHub.Test.WeatherForcastUI.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    public AuthenticationDelegatingHandler(ITokenService tokenService, IAuthService authService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            TokenModel tokenModel = new TokenModel
            {
                AccessToken = token,
                RefreshToken = _tokenService.GetRefreshToken()
            };

            tokenModel = await _authService.RefreshToken(tokenModel);
            if (tokenModel != null)
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(tokenModel.AccessToken);
                var claims = jwtSecurityToken.Claims.ToList();

                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);

                _tokenService.SetToken(tokenModel.AccessToken ?? string.Empty, jwtSecurityToken.ValidTo, tokenModel.RefreshToken ?? string.Empty);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{_tokenService.GetToken()}");
                response = await base.SendAsync(request, cancellationToken);
            }
            
        }

        return response;
    }
}
