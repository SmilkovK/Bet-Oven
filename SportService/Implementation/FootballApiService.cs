using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SportDomain.DTO;
using SportDomain.models;

namespace SportService.Implementation
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v3.football.api-sports.io/";
        private const string ApiKey = "f5728cad0c3c3537e7ffcb5255b0191e";

        public FootballApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
        }

        private MatchStatus MapStatus(ApiStatus apiStatus)
        {
            if (apiStatus == null)
                return null;

            return new MatchStatus
            {
                Long = apiStatus.Long,
                Short = apiStatus.Short,
                Elapsed = apiStatus.Elapsed
            };
        }

        public async Task<List<Fixture>> GetFixtures(int leagueId, int season)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?league={leagueId}&season={season}");

            if (!response.IsSuccessStatusCode) return new List<Fixture>();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var fixtures = apiResponse?.Response ?? new List<Fixture>();

            foreach (var fixture in fixtures)
            {
                if (fixture.Timestamp > 0)
                {
                    fixture.Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;
                }
            }

            var allOdds = await GetOdds(DateTime.UtcNow.ToString("yyyy-MM-dd"), bookmakerId: 8);

            foreach (var fixture in fixtures)
            {
                fixture.Odds = allOdds.FirstOrDefault(o => o.Fixture.Id == fixture.Id) ?? new OddsInfo();
            }

            return fixtures;
        }

        public async Task<MatchResult> GetMatchAsync(string homeTeam, string awayTeam, DateTime? date = null)
        {
            List<Fixture> fixtures;

            if (date.HasValue)
            {
                fixtures = await GetFixturesByDate(date.Value);
            }
            else
            {
                fixtures = await GetTodaysFixtures();
            }

            var fixture = fixtures.FirstOrDefault(f =>
                string.Equals(f.Teams.Home.Name, homeTeam, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(f.Teams.Away.Name, awayTeam, StringComparison.OrdinalIgnoreCase));

            if (fixture == null || fixture.Goals == null)
                return null;

            return new MatchResult
            {
                HomeGoals = fixture.Goals.Home ?? 0,
                AwayGoals = fixture.Goals.Away ?? 0,
                Finished = string.Equals(fixture.Status?.Short, "FT", StringComparison.OrdinalIgnoreCase)
            };
        }
        public async Task<List<Fixture>> GetFixturesByDate(DateTime date, int bookmakerId = 8)
        {
            string formattedDate = date.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?date={formattedDate}");
            if (!response.IsSuccessStatusCode) return new List<Fixture>();

            var json = await response.Content.ReadAsStringAsync();
            var fixturesApi = JsonSerializer.Deserialize<ApiFootballResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var fixtures = fixturesApi?.Response?.Select(f => new Fixture
            {
                Id = f.Fixture.Id,
                Timestamp = f.Fixture.Timestamp,
                Date = f.Fixture.Date,
                Status = MapStatus(f.Fixture.Status),
                League = f.League,
                Teams = f.Teams,
                Goals = f.Goals,
                Popular = f.Popular
            }).ToList() ?? new List<Fixture>();

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

        public async Task<List<Fixture>> GetTodaysFixtures(int bookmakerId = 8)
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var fixtureResponse = await _httpClient.GetAsync($"{BaseUrl}fixtures?date={today}");
            if (!fixtureResponse.IsSuccessStatusCode) return new List<Fixture>();

            var fixtureJson = await fixtureResponse.Content.ReadAsStringAsync();
            var fixturesApi = JsonSerializer.Deserialize<ApiFootballResponse>(fixtureJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var fixtures = fixturesApi?.Response?.Select(f => new Fixture
            {
                Id = f.Fixture.Id,
                Timestamp = f.Fixture.Timestamp,
                Date = f.Fixture.Date,
                Status = MapStatus(f.Fixture.Status),
                League = f.League,
                Teams = f.Teams,
                Goals = f.Goals,
                Popular = f.Popular
            }).ToList() ?? new List<Fixture>();

            var allOdds = new List<OddsInfo>();
            int currentPage = 1;
            int totalPages = 1;
            int maxPages = 30;

            do
            {
                var oddsResponse = await _httpClient.GetAsync($"{BaseUrl}odds?date={today}&bookmaker={bookmakerId}&page={currentPage}");
                if (!oddsResponse.IsSuccessStatusCode) break;

                var oddsJson = await oddsResponse.Content.ReadAsStringAsync();
                var oddsApi = JsonSerializer.Deserialize<ApiFootballOddsPagedResponse>(oddsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (oddsApi?.Response != null)
                    allOdds.AddRange(oddsApi.Response);

                totalPages = oddsApi?.Paging?.Total ?? 1;
                currentPage++;

            } while (currentPage <= totalPages && currentPage <= maxPages);

            foreach (var fixture in fixtures)
            {
                fixture.Odds = allOdds.FirstOrDefault(o => o.Fixture.Id == fixture.Id);
            }

            fixtures = fixtures.Where(f => f.Odds != null).ToList();

            return fixtures;
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

            var apiResponse = JsonSerializer.Deserialize<ApiFootballResponse>(json, options);

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
                        Status = MapStatus(apiWrapper.Fixture.Status),
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
            var live = await GetLiveMatches();
            var fixture = live.FirstOrDefault(f => f.Id == fixtureId);
            if (fixture != null) return fixture;

            var today = await GetFixturesByDate(DateTime.UtcNow);
            fixture = today.FirstOrDefault(f => f.Id == fixtureId);
            if (fixture != null) return fixture;

            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures?id={fixtureId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiFootballFixturesResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            fixture = apiResponse?.Response?.FirstOrDefault();

            if (fixture != null && fixture.Timestamp > 0)
                fixture.Date = DateTimeOffset.FromUnixTimeSeconds(fixture.Timestamp).UtcDateTime;

            return fixture;
        }

        public async Task<ApiStatsResponse> GetFixtureStatistics(int fixtureId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}fixtures/statistics?fixture={fixtureId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var statsResponse = JsonSerializer.Deserialize<ApiStatsResponse>(json, options);
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

        public async Task<List<OddsInfo>> GetOdds(string date, int bookmakerId = 8)
        {
            var allOdds = new List<OddsInfo>();
            int currentPage = 1;
            int totalPages = 1;
            int maxPages = 30;

            do
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}odds?date={date}&bookmaker={bookmakerId}&page={currentPage}");

                if (!response.IsSuccessStatusCode)
                    break;

                var json = await response.Content.ReadAsStringAsync();
                var oddsApi = JsonSerializer.Deserialize<ApiFootballOddsPagedResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (oddsApi?.Response != null)
                    allOdds.AddRange(oddsApi.Response);

                totalPages = oddsApi?.Paging?.Total ?? 1;
                currentPage++;

            } while (currentPage <= totalPages && currentPage <= maxPages);

            return allOdds;
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

    public class ApiFootballFixturesResponse
    {
        public List<Fixture> Response { get; set; }
    }

    public class LeagueResponse
    {
        public LeagueStandings League { get; set; }
    }

    public class LeagueStandings
    {
        public List<List<Standing>> Standings { get; set; }
    }
}
