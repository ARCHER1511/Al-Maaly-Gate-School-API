namespace Domain.Wrappers
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ServiceResult() { }

        public ServiceResult(bool success,string? message = null,T? data = default) 
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ServiceResult<T> Ok(T data,string? message = "Success") 
        {
            return new ServiceResult<T>(true,message,data);
        }

        public static ServiceResult<T> Fail(string? message)
        {
            return new ServiceResult<T>(false,message);
        }
    }
}
