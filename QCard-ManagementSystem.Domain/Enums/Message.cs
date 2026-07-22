using System.ComponentModel;

namespace QCard.ManagementSystem.Domain.Enums
{
    public enum Message
    {
        [Description("MTPSLOGIN01")]
        LoginSuccess,

        [Description("MTPSLOGIN02")]
        LoginFailed,

        [Description("MTPSLOGOUT01")]
        LogoutSuccess,

        [Description("MTPSLOGOUT02")]
        LogoutFailed,

        [Description("MTPS00016I")] // Saving data is completed successfully
        SaveSuccess,

        [Description("MTPS00130E")] // Saving Data is Failed
        SaveFailed,

        [Description("MTPS00020I")] // Data has been deleted successfully
        DeleteSuccess,

        [Description("MTPS00131E")] // Deleting Data is Failed
        DeleteFailed,

        [Description("MTPS00057I")]
        UploadSuccess,

        [Description("MTPS00058E")]
        UploadFailed,

        [Description("MTPS00115C")]
        ConfirmDelete,

        [Description("MTPS00116C")]
        ConfirmSave,

        [Description("MTPS00117C")]
        ConfirmUpload,

        [Description("MTPS00128I")]
        DownloadSuccess,

        [Description("MTPS00082E")]
        DownloadFailed,

        [Description("MTPS00118C")]
        ConfirmStop,

        [Description("MTPS00119C")]
        ConfirmCancel,

        [Description("MTPS00127I")]
        CancelSuccess,

        [Description("MTPS00133E")]
        CancelFailed,

        [Description("MTPS00014E")]
        NoDataFound,

        [Description("MTPS00134I")]
        UpdateStatusBOETDFlagSuccess,

        [Description("MTPS00135E")]
        UpdateStatusBOETDFlagFailed,

        [Description("MTPS00135I")]
        CopyDataSuccess,

        [Description("MTPS00136E")]
        CopyDataFailed,



        [Description("MTPS00003E")]
        DataShouldNotEmpty,
        [Description("MTPS00004E")]
        DataInvalidFormat,
        [Description("MTPS00092E")]
        DataNotMatch,
        [Description("MTPS00123E")]
        DataDuplicateFound,
        [Description("MTPS00124E")]
        DataNotFoundInTable,
        [Description("MTPS00125E")]
        DataTotalQtyNotEqual,
        [Description("MTPS00127E")]
        DataETDDateValidation,
        [Description("MTPS00129E")]
        DataETDDateFilledWithWorkingDay,

        [Description("MTPS00040I")]
        EmailSendSuccess,

        [Description("MTPS00053E")]
        EmailSendFailed,

        [Description("MTPS00137E")]
        ErrorIOA,
        [Description("MTPS00138I")]
        OrderSendToGTStart,
        [Description("MTPS00139I")]
        SuccessSendFTP,
        [Description("MTPS00139E")]
        FailedSendFTP,
        [Description("MTPS00140E")]
        FailedValidateConfig,
        [Description("MTPS00141I")]
        SuccessCreatedFile,
        [Description("MTPS00141E")]
        ProcessFailed,
        [Description("MTPS00142I")]
        OrderSendToGTFinished,
        [Description("MTPS00142E")]
        OrderSendToGTFailed,
    }
}
