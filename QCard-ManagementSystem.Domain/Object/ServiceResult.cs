using QCard.ManagementSystem.Domain.Constants;

namespace QCard.ManagementSystem.Domain.Object
{
    public class ServiceResult
    {
        public ServiceResult()
        {
            BadRequest();
        }
        public int Code { get; set; }
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public string? Description { get; set; }

        // Add New For Conversion

        //public int StatusCode { get; set; }
        //public bool Result { get; set; }
        //public string? FrameNo { get; set; }

        //public void OK(string? frameNo, string? message, bool result, int statusCode)
        //{
        //    FrameNo = frameNo;
        //    Message = message;
        //    Result = result;
        //    StatusCode = statusCode;
        //}

        public void SetMessage(string? message)
        {
            Message = message;
        }
        public void OK()
        {
            Code = 200;
            Succeeded = true;
            Message = MessageConstant.StatusOk;
            Description = string.Empty;
        }
        public void OK(string? message)
        {
            OK();
            Message = message;
        }
        public void BadRequest()
        {
            Code = 400;
            Succeeded = false;
            Message = MessageConstant.StatusBadRequest;
            Description = string.Empty;
        }
        public void BadRequest(string? message)
        {
            BadRequest();
            Message = message;
            Description = string.Empty;
        }
        public void UnAuthorized()
        {
            Code = 401;
            Succeeded = false;
            Message = "Unauthorized";
            Description = MessageConstant.StatusUnauthorized;
        }
        public void Confirmation()
        {
            Code = 205;
            Succeeded = false;
            Message = MessageConstant.StatusConfirmation;
            Description = string.Empty;
        }
        public void Confirmation(string? message)
        {
            Confirmation();
            Message = message;
            Description = string.Empty;
        }
        public void MultipleChoice()
        {
            Code = 300;
            Succeeded = false;
            Message = MessageConstant.StatusMultipleChoice;
            Description = string.Empty;
        }
        public void MultipleChoice(string? message)
        {
            Confirmation();
            Message = message;
            Description = string.Empty;
        }
        public void UnAuthorized(string? message)
        {
            UnAuthorized();
            Message = message;
            Description = string.Empty;
        }
        public void NotFound()
        {
            Code = 404;
            Succeeded = false;
            Message = MessageConstant.StatusNotFound;
        }
        public void NotFound(string? message)
        {
            NotFound();
            Message = message;
            Description = string.Empty;
        }
        public void Error(string? message, string? description)
        {
            Code = 500;
            Succeeded = false;
            Message = message;
            Description = description;
        }

        public void Forbidden(string? message)
        {
            Code = 403;
            Succeeded = false;
            Message = message;
            Description = string.Empty;
        }
    }



    public class APIResult : ServiceResult
    {
        public string? Response { get; set; }
    }
    public class ObjectResult<T>
    {
        public ObjectResult()
        {
            Status = new ServiceResult();
        }
        public T Obj { get; set; }
        public ServiceResult Status { get; set; }
        //public Guid Id { get; set; }

        public void NotFound()
        {
            Status.NotFound();
        }

        public void Error(string? message, string? description)
        {
            Status.Error(message, description);
        }
    }
    public class ListResult<T>
    {
        public ListResult()
        {
            ListObj = new List<T>();
            Status = new ServiceResult();
        }
        public List<T> ListObj { get; set; }
        public ServiceResult Status { get; set; }
    }
    public class ListPageResult<T> : ListResult<T>
    {
        public int Count { get; set; }
    }

    public class ResponseDataList<T>
    {
        public ResponseDataList()
        {
            Data = new List<T>();
            Status = new ServiceResult();
        }
        public int Total { get; set; }
        public List<T> Data { get; set; }
        public ServiceResult Status { get; set; }
    }

}
