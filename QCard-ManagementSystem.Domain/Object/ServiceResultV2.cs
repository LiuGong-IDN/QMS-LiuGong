using QCard.ManagementSystem.Domain.Constants;

namespace QCard.ManagementSystem.Domain.Object
{
    public class ServiceResultV2
    {
        public ServiceResultV2()
        {
            Empty();
        }
        public int? Code { get; set; }
        public string? Message { get; set; }

        public void Empty()
        {
            Code = null;
            Message = null;
        }
        public void SetMessage(string? message)
        {
            Message = message;
        }
        public void OK()
        {
            Code = 200;
            Message = MessageConstant.StatusOk;
        }
        public void OK(string? message)
        {
            OK();
            Message = message;
        }
        public void BadRequest()
        {
            Code = 400;
            Message = MessageConstant.StatusBadRequest;
        }
        public void BadRequest(string? message)
        {
            BadRequest();
            Message = message;
        }
        public void UnAuthorized()
        {
            Code = 401;
            Message = "Unauthorized";
        }
        public void Confirmation()
        {
            Code = 205;
            Message = MessageConstant.StatusConfirmation;
        }
        public void Confirmation(string? message)
        {
            Confirmation();
            Message = message;
        }
        public void MultipleChoice()
        {
            Code = 300;
            Message = MessageConstant.StatusMultipleChoice;
        }
        public void MultipleChoice(string? message)
        {
            Confirmation();
            Message = message;
        }
        public void UnAuthorized(string? message)
        {
            UnAuthorized();
            Message = message;
        }
        public void NotFound()
        {
            Code = 404;
            Message = MessageConstant.StatusNotFound;
        }
        public void NotFound(string? message)
        {
            NotFound();
            Message = message;
        }
        public void Error(string? message, string? description)
        {
            Code = 500;
            Message = message;
        }

        public void Forbidden(string? message)
        {
            Code = 403;
            Message = message;
        }
    }

    public class ObjectResultV2<T>
    {
        public ObjectResultV2()
        {
            Error = new ServiceResultV2();
        }
        public int Total { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
        public ServiceResultV2? Error { get; set; }
    }

    public class ResponseDataListV2<T>
    {
        public ResponseDataListV2()
        {
            Data = new List<T>();
            Error = new ServiceResultV2();
        }
        public int Total { get; set; }
        public bool Success { get; set; }
        public List<T> Data { get; set; }
        public ServiceResultV2? Error { get; set; }
    }
}
