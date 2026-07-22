using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.QCard.Request;
using QCard.ManagementSystem.Domain.Modules.QCard.ViewModel;

namespace QCard.ManagementSystem.Service.Modules.QCard.Validator
{
    public class ApprovalValidator : AbstractValidator<ApprovalRequest>
    {
        public ApprovalValidator()
        {
            RuleFor(x => x.DOCUMENT_APPROVAL_IDS)
            .NotNull().WithMessage("Document Approval must not be empty")
            .Must(list => list != null && list.Any()).WithMessage("Document Approval list must contain at least one item");
            RuleFor(x => x.FORM_TYPE_ID).NotEmpty().WithMessage("Form Type must not be empty ");
            RuleFor(x => x.ACTION_NAME).NotEmpty().WithMessage("Action Name must not be empty ");
        }
    }
}
