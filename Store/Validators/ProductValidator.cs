using Store.BLL.Entities;
using FluentValidation;
using Store.Models;

namespace Store.Validators;

public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).Length(0, 100);
        RuleFor(x => x.Description).Length(0, 1000);
        RuleFor(x => x.Count).GreaterThanOrEqualTo(0);
    }
}
