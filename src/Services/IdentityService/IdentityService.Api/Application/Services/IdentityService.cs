using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Api.Application.Services;

public class IdentityService : IIdentityService
{
    public Task<LoginResponseModel> LoginAsync(LoginRequestModel requestModel)
    {
        var claim = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, requestModel.UserName),
            new Claim(ClaimTypes.Name, "Dincer Yigit"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Thisismyverylongsecretkeyitistrue"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddDays(10);

        var token = new JwtSecurityToken(claims: claim,expires: expiry, signingCredentials: creds,
            notBefore: DateTime.UtcNow);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

        LoginResponseModel response = new()
        {
            UserToken = encodedJwt,
            UserName = requestModel.UserName
        };

        return Task.FromResult(response);
    }
}