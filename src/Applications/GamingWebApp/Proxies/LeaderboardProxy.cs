using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
//using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GamingWebApp.Proxies
{
    // Refit tooling is using Mono.exe which is not part of build image
    // microsoft/aspnetcore-build:1.0-1.1
    // Left it here for educational purposes

    public interface ILeaderboard
    {
        [Headers("User-Agent: TechDays 2017 Gaming App")]
        [Get("/api/leaderboard")]
        Task<IEnumerable<string>> GetLeaderboard();
    }

    public class LeaderboardProxy
    {
        private readonly string hostUri;
        private const string leaderboardApiEndpoint = "/api/leaderboard";
        private ILogger logger;

        public LeaderboardProxy(string hostUri, ILogger logger)
        {
            this.logger = logger;
            this.hostUri = hostUri;
        }

        //public async Task<IEnumerable<string>> GetLeaderboardAsync()
        //{
        //    var api = RestService.For<ILeaderboard>(new ResilientHttpClient(logger), hostUri);
        //    return await api.GetLeaderboard();
        //}

        public async Task<IEnumerable<dynamic>> GetLeaderboardAsync()
        {
                HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(
                    HttpMethod.Get,
                    hostUri + leaderboardApiEndpoint);
                response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new List<string>();

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content);
            }
            catch (HttpRequestException)
            {
                return new List<string>();
            }

        }
    }
}
