using FluentValidation;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class UpdateClaimProcessValidator : AbstractValidator<ClaimProcessRequest>
    {
        public UpdateClaimProcessValidator()
        {
            RuleFor(x => x.DnNumber)
                .NotEmpty().WithMessage("Delivery Note Number must not be empty")
                .When(x => x.ActionName == "Approve" || x.ActionName == "Pending");

            RuleFor(x => x.PgiNo)
                .NotEmpty().WithMessage("PGI Number must not be empty")
                .When(x => x.ActionName == "Mark Ready");

            RuleFor(x => x.SpbNo)
                .NotEmpty().WithMessage("SPB Number must not be empty")
                .When(x => x.ActionName == "Pickup");
            RuleFor(x => x.ResiNo)
                .NotEmpty().WithMessage("Resi Number must not be empty")
                .When(x => x.ActionName == "Pickup");

            RuleFor(x => x.NOTES)
                .NotEmpty().WithMessage("Shipping Notes must not be empty")
                .When(x => x.ShippingAgent == 7 && x.ActionName == "Pickup");


            RuleFor(x => x.ReasonReject)
                .NotEmpty().WithMessage("Reject reason must not be empty")
                .When(x => x.ActionName == "Reject");
        }
    }
}
