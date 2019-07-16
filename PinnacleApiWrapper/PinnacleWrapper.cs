using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PinnacleApiWrapper;

// ReSharper disable once CheckNamespace
namespace Ps3838.Bets
{
    public partial class BetsClient
    {
        public BetsClient(string clientId, string password) : this(new HttpClient())
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{password}")));
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new GetBetsByTypeResponseJsonConverter());
        }
    }
}
