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
using System.Security.Cryptography;
using ECommerceApi.Middleware;
namespace Ecommerceapi.services.UserServices
{
    public class UserServices(AppDBContext context, IConfiguration configuration) : IUserServices
    {
        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                throw new lNullReferenceException($"User with ID {id} not found.");
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
                throw new lNullReferenceException(nameof(loginUserDto));
            }

            var user = await context.Users
                .FirstOrDefaultAsync(
                    u => u.Username == loginUserDto.Username);

            if (user is null)
            {
                throw new lNullReferenceException($"User with username {loginUserDto.Username} not found.");
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
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return await GenerateTokenAsync(user);
        }
        private async Task<Accesstoken?> GenerateTokenAsync(User user)
        {
            return new Accesstoken
            {
                refreshToken = await GenerateAndSaveRefreshTokenAsync(user),
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
            ClaimTypes.Role,
            user.Role
        ),
        new Claim(
            ClaimTypes.Name,
            user.Username
        )
    };


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["AppSettings:SecretKey"]!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var accessToken = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
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
                throw new BadRequestException("Username already exists.");
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
        private string GenerateToken()
        {
            var rondomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rondomNumber);
            return Convert.ToBase64String(rondomNumber);
        }
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        public async Task<Accesstoken?> RefreshTokenAsync(string refreshToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }


            return await GenerateTokenAsync(user);
        }

    }
}