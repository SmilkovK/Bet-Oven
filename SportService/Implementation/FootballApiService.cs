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

        public async Task<List<TeamInfo>> GetTeams(int leagueId, int season)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}teams?league={leagueId}&season={season}");

            if (!response.IsSuccessStatusCode) return new List<TeamInfo>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballTeamsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<TeamInfo>();
        }

        public async Task<List<Fixture>> GetFixtures(int leagueId, int season)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?league={leagueId}&season={season}");

            if (!response.IsSuccessStatusCode) return new List<Fixture>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<Fixture>();
        }
        public async Task<List<AllLeagues>> GetLeagues()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}leagues");

            if (!response.IsSuccessStatusCode)
            {
                return new List<AllLeagues>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballLeaguesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<AllLeagues>();
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

            return apiResponse?.Response ?? new List<Fixture>();
        }
    }
    public class ApiFootballLeaguesResponse
    {
        public List<AllLeagues> Response { get; set; }
    }

    public class ApiFootballTeamsResponse
    {
        public List<TeamInfo> Response { get; set; }
    }

    public class ApiFootballFixturesResponse
    {
        public List<Fixture> Response { get; set; }
    }
    public class ApiFootballResponse
    {
        public List<Fixture> Response { get; set; }
    }

}
