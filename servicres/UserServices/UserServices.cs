using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ecommerceapi.Data;
using Ecommerceapi.Dtos.token;
using Ecommerceapi.Dtos.UserDtos;
using Ecommerceapi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
namespace Ecommerceapi.services.UserServices
{
    public class UserServices(AppDBContext context, IConfiguration configuration) : IUserServices
    {
        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return null!;
            }
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            };
        }

      public async Task<Accesstoken?> LoginUserAsync(
    LoginUserDto loginUserDto)
{
    if (loginUserDto is null)
    {
        throw new ArgumentNullException(nameof(loginUserDto));
    }

    var user = await context.Users
        .FirstOrDefaultAsync(
            u => u.Username == loginUserDto.Username);

    if (user is null)
    {
        return null;
    }

    var passwordVerificationResult =
        new PasswordHasher<User>()
            .VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginUserDto.Password
            );

    if (passwordVerificationResult ==
        PasswordVerificationResult.Failed)
    {
        return null;
    }

    return new Accesstoken
    {
        Token = GenerateToken(user)
    };
}

private string GenerateToken(User user)
{
    var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()
        ),

        new Claim(
            ClaimTypes.Name,
            user.Username
        )
    };

    var secretKey = configuration["AppSettings:SecretKey"];

    var issuer = configuration["AppSettings:Issuer"];

    var audience = configuration["AppSettings:Audience"];

    if (string.IsNullOrEmpty(secretKey) ||
        string.IsNullOrEmpty(issuer) ||
        string.IsNullOrEmpty(audience))
    {
        throw new InvalidOperationException(
            "JWT configuration is missing."
        );
    }

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(secretKey)
    );

    var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256
    );

    var accessToken = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler()
        .WriteToken(accessToken);
}

        public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            if (await context.Users.AnyAsync(u => u.Username == registerUserDto.Username))
            {
                throw new Exception("Username already exists.");
            }

            var user = new User
            {
                Username = registerUserDto.Username,
                CreatedAt = DateTime.UtcNow
            };
            var passwordHash = new PasswordHasher<User>().HashPassword(user, registerUserDto.Password);
            user.PasswordHash = passwordHash;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            };
        }
    }
}