using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using PinnacleApiWrapper;

namespace PinnacleApiWrapperTest
{
    [TestFixture]
    public class PinnacleWrapperTest
    {
        PinnacleWrapper GetPinnacleWrapperMock(string s)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(s)
                }));

            var pinnacleWrapper = new PinnacleWrapper(new HttpClient(mockMessageHandler.Object));
            return pinnacleWrapper;
        }

        [Test]
        public async Task GetBetsByTypeAsync_WithStraightBetAndParlayBet_Return2BetsAndExpectedValues()
        {
            // Arrange
            var jsonForTest = "{\r\n  \"bets\" : [ {\r\n    \"betId\" : 278376930,\r\n    \"wagerNumber\" : 1,\r\n    \"placedAt\" : \"2018-02-11T15:37:10Z\",\r\n    \"win\" : 42.84,\r\n    \"risk\" : 45.0,\r\n    \"winLoss\" : 42.84,\r\n    \"betStatus\" : \"WON\",\r\n    \"betType\" : \"TOTAL_POINTS\",\r\n    \"oddsFormat\" : \"DECIMAL\",\r\n    \"customerCommission\" : 0.00,\r\n    \"sportId\" : 4,\r\n    \"leagueId\" : 472,\r\n    \"eventId\" : 814769595,\r\n    \"handicap\" : 163.0,\r\n    \"price\" : 1.952,\r\n    \"side\" : \"OVER\",\r\n    \"pitcher1MustStart\" : \"FALSE\",\r\n    \"pitcher2MustStart\" : \"FALSE\",\r\n    \"isLive\" : \"TRUE\",\r\n    \"team1\" : \"Vanoli Braga Cremona\",\r\n    \"team2\" : \"Reyer Venezia Mestre\",\r\n    \"periodNumber\" : 0,\r\n    \"team1Score\" : 0,\r\n    \"team2Score\" : 0,\r\n    \"ftTeam1Score\" : 83,\r\n    \"ftTeam2Score\" : 85\r\n  }, {\r\n    \"betId\" : 266314288,\r\n    \"wagerNumber\" : 1,\r\n    \"placedAt\" : \"2018-01-16T21:21:04Z\",\r\n    \"win\" : 21.6,\r\n    \"risk\" : 45.0,\r\n    \"winLoss\" : 12.825,\r\n    \"betStatus\" : \"WON\",\r\n    \"betType\" : \"PARLAY\",\r\n    \"oddsFormat\" : \"DECIMAL\",\r\n    \"customerCommission\" : 0E-9,\r\n    \"legs\" : [ {\r\n      \"sportId\" : 33,\r\n      \"leagueId\" : 3260,\r\n      \"eventId\" : 807535906,\r\n      \"eventStartTime\" : \"2018-01-17T02:15:00Z\",\r\n      \"legBetType\" : \"MONEYLINE\",\r\n      \"legBetStatus\" : \"WON\",\r\n      \"handicap\" : 0.0,\r\n      \"price\" : 1.285,\r\n      \"teamName\" : \"Denis Shapovalov (+2.5 Sets)\",\r\n      \"pitcher1MustStart\" : false,\r\n      \"pitcher2MustStart\" : false,\r\n      \"team1\" : \"Denis Shapovalov (+2.5 Sets)\",\r\n      \"team2\" : \"Jo-Wilfried Tsonga (-2.5 Sets)\",\r\n      \"isLive\" : false,\r\n      \"periodNumber\" : 0\r\n    }, {\r\n      \"sportId\" : 33,\r\n      \"leagueId\" : 3260,\r\n      \"eventId\" : 807494698,\r\n      \"eventStartTime\" : \"2018-01-17T00:30:00Z\",\r\n      \"legBetType\" : \"MONEYLINE\",\r\n      \"legBetStatus\" : \"PUSH\",\r\n      \"handicap\" : 0.0,\r\n      \"price\" : 1.151,\r\n      \"teamName\" : \"Gilles Simon (+2.5 Sets)\",\r\n      \"pitcher1MustStart\" : false,\r\n      \"pitcher2MustStart\" : false,\r\n      \"team1\" : \"Pablo Carreno-Busta (-2.5 Sets)\",\r\n      \"team2\" : \"Gilles Simon (+2.5 Sets)\",\r\n      \"isLive\" : false,\r\n      \"periodNumber\" : 0\r\n    } ],\r\n    \"odds\" : 1.48\r\n  } ]\r\n}";

            var client = GetPinnacleWrapperMock(jsonForTest);

            // Act
            var result = await client.GetBetsByTypeAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.StraightBets, "1 StraightBet should be retrieved");
            Assert.AreEqual(1, result.StraightBets.Count, "1 StraightBet should be retrieved");
            var straightBet = result.StraightBets.Single();
            Assert.AreEqual(278376930, straightBet.BetId);
            Assert.AreEqual(1, straightBet.WagerNumber);
            Assert.AreEqual(new DateTime(2018, 02, 11, 15, 37, 10, DateTimeKind.Utc), straightBet.PlacedAt);
            Assert.AreEqual(42.84, straightBet.Win);
            Assert.AreEqual(45.0, straightBet.Risk);
            Assert.AreEqual(42.84, straightBet.WinLoss);
            Assert.AreEqual(StraightBetBetStatus.WON, straightBet.BetStatus);
            Assert.AreEqual(StraightBetBetType.TOTAL_POINTS, straightBet.BetType);
            Assert.AreEqual(OddsFormat.DECIMAL, straightBet.OddsFormat);
            Assert.AreEqual(0.0, straightBet.CustomerCommission);
            Assert.AreEqual(4, straightBet.SportId);
            Assert.AreEqual(472, straightBet.LeagueId);
            Assert.AreEqual(814769595, straightBet.EventId);
            Assert.AreEqual(163.0, straightBet.Handicap);
            Assert.AreEqual(1.952, straightBet.Price);
            Assert.AreEqual(StraightBetSide.OVER, straightBet.Side);
            Assert.AreEqual(StraightBetPitcher1MustStart.False, straightBet.Pitcher1MustStart);
            Assert.AreEqual(StraightBetPitcher2MustStart.False, straightBet.Pitcher2MustStart);
            Assert.AreEqual(StraightBetIsLive.True, straightBet.IsLive);
            Assert.AreEqual("Vanoli Braga Cremona", straightBet.Team1);
            Assert.AreEqual("Reyer Venezia Mestre", straightBet.Team2);
            Assert.AreEqual(0, straightBet.PeriodNumber);
            Assert.AreEqual(0, straightBet.Team1Score);
            Assert.AreEqual(0, straightBet.Team2Score);
            Assert.AreEqual(83, straightBet.FtTeam1Score);
            Assert.AreEqual(85, straightBet.FtTeam2Score);

            Assert.IsNotNull(result.ParlayBets, "1 ParlayBet should be retrieved");
            Assert.AreEqual(1, result.ParlayBets.Count, "1 ParlayBet should be retrieved");
            var parlayBet = result.ParlayBets.Single();
            Assert.AreEqual(266314288, parlayBet.BetId);
            Assert.AreEqual(1, parlayBet.WagerNumber);
            Assert.AreEqual(new DateTime(2018, 01, 16, 21, 21, 04, DateTimeKind.Utc), parlayBet.PlacedAt);
            Assert.AreEqual(21.6, parlayBet.Win);
            Assert.AreEqual(45.0, parlayBet.Risk);
            Assert.AreEqual(12.825, parlayBet.WinLoss);
            Assert.AreEqual(ParlayBetBetStatus.WON, parlayBet.BetStatus);
            Assert.AreEqual(StraightBetBetType.PARLAY.ToString(), parlayBet.BetType);
            Assert.AreEqual(OddsFormat.DECIMAL, parlayBet.OddsFormat);
            Assert.AreEqual(0.0, parlayBet.CustomerCommission);
            Assert.AreEqual(2, parlayBet.Legs.Count);
            var leg1 = parlayBet.Legs.Single(x => x.EventId == 807535906);
            Assert.IsNotNull(leg1, "Parlay leg 1 must not be null");
            Assert.AreEqual(33, leg1.SportId);
            Assert.AreEqual(3260, leg1.LeagueId);
            Assert.AreEqual(new DateTime(2018, 01, 17, 02, 15, 00, DateTimeKind.Utc), leg1.EventStartTime);
            Assert.AreEqual(ParlayLegLegBetType.MONEYLINE, leg1.LegBetType);
            Assert.AreEqual(ParlayLegLegBetStatus.WON, leg1.LegBetStatus);
            Assert.AreEqual(0.0, leg1.Handicap);
            Assert.AreEqual(1.285, leg1.Price);
            Assert.AreEqual("Denis Shapovalov (+2.5 Sets)", leg1.TeamName);
            Assert.AreEqual(false, leg1.Pitcher1MustStart);
            Assert.AreEqual(false, leg1.Pitcher2MustStart);
            Assert.AreEqual("Denis Shapovalov (+2.5 Sets)", leg1.Team1);
            Assert.AreEqual("Jo-Wilfried Tsonga (-2.5 Sets)", leg1.Team2);
            Assert.AreEqual(0, leg1.PeriodNumber);
            var leg2 = parlayBet.Legs.Single(x => x.EventId == 807494698);
            Assert.IsNotNull(leg2, "Parlay leg 2 must not be null");
            Assert.AreEqual(33, leg2.SportId);
            Assert.AreEqual(3260, leg2.LeagueId);
            Assert.AreEqual(new DateTime(2018, 01, 17, 00, 30, 00, DateTimeKind.Utc), leg2.EventStartTime);
            Assert.AreEqual(ParlayLegLegBetType.MONEYLINE, leg2.LegBetType);
            Assert.AreEqual(ParlayLegLegBetStatus.PUSH, leg2.LegBetStatus);
            Assert.AreEqual(0.0, leg2.Handicap);
            Assert.AreEqual(1.151, leg2.Price);
            Assert.AreEqual("Gilles Simon (+2.5 Sets)", leg2.TeamName);
            Assert.AreEqual(false, leg2.Pitcher1MustStart);
            Assert.AreEqual(false, leg2.Pitcher2MustStart);
            Assert.AreEqual("Pablo Carreno-Busta (-2.5 Sets)", leg2.Team1);
            Assert.AreEqual("Gilles Simon (+2.5 Sets)", leg2.Team2);
            Assert.AreEqual(0, leg2.PeriodNumber);
        }

        [Test]
        public async Task GetBetsByTypeAsync_WithSpecialBet_Return1BetAndExpectedValues()
        {
            // Arrange
            var jsonForTest = "{\r\n  \"bets\" : [ {\r\n    \"betId\" : 266187478,\r\n    \"wagerNumber\" : 1,\r\n    \"placedAt\" : \"2018-01-16T16:17:22Z\",\r\n    \"win\" : 28.13,\r\n    \"risk\" : 22.5,\r\n    \"winLoss\" : -22.5,\r\n    \"betStatus\" : \"LOSE\",\r\n    \"betType\" : \"SPECIAL\",\r\n    \"oddsFormat\" : \"DECIMAL\",\r\n    \"customerCommission\" : 0E-9,\r\n    \"sportId\" : 4,\r\n    \"leagueId\" : 382,\r\n    \"eventId\" : 805536126,\r\n    \"handicap\" : 13.5,\r\n    \"price\" : 2.25,\r\n    \"teamName\" : \"\",\r\n    \"pitcher1\" : \"\",\r\n    \"pitcher2\" : \"\",\r\n    \"pitcher1MustStart\" : \"FALSE\",\r\n    \"pitcher2MustStart\" : \"FALSE\",\r\n    \"isLive\" : \"FALSE\",\r\n    \"team1\" : \"\",\r\n    \"team2\" : \"\",\r\n    \"periodNumber\" : 0,\r\n    \"team1Score\" : 0,\r\n    \"team2Score\" : 0,\r\n    \"ftTeam1Score\" : 0,\r\n    \"ftTeam2Score\" : 0,\r\n    \"pTeam1Score\" : 0,\r\n    \"pTeam2Score\" : 0\r\n  } ]\r\n}";

            var client = GetPinnacleWrapperMock(jsonForTest);
            
            // Act
            var result = await client.GetBetsByTypeAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.SpecialBets, "1 StraightBet should be retrieved");
            Assert.AreEqual(1, result.SpecialBets.Count, "1 StraightBet should be retrieved");
            var straightBet = result.SpecialBets.Single();
            Assert.AreEqual(266187478, straightBet.BetId);
            Assert.AreEqual(1, straightBet.WagerNumber);
            Assert.AreEqual(new DateTime(2018, 01, 16, 16, 17, 22, DateTimeKind.Utc), straightBet.PlacedAt);
            Assert.AreEqual(28.13, straightBet.Win);
            Assert.AreEqual(22.5, straightBet.Risk);
            Assert.AreEqual(-22.5, straightBet.WinLoss);
            Assert.AreEqual(SpecialBetBetStatus.LOSE, straightBet.BetStatus);
            Assert.AreEqual(StraightBetBetType.SPECIAL.ToString(), straightBet.BetType);
            Assert.AreEqual(OddsFormat.DECIMAL, straightBet.OddsFormat);
            Assert.AreEqual(0.0, straightBet.CustomerCommission);
            Assert.AreEqual(4, straightBet.SportId);
            Assert.AreEqual(382, straightBet.LeagueId);
            Assert.AreEqual(805536126, straightBet.EventId);
            Assert.AreEqual(13.5, straightBet.Handicap);
            Assert.AreEqual(2.25, straightBet.Price);
            Assert.AreEqual(string.Empty, straightBet.Team1);
            Assert.AreEqual(string.Empty, straightBet.Team2);
            Assert.AreEqual(0, straightBet.PeriodNumber);
        }

        [Test]
        public async Task GetBetsByTypeAsync_WithNoBet_ReturnObjectNotNullWithEmptyCollections()
        {
            // Arrange
            var jsonForTest = "{\r\n  \"bets\" : [ ]\r\n}";

            var client = GetPinnacleWrapperMock(jsonForTest);
            
            // Act
            var result = await client.GetBetsByTypeAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ManualBets);
            Assert.IsNotNull(result.ParlayBets);
            Assert.IsNotNull(result.SpecialBets);
            Assert.IsNotNull(result.StraightBets);
            Assert.IsNotNull(result.TeaserBets);
        }
    }
}
