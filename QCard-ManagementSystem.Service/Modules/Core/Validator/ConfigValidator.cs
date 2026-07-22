using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class ConfigValidator : AbstractValidator<Config>
    {
        public ConfigValidator()
        {
            RuleFor(x => x.SYSTEM_ID).NotEmpty().WithMessage("System Id must not be empty ");
            RuleFor(x => x.SYSTEM_TYPE).NotEmpty().WithMessage("System Type must not be empty ");
        }
    }
}
