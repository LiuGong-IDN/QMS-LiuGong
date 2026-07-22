using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class ModelValidator : AbstractValidator<ModelRequest>
    {
        public ModelValidator()
        {
            RuleFor(x => x.MODEL_CODE).NotEmpty().WithMessage("Product Model No. must not be empty ");
            RuleFor(x => x.MODEL_NAME).NotEmpty().WithMessage("Product Model Name must not be empty ");
            RuleFor(x => x.ASSET_TYPE_ID).NotEmpty().WithMessage("Product Group must not be empty ");
        }
    }
}
