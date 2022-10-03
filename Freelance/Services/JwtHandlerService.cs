using Freelance.Api.Extensions;
using Freelance.Api.Interfaces;
using Freelance.Core.Models.Storage;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Freelance.Api.Services;

/// <summary>
/// Служба для работы с токенами доступа.
/// </summary>
public class JwtHandlerService : IJwtHandler
{
    private readonly IConfiguration _configuration;

    public JwtHandlerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Генерация токена доступа.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>Токен доступа.</returns>
    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimsPrincipalExtensions.CLAIM_USER_ID, user.Id.ToString()),
                new Claim(ClaimsPrincipalExtensions.CLAIM_USER_UUID, user.UniqueIdentifier.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

        var token = new JwtSecurityToken(
          _configuration["Jwt:Issuer"],
          _configuration["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddDays(1),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
