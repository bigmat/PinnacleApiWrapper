using Newtonsoft.Json;

namespace PinnacleApiWrapper
{
    public partial class PinnacleWrapper
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new GetBetsByTypeResponseJsonConverter());
        }
    }
}
