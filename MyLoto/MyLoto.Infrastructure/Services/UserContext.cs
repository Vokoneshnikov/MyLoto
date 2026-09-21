using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyLoto.Application.Abstractions;
using MyLoto.Application.Abstractions.Contexts;

namespace MyLoto.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Пользователь не авторизован или токен не содержит ID");
            }

            return userId;
        }
    }
}