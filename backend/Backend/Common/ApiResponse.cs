namespace Backend.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T? data = default, string? message = null)
            => new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Request was successful.",
                Data = data
            };

        public static ApiResponse<T> FailureResponse(string message)
            => new ApiResponse<T>
            {
                Success = false,
                Message = message
            };

        // Thất bại kèm danh sách lỗi chi tiết (dùng cho validation)
        public static ApiResponse<T> FailureResponse(string message, IEnumerable<string> errors)
            => new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                Data = default
            };

        // Thất bại chỉ có danh sách lỗi (message mặc định)
        public static ApiResponse<T> FailureResponse(IEnumerable<string> errors)
            => FailureResponse("Input data not valid", errors);
    }
}
