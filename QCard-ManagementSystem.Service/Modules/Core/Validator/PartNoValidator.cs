using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class PartNoValidator : AbstractValidator<PartNoRequest>
    {
        public PartNoValidator()
        {
            RuleFor(x => x.PART_NO).NotEmpty().WithMessage("Part No. must not be empty ");
            RuleFor(x => x.PART_NAME).NotEmpty().WithMessage("Part Name must not be empty ");
            RuleFor(x => x.RETAIL_PRICE).NotEmpty().WithMessage("Retail Price must not be empty ");
        }
    }
}
