using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SportDomain.models;

namespace SportService.Implementation
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v3.football.api-sports.io/";
        private const string ApiKey = "6aa5657eb2e9e42c02893f4617cb4a71";

        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
        }

        public async Task<List<Fixture>> GetLiveMatches()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?live=all");

            if (!response.IsSuccessStatusCode)
            {
                return new List<Fixture>(); 
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<Fixture>(); // Ensure it's not null
        }
    }

    public class ApiFootballResponse
    {
        public List<Fixture> Response { get; set; }
    }
}
