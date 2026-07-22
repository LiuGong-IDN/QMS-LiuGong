using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class CustomerValidator : AbstractValidator<CustomerRequest>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.COMPANY_CODE).NotEmpty().WithMessage("Company Code must not be empty ");
            RuleFor(x => x.COMPANY_NAME).NotEmpty().WithMessage("Company Name must not be empty ");
            RuleFor(x => x.ADDRESS).NotEmpty().WithMessage("Address must not be empty ");
        }

    }
}
