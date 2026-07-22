using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(x => x.FIRST_NAME).NotEmpty().WithMessage("First Name must not be empty ");
            RuleFor(x => x.LAST_NAME).NotEmpty().WithMessage("Last Name must not be empty ");
            RuleFor(x => x.PHONE_NUMBER).NotEmpty().WithMessage("Mobile No. must not be empty ");
            RuleFor(x => x.EMAIL).NotEmpty().WithMessage("Email must not be empty ");
            RuleFor(x => x.ROLE).NotEmpty().WithMessage("Role must not be empty ");
            RuleFor(x => x.QCARD)
                .NotEmpty()
                .WithMessage("Q-Card No. must not be empty")
                .When(x => x.ROLE?.Contains("23") == true);
            RuleFor(x => x.PASSWORD)
                .NotEmpty()
                .WithMessage("Password must not be empty ")
                .When(x => x.ROLE?.Contains("23") == true);
            RuleFor(x => x.PASSWORD_CONFIRM)
                .NotEmpty()
                .WithMessage("Password confirm must not be empty")
                .Equal(x => x.PASSWORD).WithMessage("Password confirmation does not match the password")
                .When(x => x.ROLE?.Contains("23") == true);
        }
    }
}