using Store.BLL.Entities;
using FluentValidation;

namespace Store.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).Length(0, 100);
        RuleFor(x => x.Description).Length(0, 1000);
        RuleFor(x => x.Count).GreaterThanOrEqualTo(0);
    }
}
