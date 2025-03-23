using v8proj.Core.Enums;

namespace v8proj.Web.Model.DTO
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public OperationStatus Status { get; set; }
        public string Message { get; set; }

        public BaseResponse(T data, OperationStatus status = OperationStatus.Success, string message = "")
        {
            Data = data;
            Status = status;
            Message = message;
        }
    }
}