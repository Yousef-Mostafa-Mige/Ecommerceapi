using FluentValidation;

namespace Ecommerceapi.Dtos.CategoryDtos;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}
public class CategoryValidationCreateCategoryDto : AbstractValidator<CreateCategoryDto>
{
    public CategoryValidationCreateCategoryDto()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
    }
}