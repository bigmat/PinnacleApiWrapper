using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PinnacleApiWrapper
{
    internal class GetBetsByTypeResponseJsonConverter : CustomCreationConverter<GetBetsByTypeResponse>
    {
        #region Overrides of CustomCreationConverter<GetBetsByTypeResponse>

        public override GetBetsByTypeResponse Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides of CustomCreationConverter<GetBetsByTypeResponse>

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(GetBetsByTypeResponse))
            {
                var result = new GetBetsByTypeResponse();
                result.ParlayBets = new ObservableCollection<ParlayBet>();
                result.StraightBets = new ObservableCollection<StraightBet>();
                result.ManualBets = new ObservableCollection<ManualBet>();
                result.SpecialBets = new ObservableCollection<SpecialBet>();
                result.TeaserBets = new ObservableCollection<TeaserBet>();

                var jObject = JObject.Load(reader);

                if (jObject.First.First is JArray jArray)
                {
                    foreach (var jToken in jArray)
                    {
                        if (jToken is JObject bet)
                        {
                            var betType = bet.Property("betType").Value?.ToString();
                            if (betType == StraightBetBetType.PARLAY.ToString())
                            {
                                var parlayBet = bet.ToObject<ParlayBet>();
                                result.ParlayBets.Add(parlayBet);
                            }
                            else if (betType == StraightBetBetType.SPECIAL.ToString())
                            {
                                var specialBet = bet.ToObject<SpecialBet>();
                                result.SpecialBets.Add(specialBet);
                            }
                            else if (betType == StraightBetBetType.TEASER.ToString())
                            {
                                var teaserBet = bet.ToObject<TeaserBet>();
                                result.TeaserBets.Add(teaserBet);
                            }
                            else if (betType == StraightBetBetType.MANUAL.ToString())
                            {
                                var manualBet = bet.ToObject<ManualBet>();
                                result.ManualBets.Add(manualBet);
                            }
                            else
                            {
                                var straightBet = bet.ToObject<StraightBet>();
                                result.StraightBets.Add(straightBet);
                            }
                        }
                    }
                }

                return result;
            }
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        #endregion
    }
}
