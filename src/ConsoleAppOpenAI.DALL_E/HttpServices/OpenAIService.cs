using ConsoleAppOpenAI.DALL_E.Models.Images;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConsoleAppOpenAI.DALL_E.Services
{
    public class OpenAIService
    {
        readonly HttpClient _httpClient;

        readonly IConfiguration _configuration;

        public OpenAIService(IConfiguration configuration, string baseUrl = "https://api.openai.com")
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<GenerateImageResponse> GenerateImages(GenerateImageRequest prompt, CancellationToken cancellation = default)
        {
            using var rq = new HttpRequestMessage(HttpMethod.Post, "/v1/images/generations");

            var jsonRequest = JsonSerializer.Serialize<GenerateImageRequest>(prompt, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            rq.Content = new StringContent(jsonRequest);
            rq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var apiKey = _configuration["OpenAi:ApiKey"];
            rq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var subscriptionId = _configuration["OpenAi:SubscriptionId"];
            rq.Headers.TryAddWithoutValidation("OpenAI-Organization", subscriptionId);

            //var response = await _httpClient.PostAsJsonAsync("", prompt);
            var response = await _httpClient.SendAsync(rq, cancellation);

            response.EnsureSuccessStatusCode();

            var content = response.Content;

            var jsonResponse = await content.ReadFromJsonAsync<GenerateImageResponse>();

            return jsonResponse;
        }

        internal async Task<byte[]> DownloadImage(string url)
        {
            var buffer = await _httpClient.GetByteArrayAsync(url);

            return buffer;
        }
    }
}
