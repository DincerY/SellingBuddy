using System.Security.Claims;

namespace BasketService.Api.Core.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string GetUserName() => "DincerYigit";
    public string GetUserName(int a) => _httpContextAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;

    public string GetUserName(string a) => _httpContextAccessor.HttpContext.User.Claims
        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

}