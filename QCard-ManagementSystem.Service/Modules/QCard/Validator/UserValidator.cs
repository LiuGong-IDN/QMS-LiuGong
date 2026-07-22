using FluentValidation;
using QCard.ManagementSystem.Domain.Modules.QCard.Model;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class TopupSubmitValidator : AbstractValidator<TopupValidatorModel>
    {
        public TopupSubmitValidator()
        {
            RuleFor(x => x.APPROVAL_LEVEL_ID).NotEmpty().WithMessage("Approval Id must not be empty ");
        }
    }
}