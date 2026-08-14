using FluentValidation;

namespace Ecommerceapi.Dtos.UserDtos
{

    public class LoginUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class Uservalidationlogin : AbstractValidator<LoginUserDto>
    {
        public Uservalidationlogin()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}