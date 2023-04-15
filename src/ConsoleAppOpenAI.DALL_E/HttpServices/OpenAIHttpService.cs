using ConsoleAppOpenAI.DALL_E.HttpServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConsoleAppOpenAI.DALL_E.Services;

public class OpenAIHttpService : IOpenAIProxy
{
    readonly HttpClient _httpClient;

    readonly string _subscriptionId;

    readonly string _apiKey;

    public OpenAIHttpService(IConfiguration configuration)
    {
        var openApiUrl = configuration["OpenAi:Url"] ?? throw new ArgumentException(nameof(configuration));
        _httpClient = new HttpClient { BaseAddress = new Uri(openApiUrl) };

        _subscriptionId = configuration["OpenAi:SubscriptionId"];
        _apiKey = configuration["OpenAi:ApiKey"];
    }

    public async Task<GenerateImageResponse> GenerateImages(GenerateImageRequest prompt, CancellationToken cancellation = default)
    {
        using var rq = new HttpRequestMessage(HttpMethod.Post, "/v1/images/generations");

        var jsonRequest = JsonSerializer.Serialize(prompt, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        rq.Content = new StringContent(jsonRequest);
        rq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var apiKey = _apiKey;
        rq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var subscriptionId = _subscriptionId;
        rq.Headers.TryAddWithoutValidation("OpenAI-Organization", subscriptionId);

        var response = await _httpClient.SendAsync(rq, HttpCompletionOption.ResponseHeadersRead, cancellation);

        response.EnsureSuccessStatusCode();

        var content = response.Content;

        var jsonResponse = await content.ReadFromJsonAsync<GenerateImageResponse>(cancellationToken: cancellation);

        return jsonResponse;
    }

    public async Task<byte[]> DownloadImage(string url)
    {
        var buffer = await _httpClient.GetByteArrayAsync(url);

        return buffer;
    }
}
