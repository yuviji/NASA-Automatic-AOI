using FluentValidation;

namespace aoiRouting.Shared.Models.Validators
{
    public class AOIValidator : AbstractValidator<AOI>
    {
        public AOIValidator()
        {
            RuleFor(x => x.ID).NotNull().NotEmpty();
            RuleFor(x => x.UserID).NotNull().NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(8191);
			RuleFor(x => x.Created).NotNull().NotEmpty();
		}
	}
}