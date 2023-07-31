using FluentValidation;
namespace aoiRouting.Shared.Models.Validators
{
    public class PinValidator : AbstractValidator<Pin>
    {
        public PinValidator()
        {
            RuleFor(x => x.ID).NotNull().NotEmpty();
            RuleFor(x => x.AOIID).NotNull().NotEmpty();
            RuleFor(x => x.UserID).NotNull().NotEmpty();
            RuleFor(x => x.PointID).InclusiveBetween(0,36).NotNull().NotEmpty();
			RuleFor(x => x.Lat).InclusiveBetween(-90, 90).NotNull();
            RuleFor(x => x.Lon).InclusiveBetween(-180, 180).NotNull();
            RuleFor(x => x.Notes).MaximumLength(8191);
            RuleFor(x => x.Created).NotNull().NotEmpty();
        }
    }
}