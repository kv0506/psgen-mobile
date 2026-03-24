using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PsGen.Mobile.Models;

namespace PsGen.Mobile.Services;

public class ApiService
{
    private const string BaseUrl = "https://psgen-api.azurewebsites.net/api/";
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<T?> GetAsync<T>(string path, string? authToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        AddAuthHeader(request, authToken);

        var response = await _httpClient.SendAsync(request);
        return await DeserializeResponse<T>(response);
    }

    public async Task<T?> PutAsync<T>(string path, object body, string? authToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, path)
        {
            Content = CreateJsonContent(body)
        };
        AddAuthHeader(request, authToken);

        var response = await _httpClient.SendAsync(request);
        return await DeserializeResponse<T>(response);
    }

    public async Task<T?> PostAsync<T>(string path, object body, string? authToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = CreateJsonContent(body)
        };
        AddAuthHeader(request, authToken);

        var response = await _httpClient.SendAsync(request);
        return await DeserializeResponse<T>(response);
    }

    public async Task<T?> DeleteAsync<T>(string path, object body, string? authToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, path)
        {
            Content = CreateJsonContent(body)
        };
        AddAuthHeader(request, authToken);

        var response = await _httpClient.SendAsync(request);
        return await DeserializeResponse<T>(response);
    }

    private static void AddAuthHeader(HttpRequestMessage request, string? authToken)
    {
        if (!string.IsNullOrWhiteSpace(authToken))
            request.Headers.Add("AuthToken", authToken);
    }

    private StringContent CreateJsonContent(object body)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            // Return a typed failure so callers see IsSuccess == false without an exception.
            if (typeof(ApiResponse).IsAssignableFrom(typeof(T)))
            {
                var error = Activator.CreateInstance<T>();
                if (error is ApiResponse apiError)
                {
                    apiError.IsSuccess = false;
                    apiError.Message = "Session expired. Please log in again.";
                }
                return error;
            }

            return default;
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }
}
