using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class SerialNoValidator : AbstractValidator<MachineRequest>
    {
        public SerialNoValidator()
        {
            RuleFor(x => x.CUSTOMER_ID).NotEmpty().WithMessage("Customer must not be empty ");
            RuleFor(x => x.SERIAL_NO).NotEmpty().WithMessage("Serial No. must not be empty ");
            RuleFor(x => x.PJB_NO).NotEmpty().WithMessage("PJB No. must not be empty ");
            RuleFor(x => x.EVIDENCE_PJB).NotEmpty().WithMessage("Evidence must not be empty ");
            RuleFor(x => x.ASSET_TYPE_ID).NotEmpty().WithMessage("Product Group must not be empty ");
            RuleFor(x => x.MODEL_ID).NotEmpty().WithMessage("Product Model must not be empty ");
            RuleFor(x => x.PURCHASE_DATE).NotEmpty().WithMessage("Purchase Date must not be empty ");
            RuleFor(x => x.SERVICE_PARTY_ID).NotEmpty().WithMessage("Service Party must not be empty ");
            RuleFor(x => x.BRANCH).NotEmpty().WithMessage("Branch must not be empty ");

            RuleForEach(x => x.DETAILS)
                .ChildRules(detail =>
                {
                    detail.RuleFor(d => d.PROGRAM_ID)
                        .GreaterThan(0).WithMessage("Program ID must be valid");

                    detail.RuleFor(d => d.PACKAGE_QTY)
                        .GreaterThan(0).WithMessage("Package Qty must be greater than zero");

                    detail.RuleFor(d => d.CURRENT_PACKAGE_QTY)
                        .GreaterThanOrEqualTo(0).WithMessage("Current Package Qty cannot be negative");
                });
            RuleFor(x => x)
            .Must(x => x.IS_PROGRAM || x.IS_VOUCHER)
            .WithMessage("Please select at least one Category Program")
            .OverridePropertyName("PROGRAM_CATEGORY");
        }
    }
}
