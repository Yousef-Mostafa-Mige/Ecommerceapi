using FluentValidation;

namespace Ecommerceapi.Dtos.UserDtos;

public class RegisterUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string role {get;set;}
}
public class Uservalidationsign_in : AbstractValidator<RegisterUserDto>
    {
        public Uservalidationsign_in()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password ).NotEmpty().WithMessage("Password is required.");
        }
    }