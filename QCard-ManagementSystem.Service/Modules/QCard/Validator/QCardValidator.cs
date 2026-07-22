using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class QCardValidator : AbstractValidator<QCardRequest>
    {
        public QCardValidator()
        {
            //RuleFor(x => x.USERNAME)
            //    .NotEmpty().WithMessage("Username must not be empty ")
            //    .Must((model, username) => string.IsNullOrEmpty(BeUniqueUsername(model))).WithMessage(model => BeUniqueUsername(model));
            //RuleFor(x => x.REG_NO)
            //    .NotEmpty().WithMessage("REG_NO must not be empty ")
            //    .Must((model, regNo) => string.IsNullOrEmpty(BeUniqueRegNo(model))).WithMessage(model => BeUniqueRegNo(model));
            RuleFor(x => x.CUSTOMER_ID).NotEmpty().WithMessage("Customer must not be empty ");
            RuleFor(x => x.CARD_NO)
            .NotEmpty()
                .WithMessage("Card No. must not be empty")
            .Length(16)
                .WithMessage("Card No. must be exactly 16 characters");
            RuleFor(x => x.VALID_TO_MONTH).NotEmpty().WithMessage("Valid Month must not be empty ");
            RuleFor(x => x.VALID_TO_YEAR).NotEmpty().WithMessage("Valid Year must not be empty ");
        }
    }
}
