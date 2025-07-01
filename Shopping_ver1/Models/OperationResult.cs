namespace Shopping_ver1.Models
{
    public class OperationResult
    {
        // Kết quả
        public bool Success { get; set; }
        // Thông báo
        public string Message { get; set; }

        public OperationResult() { }
        public OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

    }
}
