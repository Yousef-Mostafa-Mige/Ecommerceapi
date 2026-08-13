using Ecommerceapi.Dtos.token;
using Ecommerceapi.Dtos.UserDtos;

namespace Ecommerceapi.services.UserServices
{
    public interface IUserServices
    {
        Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<Accesstoken?> LoginUserAsync(LoginUserDto loginUserDto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<Accesstoken?> RefreshTokenAsync(string refreshToken);
    }
}