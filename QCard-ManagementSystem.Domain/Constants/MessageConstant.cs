namespace QCard.ManagementSystem.Domain.Constants
{
	public class MessageConstant
	{
		public const string InputRequired = "{PropertyName} must be filled";
		public const string SelectRequired = "Please Select {PropertyName}";
		public const string Unique = "{PropertyName} has already been taken";
		public const string ValidEmail = "{PropertyName} is not a valid email";
		public const string General = "Please fill with the valid value";

		public const string SuccessSave = "{0} has been saved";
		public const string FailedSave = "{0} failed to be save";
		public const string FailedValidate = "{0} failed to be validate";

		public const string StatusOk = "All OK";
		public const string StatusBadRequest = "Bad Request";
		public const string StatusUnauthorized = "Unauthorized";
		public const string StatusConfirmation = "Confirmation";
		public const string StatusMultipleChoice = "MultipleChoice";
		public const string StatusNotFound = "Data Not Found";
	}
}
