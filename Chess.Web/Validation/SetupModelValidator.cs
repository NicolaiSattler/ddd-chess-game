using Chess.Web.Model;
using FluentValidation;

namespace Chess.Web.Validation;

public class SetupModelValidator: AbstractValidator<SetupModel>
{
    public SetupModelValidator()
    {
        RuleFor(m => m.MemberOne).NotEmpty();
        RuleFor(m => m.MemberTwo).NotEmpty();
        RuleFor(m => m.MaxTurnTime).GreaterThan(0);
        RuleFor(m => m.MaxTurnTime).LessThan(60);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<SetupModel>.CreateWithOptions((SetupModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}