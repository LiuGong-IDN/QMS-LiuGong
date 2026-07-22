using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {
            RuleFor(x => x.ROLE_NAME)
                .NotEmpty().WithMessage("Username must not be empty ")
                .Must((model, username) => string.IsNullOrEmpty(BeUnique(model))).WithMessage(model => BeUnique(model));
            //RuleFor(x => x.REG_NO)
            //    .NotEmpty().WithMessage("REG_NO must not be empty ")
            //    .Must((model, regNo) => string.IsNullOrEmpty(BeUniqueRegNo(model))).WithMessage(model => BeUniqueRegNo(model));
            RuleFor(x => x.ROLE_NAME).NotEmpty().WithMessage("First Name must not be empty ");
        }
        public static string BeUnique(Role model)
        {
            string res = string.Empty;

            using (DbHelper db = new())
            {
                List<Role> userList = db.DataSourceRepository.GetRoleList("");
                var result = userList.Where(x => x.ROLE_NAME.Equals(model.ROLE_NAME, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (result.Count > 0)
                {
                    res = "Role Name is already exist ";
                }
            }
            return res;
        }
    }
}
