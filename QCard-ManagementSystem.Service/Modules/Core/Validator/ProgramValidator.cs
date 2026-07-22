using FluentValidation;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Domain.Modules.Core.Request;

namespace QCard.ManagementSystem.Service.Modules.Core.Validator
{
    public class ProgramValidator : AbstractValidator<ProgramRequest>
    {
        public ProgramValidator()
        {
            RuleFor(x => x.PROGRAM_CODE).NotEmpty().WithMessage("Program Category must not be empty ");
            RuleFor(x => x.PROGRAM_NAME).NotEmpty().WithMessage("Program Name must not be empty ");
            RuleFor(x => x.ASSET_TYPE_ID).NotEmpty().WithMessage("Product Group must not be empty ");
            RuleFor(x => x.MODEL_ID).NotEmpty().WithMessage("Product Model must not be empty ");
            RuleFor(x => x.PRIORITY).NotEmpty().WithMessage("Priority must not be empty ");

            RuleForEach(x => x.DETAILS)
                .ChildRules(detail =>
                {
                    detail.RuleFor(d => d.PART_NO)
                        .NotEmpty().WithMessage("Part No must not be empty");

                    detail.RuleFor(d => d.QTY)
                        .GreaterThan(0).WithMessage("Quantity must be greater than zero");
                });
        }
        //public static string BeUniquePhoneNumber(ModelRequest model)
        //{
        //    string res = string.Empty;

        //    using (DbHelper db = new())
        //    {
        //        List<UserView> userList = db.UserRepository.ListModel("GetList", "", "", "");
        //        var result = userList.Where(x => x.PHONE_NUMBER.Equals(model.PHONE_NUMBER, StringComparison.CurrentCultureIgnoreCase)).ToList();
        //        if (result.Count > 0)
        //        {
        //            res = "Phone Number is already exist ";
        //        }
        //    }
        //    return res;
        //}
        //public static string BeUniqueEmail(UserRequest model)
        //{
        //    string res = string.Empty;

        //    using (DbHelper db = new())
        //    {
        //        List<UserView> userList = db.UserRepository.ListUser("GetList", "", "", "");
        //        var result = userList.Where(x => x.EMAIL.Equals(model.EMAIL, StringComparison.CurrentCultureIgnoreCase)).ToList();
        //        if (result.Count > 0)
        //        {
        //            res = "Email is already exist ";
        //        }
        //    }
        //    return res;
        //}
    }
}
