using System.Text.Json.Serialization;

namespace PsGen.Mobile.Models;

public class ApiResponse
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("result")]
    public T Result { get; set; } = default!;
}

public class RecordResponse<T> : ApiResponse<T>
{
}

public class RecordsResponse<T> : ApiResponse<List<T>>
{
}

public class DeletedResponse : ApiResponse<bool>
{
}
