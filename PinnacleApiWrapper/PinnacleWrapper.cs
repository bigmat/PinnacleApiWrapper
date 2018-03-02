using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace PinnacleApiWrapper
{
    public partial class PinnacleWrapper
    {
        public PinnacleWrapper(string clientId, string password) : this(new HttpClient())
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
