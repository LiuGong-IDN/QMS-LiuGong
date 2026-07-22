using FluentValidation;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class ClaimProcessValidator : AbstractValidator<ClaimProcessUpdatePartialRequest>
    {
        public ClaimProcessValidator(ClaimProcessUpdatePartialRequest request)
        {
            if(request.Type.ToLower() == "pgi")
            {
                RuleFor(x => x.PgiNo).NotEmpty().WithMessage("PGI must not be empty ");
            }
            else
            {
                RuleFor(x => x.ShippingAgent).NotEmpty().WithMessage("Please select Shipping Agent");
                RuleFor(x => x.SpbNo).NotEmpty().WithMessage("SPB must not be empty ");
                RuleFor(x => x.ResiNo).NotEmpty().WithMessage("Resi must not be empty ");
                RuleFor(x => x.NOTES)
                .NotEmpty().WithMessage("Shipping Notes must not be empty")
                .When(x => x.ShippingAgent == 7);
            }
        }
    }
}
