using FluentValidation;
using QCard.ManagementSystem.Domain.Modules.MyMachines.Model;

namespace QCard.ManagementSystem.Service.Modules.MyMachince.Validator
{
    public class ClaimSubmitValidator : AbstractValidator<ClaimValidatorModel>
    {
        public ClaimSubmitValidator()
        {
            RuleFor(x => x.PDC_ID).NotEmpty().WithMessage("PDC must not be empty ");
        }
    }
}