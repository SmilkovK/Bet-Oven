using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using SportDomain.models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SportService.Implementation
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v3.football.api-sports.io/";
        private const string ApiKey = "c26581fbcde99337cec7d73133eaad2a";
        private const string ApiKey2 = "6aa5657eb2e9e42c02893f4617cb4a71";
        private const string ApiKey3 = "3e5d931483d36220980032117ee6e6dd";


        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", ApiKey3);
        }
        public async Task<List<Fixture>> GetFixtures(int leagueId, int season)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?league={leagueId}&season={season}");

            if (!response.IsSuccessStatusCode) return new List<Fixture>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var fixtures = apiResponse?.Response ?? new List<Fixture>();

            foreach (var fixture in fixtures)
            {
                if (fixture.Timestamp > 0) 
                {
                    fixture.Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;
                }
            }

            var tasks = fixtures.Select(async fixture =>
            {
                fixture.Odds = await GetOdds(fixture.Id);
            });

            await Task.WhenAll(tasks);

            return fixtures;
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
                    Seasons = l.Seasons.Where(s => s.Current).ToList()
                })
                .Where(l => l.Seasons.Any())
                .ToList();

            return formattedLeagues;
        }

        public async Task<List<Fixture>> GetTodaysFixtures()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?date={today}");

            if (!response.IsSuccessStatusCode) return new List<Fixture>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Response ?? new List<Fixture>();
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

            var fixtures = apiResponse?.Response ?? new List<Fixture>();

            foreach (var fixture in fixtures)
            {
                if (fixture.Timestamp > 0) 
                {
                    fixture.Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;
                }
            }

            return fixtures;
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
        public async Task<Odds> GetOdds(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}odds?fixture={fixtureId}&bookmaker=8");

            if (!response.IsSuccessStatusCode)
            {
                return new Odds(); 
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballOddsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var oddsData = apiResponse?.Response?.FirstOrDefault();

            if (oddsData == null)
            {
                return new Odds(); 
            }

            var odds = new Odds
            {
                Bookmakers = oddsData.Bookmakers
                    .Select(b => new Bookmaker
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Bets = b.Bets
                            .Select(bet => new Bet
                            {
                                Name = bet.Name,
                                Values = bet.Values
                                    .Select(v => new BetValue
                                    {
                                        Value = v.Value,
                                        Odd = v.Odd
                                    }).ToList()
                            }).ToList()
                    }).ToList()
            };

            return odds;
        }
    }
    public class ApiFootballOddsResponse
    {
        public List<OddsResponse> Response { get; set; }
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

