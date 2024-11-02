using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace template_lambda;
public class NotionClient {
    private readonly HttpClient httpClient;
    private const string BaseUrl = "https://api.notion.com/v1/";
    private const string NotionVersion = "2022-06-28"; // Set this to the latest Notion version.

    public NotionClient() {
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(BaseUrl);
        string apiKey = Environment.GetEnvironmentVariable("NOTION_API_KEY") ?? throw new Exception("Missing NOTION_API_KEY env var");
        // Set the Authorization and Notion-Version headers
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionVersion);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint) {
        var response = await httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T content) {
        var jsonContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(endpoint, jsonContent);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> PatchAsync<T>(string endpoint, T content) {
        var jsonContent = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint) { Content = jsonContent };
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint) {
        var response = await httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<List<ServiceEntry>> GetServicesInNext12Days(string databaseId, string dateProperty) {
        var now = DateTime.UtcNow;
        var twelveDaysFromNow = now.AddDays(12);

        var requestPayload = new {
            filter = new {
                and = new List<object> {
                new {
                    property = dateProperty,
                    date = new {
                        on_or_after = now.ToString("yyyy-MM-dd"),
                    }
                },
                new {
                    property = dateProperty,
                    date = new {
                        on_or_before = twelveDaysFromNow.ToString("yyyy-MM-dd"),
                    }
                },
                new {
                    property = dateProperty,
                    date = new {
                        is_not_empty = true,
                    }
                }
            }
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"databases/{databaseId}/query", jsonContent);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var services = new List<ServiceEntry>();

        foreach (var result in jsonDocument.RootElement.GetProperty("results").EnumerateArray()) {
            var properties = result.GetProperty("properties");
            var musicList = properties.GetProperty("Music List");

            var formattedMusicListHtml = string.Join("", musicList.GetProperty("rich_text").EnumerateArray()
                .Select(rt => {
                    var text = rt.GetProperty("text").GetProperty("content").GetString() ?? string.Empty;
                    var annotations = rt.GetProperty("annotations");

                    if (annotations.GetProperty("bold").GetBoolean()) text = $"<b>{text}</b>";
                    if (annotations.GetProperty("italic").GetBoolean()) text = $"<i>{text}</i>";

                    return text;
                }));

            var musicListStringHtml = string.Join("<br>", formattedMusicListHtml.Split("\n").Select(line => line.Trim()).ToArray());
            var callTime = properties.GetProperty("Service Time").GetProperty("date").GetProperty("start").GetString() ?? string.Empty;

            services.Add(new ServiceEntry(musicListStringHtml, callTime));
        }

        return services;
    }

}
