using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class TopupValidator : AbstractValidator<TopupRequest>
    {
        public TopupValidator()
        {
            //RuleFor(x => x.USERNAME)
            //    .NotEmpty().WithMessage("Username must not be empty ")
            //    .Must((model, username) => string.IsNullOrEmpty(BeUniqueUsername(model))).WithMessage(model => BeUniqueUsername(model));
            //RuleFor(x => x.REG_NO)
            //    .NotEmpty().WithMessage("REG_NO must not be empty ")
            //    .Must((model, regNo) => string.IsNullOrEmpty(BeUniqueRegNo(model))).WithMessage(model => BeUniqueRegNo(model));
            RuleFor(x => x.CARD_ID).NotEmpty().WithMessage("Card ID not be empty ");
            RuleForEach(x => x.DETAILS)
                .ChildRules(detail =>
                {
                    detail.RuleFor(d => d.SERIAL_NO)
                        .NotEmpty().WithMessage("Serial No must not be empty");
                });
        }
    }
}
