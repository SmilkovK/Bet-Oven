using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using SportDomain.models;
using JsonSerializer = System.Text.Json.JsonSerializer;
using SportDomain.DTO;

namespace SportService.Implementation
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v3.football.api-sports.io/";
        private const string ApiKey = "ba411f3495e9bf5e6032a25bd5a86a50";


        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
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

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<SportDomain.DTO.ApiFootballResponse>(json, options);

            var fixtures = new List<Fixture>();

            if (apiResponse?.Response != null)
            {
                foreach (var apiWrapper in apiResponse.Response)
                {
                    var domainFixture = new Fixture
                    {
                        Id = apiWrapper.Fixture.Id,
                        Timestamp = apiWrapper.Fixture.Timestamp,
                        Date = apiWrapper.Fixture.Date,
                        Status = new MatchStatus
                        {
                            Long = apiWrapper.Fixture.Status?.Long,
                            Short = apiWrapper.Fixture.Status?.Short,
                            Elapsed = apiWrapper.Fixture.Status?.Elapsed
                        },
                        League = apiWrapper.League,
                        Teams = apiWrapper.Teams,
                        Goals = apiWrapper.Goals,
                        Odds = new OddsInfo()
                    };

                    fixtures.Add(domainFixture);
                }
            }

            return fixtures;
        }
        public async Task<Fixture> GetFixtureById(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?id={fixtureId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return apiResponse?.Response?.FirstOrDefault();
        }
        /*public async Task<Fixture> GetFixtureById(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?id={fixtureId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiFootballResponse>(json, options);
            var apiWrapper = apiResponse?.Response?.FirstOrDefault();

            if (apiWrapper == null) return null;

            return new Fixture
            {
                Id = apiWrapper.Fixture.Id,
                Timestamp = apiWrapper.Fixture.Timestamp,
                Date = apiWrapper.Fixture.Date,
                Status = new MatchStatus
                {
                    Long = apiWrapper.Fixture.Status?.Long,
                    Short = apiWrapper.Fixture.Status?.Short,
                    Elapsed = apiWrapper.Fixture.Status?.Elapsed
                },
                League = apiWrapper.League,
                Teams = new Teams
                {
                    Home = new TeamInfo
                    {
                        Id = apiWrapper.Teams.Home.Id,
                        Name = apiWrapper.Teams.Home.Name,
                        Logo = apiWrapper.Teams.Home.Logo
                    },
                    Away = new TeamInfo
                    {
                        Id = apiWrapper.Teams.Away.Id,
                        Name = apiWrapper.Teams.Away.Name,
                        Logo = apiWrapper.Teams.Away.Logo
                    }
                },
                Goals = apiWrapper.Goals,
                Odds = await GetOdds(apiWrapper.Fixture.Id)
            };
        }*/
        public async Task<SportDomain.DTO.ApiStatsResponse> GetFixtureStatistics(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures/statistics?fixture={fixtureId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var statsResponse = JsonSerializer.Deserialize<SportDomain.DTO.ApiStatsResponse>(json, options);
            return statsResponse;
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
        public async Task<OddsInfo> GetOdds(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}odds?fixture={fixtureId}&bookmaker=8");

            if (!response.IsSuccessStatusCode)
            {
                return new OddsInfo();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballOddsResponseDTO>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var oddsDataDTO = apiResponse?.Response?.FirstOrDefault();
            if (oddsDataDTO == null)
            {
                return new OddsInfo();
            }


            var odds = new OddsInfo
            {
                League = oddsDataDTO.League,
                Fixture = oddsDataDTO.Fixture,
                Update = oddsDataDTO.Update,
                Bookmakers = oddsDataDTO.Bookmakers.Select(b => new Bookmaker
                {
                    Id = b.Id,
                    Name = b.Name,
                    Bets = b.Bets.Select(bet => new Bet
                    {
                        Id = bet.Id,
                        Name = bet.Name,
                        Values = bet.Values.Select(v => new BetValue
                        {
                            Value = v.Value,
                            Odd = double.TryParse(v.Odd, out double parsedOdd) ? parsedOdd : 0.0
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
            return odds;
        }
    }
    public class ApiFootballOddsResponse
    {
        public List<OddsInfo> Response { get; set; }
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
    public class ApiFootballMatchesResponse
    {
        public List<Matches> Response { get; set; }
    }
}