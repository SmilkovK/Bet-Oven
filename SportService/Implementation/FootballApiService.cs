using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using SportDomain.models;

namespace SportService.Implementation
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v3.football.api-sports.io/";
        private const string ApiKey = "6aa5657eb2e9e42c02893f4617cb4a71";
        private const string ApiKey2 = "644cf906aee148fd974fe3ee08078e4a";

        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", ApiKey2);
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

            var leagues = apiResponse?.Response ?? new List<AllLeagues>();

            // Extract league info and current season
            var formattedLeagues = leagues
                .Select(l => new AllLeagues
                {
                    League = new LeagueInfo
                    {
                        Id = l.League.Id,
                        Name = l.League.Name,
                        Logo = l.League.Logo,
                        Type = l.League.Type
                    },
                    Country = l.Country,
                    Seasons = l.Seasons.Where(s => s.Current).ToList() // Only keep current season
                })
                .Where(l => l.Seasons.Any()) // Ensure we only return leagues with a current season
                .ToList();

            return formattedLeagues;
        }

        public async Task<List<Matches>> GetTodaysFixtures()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd"); // Get today's date in YYYY-MM-DD format
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?date={today}");

            if (!response.IsSuccessStatusCode) return new List<Matches>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballMatchesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<Matches>();
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
        public async Task<List<Standing>> GetStandings(int leagueId, int season)
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}standings?league={leagueId}&season={season}");

            if (!response.IsSuccessStatusCode) return new List<Standing>();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiFootballStandingsResponse>(json, options);

            return apiResponse?.Response
                ?.FirstOrDefault()
                ?.League
                ?.Standings
                ?.FirstOrDefault()
                ?? new List<Standing>();
        }

    }
    public class ApiFootballStandingsResponse
    {
        public List<LeagueResponse> Response { get; set; }
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
    public class ApiFootballMatchesResponse
    {
        public List<Matches> Response { get; set; }
    }
}
