namespace PartnersHub.InfraBase.Apis.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse {
    public int HttpCode { get; set; }
    public string Status { get; set; }
    public object? Data { get; set; }
    public object? Error { get; set; }

    public ApiResponse(int httpCode, string status, object? data, object? error) {
        HttpCode = httpCode;
        Status = status;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// Creates a success response with data
    /// </summary>
    public static ApiResponse Success(object? data = null, int httpCode = 200) {
        return new ApiResponse(httpCode, "Success", data, null);
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse Failure(string errorMessage, int httpCode = 400) {
        return new ApiResponse(httpCode, "Error", null, new { Message = errorMessage });
    }

    /// <summary>
    /// Creates an error response with detailed error object
    /// </summary>
    public static ApiResponse Failure(object errorObject, int httpCode = 400) {
        return new ApiResponse(httpCode, "Error", null, errorObject);
    }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T> {
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse() {
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) {
        return new ApiResponse<T> {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully",
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(string error) {
        return new ApiResponse<T> {
            Success = false,
            Errors = new List<string> { error },
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(List<string> errors) {
        return new ApiResponse<T> {
            Success = false,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}