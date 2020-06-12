using FluentValidation;

namespace Swashbuckle.Issue.Example.Repository
{
    public class ValueModelValidator : AbstractValidator<ValueModel>
    {
        public ValueModelValidator()
        {
            //FluentValidation included rules will appear in Swagger property documentation.
            RuleFor(x => x.Value)
                .NotNull()
                .EmailAddress()
                .MinimumLength(6)
                .MaximumLength(10);
        }
    }
}
