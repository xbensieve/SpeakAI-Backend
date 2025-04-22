using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken(List<Claim> claims);
        void HandleRefreshToken(string tokenInput, out string accessToken, out string refreshToken);
        List<Claim> DecodeToken(string token);
        bool Validation(string token);
        string? GetUserId(string token);
        string? GetUserId(List<Claim> claims);
        string? GetUserId(HttpContext httpContext);
        string GetRole(HttpContext httpContext);
        string GetRole(HttpRequest httpRequest);
        string GetRole(string token);
        string GetRole(List<Claim> claims);
        string GetAccessTokenByHeader(HttpRequest httpRequest);
        string GetAccessTokenByHeader(string authorizationValue);
        Guid? GetUserIdAsGuid(string token);
        Guid? GetUserIdAsGuid(List<Claim> claims);
        Guid? GetUserIdAsGuid(HttpContext httpContext);
    }
}
