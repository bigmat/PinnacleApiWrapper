﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Ps3838.Bets;
using Ps3838.Customer;
using Ps3838.Lines;
using OddsFormat = Ps3838.Bets.OddsFormat;
using RoundRobinOptionWithOddsRoundRobinOption = Ps3838.Bets.RoundRobinOptionWithOddsRoundRobinOption;

namespace PinnacleApiWrapperTest
{
    [TestFixture]
    public class PinnacleWrapperTest
    {
        BetsClient GetBetsClientMock(string s)
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

            var pinnacleWrapper = new BetsClient(new HttpClient(mockMessageHandler.Object));
            return pinnacleWrapper;
        }

        LinesClient GetLinesClientMock(string s)
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

            var pinnacleWrapper = new LinesClient(new HttpClient(mockMessageHandler.Object));
            return pinnacleWrapper;
        }

        CustomerClient GetCustomerClientMock(string s)
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

            var pinnacleWrapper = new CustomerClient(new HttpClient(mockMessageHandler.Object));
            return pinnacleWrapper;
        }

        [Test]
        public async Task GetBetsByTypeAsync_WithStraightBetAndParlayBet_Return2BetsAndExpectedValues()
        {
            // Arrange
            var jsonForTest = "{\r\n  \"bets\" : [ {\r\n    \"betId\" : 278376930,\r\n    \"wagerNumber\" : 1,\r\n    \"placedAt\" : \"2018-02-11T15:37:10Z\",\r\n    \"win\" : 42.84,\r\n    \"risk\" : 45.0,\r\n    \"winLoss\" : 42.84,\r\n    \"betStatus\" : \"WON\",\r\n    \"betType\" : \"TOTAL_POINTS\",\r\n    \"oddsFormat\" : \"DECIMAL\",\r\n    \"customerCommission\" : 0.00,\r\n    \"sportId\" : 4,\r\n    \"leagueId\" : 472,\r\n    \"eventId\" : 814769595,\r\n    \"handicap\" : 163.0,\r\n    \"price\" : 1.952,\r\n    \"side\" : \"OVER\",\r\n    \"pitcher1MustStart\" : \"FALSE\",\r\n    \"pitcher2MustStart\" : \"FALSE\",\r\n    \"isLive\" : \"TRUE\",\r\n    \"team1\" : \"Vanoli Braga Cremona\",\r\n    \"team2\" : \"Reyer Venezia Mestre\",\r\n    \"periodNumber\" : 0,\r\n    \"team1Score\" : 0,\r\n    \"team2Score\" : 0,\r\n    \"ftTeam1Score\" : 83,\r\n    \"ftTeam2Score\" : 85\r\n  }, {\r\n    \"betId\" : 266314288,\r\n    \"wagerNumber\" : 1,\r\n    \"placedAt\" : \"2018-01-16T21:21:04Z\",\r\n    \"win\" : 21.6,\r\n    \"risk\" : 45.0,\r\n    \"winLoss\" : 12.825,\r\n    \"betStatus\" : \"WON\",\r\n    \"betType\" : \"PARLAY\",\r\n    \"oddsFormat\" : \"DECIMAL\",\r\n    \"customerCommission\" : 0E-9,\r\n    \"legs\" : [ {\r\n      \"sportId\" : 33,\r\n      \"leagueId\" : 3260,\r\n      \"eventId\" : 807535906,\r\n      \"eventStartTime\" : \"2018-01-17T02:15:00Z\",\r\n      \"legBetType\" : \"MONEYLINE\",\r\n      \"legBetStatus\" : \"WON\",\r\n      \"handicap\" : 0.0,\r\n      \"price\" : 1.285,\r\n      \"teamName\" : \"Denis Shapovalov (+2.5 Sets)\",\r\n      \"pitcher1MustStart\" : false,\r\n      \"pitcher2MustStart\" : false,\r\n      \"team1\" : \"Denis Shapovalov (+2.5 Sets)\",\r\n      \"team2\" : \"Jo-Wilfried Tsonga (-2.5 Sets)\",\r\n      \"isLive\" : false,\r\n      \"periodNumber\" : 0\r\n    }, {\r\n      \"sportId\" : 33,\r\n      \"leagueId\" : 3260,\r\n      \"eventId\" : 807494698,\r\n      \"eventStartTime\" : \"2018-01-17T00:30:00Z\",\r\n      \"legBetType\" : \"MONEYLINE\",\r\n      \"legBetStatus\" : \"PUSH\",\r\n      \"handicap\" : 0.0,\r\n      \"price\" : 1.151,\r\n      \"teamName\" : \"Gilles Simon (+2.5 Sets)\",\r\n      \"pitcher1MustStart\" : false,\r\n      \"pitcher2MustStart\" : false,\r\n      \"team1\" : \"Pablo Carreno-Busta (-2.5 Sets)\",\r\n      \"team2\" : \"Gilles Simon (+2.5 Sets)\",\r\n      \"isLive\" : false,\r\n      \"periodNumber\" : 0\r\n    } ],\r\n    \"odds\" : 1.48\r\n  } ]\r\n}";

            var client = GetBetsClientMock(jsonForTest);

            // Act
            var result = await client.V1BetsAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.StraightBets, "1 StraightBet should be retrieved");
            Assert.AreEqual(1, result.StraightBets.Count, "1 StraightBet should be retrieved");
            var straightBet = result.StraightBets.Single();
            Assert.AreEqual(278376930, straightBet.BetId);
            Assert.AreEqual(1, straightBet.WagerNumber);
            Assert.AreEqual(new DateTimeOffset(2018, 02, 11, 15, 37, 10, TimeSpan.Zero), straightBet.PlacedAt);
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
            Assert.AreEqual(StraightBetPitcher1MustStart.FALSE, straightBet.Pitcher1MustStart);
            Assert.AreEqual(StraightBetPitcher2MustStart.FALSE, straightBet.Pitcher2MustStart);
            Assert.AreEqual(StraightBetIsLive.TRUE, straightBet.IsLive);
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
            Assert.AreEqual(new DateTimeOffset(2018, 01, 16, 21, 21, 04, TimeSpan.Zero), parlayBet.PlacedAt);
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
            Assert.AreEqual(new DateTimeOffset(2018, 01, 17, 02, 15, 00, TimeSpan.Zero), leg1.EventStartTime);
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
            Assert.AreEqual(new DateTimeOffset(2018, 01, 17, 00, 30, 00, TimeSpan.Zero), leg2.EventStartTime);
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

            var client = GetBetsClientMock(jsonForTest);
            
            // Act
            var result = await client.V1BetsAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result.SpecialBets, "1 StraightBet should be retrieved");
            Assert.AreEqual(1, result.SpecialBets.Count, "1 StraightBet should be retrieved");
            var straightBet = result.SpecialBets.Single();
            Assert.AreEqual(266187478, straightBet.BetId);
            Assert.AreEqual(1, straightBet.WagerNumber);
            Assert.AreEqual(new DateTimeOffset(2018, 01, 16, 16, 17, 22, TimeSpan.Zero), straightBet.PlacedAt);
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

            var client = GetBetsClientMock(jsonForTest);
            
            // Act
            var result = await client.V1BetsAsync(Betlist.SETTLED, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ManualBets);
            Assert.IsNotNull(result.ParlayBets);
            Assert.IsNotNull(result.SpecialBets);
            Assert.IsNotNull(result.StraightBets);
            Assert.IsNotNull(result.TeaserBets);
        }

        [Test]
        public async Task GetAsyncSportsResponse_WithSportsList_ReturnSportsList()
        {
            // Arrange
            var jsonForTest = "{\"sports\":[{\"id\":1,\"name\":\"Badminton\",\"hasOfferings\":false},{\"id\":2,\"name\":\"Bandy\",\"hasOfferings\":true},{\"id\":3,\"name\":\"Baseball\",\"hasOfferings\":true},{\"id\":4,\"name\":\"Basketball\",\"hasOfferings\":true},{\"id\":5,\"name\":\"Beach Volleyball\",\"hasOfferings\":false},{\"id\":6,\"name\":\"Boxing\",\"hasOfferings\":true},{\"id\":7,\"name\":\"Chess\",\"hasOfferings\":false},{\"id\":8,\"name\":\"Cricket\",\"hasOfferings\":true},{\"id\":9,\"name\":\"Curling\",\"hasOfferings\":true},{\"id\":10,\"name\":\"Darts\",\"hasOfferings\":false},{\"id\":11,\"name\":\"Darts (Legs)\",\"hasOfferings\":false},{\"id\":13,\"name\":\"Field Hockey\",\"hasOfferings\":false},{\"id\":14,\"name\":\"Floorball\",\"hasOfferings\":false},{\"id\":15,\"name\":\"Football\",\"hasOfferings\":false},{\"id\":16,\"name\":\"Futsal\",\"hasOfferings\":false},{\"id\":17,\"name\":\"Golf\",\"hasOfferings\":true},{\"id\":18,\"name\":\"Handball\",\"hasOfferings\":true},{\"id\":19,\"name\":\"Hockey\",\"hasOfferings\":true},{\"id\":20,\"name\":\"Horse Racing\",\"hasOfferings\":false},{\"id\":22,\"name\":\"Mixed Martial Arts\",\"hasOfferings\":true},{\"id\":24,\"name\":\"Politics\",\"hasOfferings\":false},{\"id\":26,\"name\":\"Rugby League\",\"hasOfferings\":true},{\"id\":27,\"name\":\"Rugby Union\",\"hasOfferings\":true},{\"id\":28,\"name\":\"Snooker\",\"hasOfferings\":false},{\"id\":29,\"name\":\"Soccer\",\"hasOfferings\":true},{\"id\":30,\"name\":\"Softball\",\"hasOfferings\":false},{\"id\":31,\"name\":\"Squash\",\"hasOfferings\":false},{\"id\":32,\"name\":\"Table Tennis\",\"hasOfferings\":false},{\"id\":33,\"name\":\"Tennis\",\"hasOfferings\":true},{\"id\":34,\"name\":\"Volleyball\",\"hasOfferings\":true},{\"id\":35,\"name\":\"Volleyball (Points)\",\"hasOfferings\":false},{\"id\":36,\"name\":\"Water Polo\",\"hasOfferings\":false},{\"id\":39,\"name\":\"Aussie Rules\",\"hasOfferings\":true},{\"id\":40,\"name\":\"Alpine Skiing\",\"hasOfferings\":false},{\"id\":41,\"name\":\"Biathlon\",\"hasOfferings\":true},{\"id\":42,\"name\":\"Ski Jumping\",\"hasOfferings\":false},{\"id\":43,\"name\":\"Cross Country\",\"hasOfferings\":true},{\"id\":44,\"name\":\"Formula 1\",\"hasOfferings\":false},{\"id\":45,\"name\":\"Cycling\",\"hasOfferings\":true},{\"id\":46,\"name\":\"Bobsleigh\",\"hasOfferings\":true},{\"id\":47,\"name\":\"Figure Skating\",\"hasOfferings\":false},{\"id\":48,\"name\":\"Freestyle Skiing\",\"hasOfferings\":false},{\"id\":49,\"name\":\"Luge\",\"hasOfferings\":false},{\"id\":50,\"name\":\"Nordic Combined\",\"hasOfferings\":false},{\"id\":51,\"name\":\"Short Track\",\"hasOfferings\":false},{\"id\":52,\"name\":\"Skeleton\",\"hasOfferings\":false},{\"id\":53,\"name\":\"Snow Boarding\",\"hasOfferings\":false},{\"id\":54,\"name\":\"Speed Skating\",\"hasOfferings\":false},{\"id\":55,\"name\":\"Olympics\",\"hasOfferings\":false},{\"id\":56,\"name\":\"Athletics\",\"hasOfferings\":false},{\"id\":57,\"name\":\"Crossfit\",\"hasOfferings\":true},{\"id\":58,\"name\":\"Entertainment\",\"hasOfferings\":true},{\"id\":60,\"name\":\"Drone Racing\",\"hasOfferings\":false},{\"id\":62,\"name\":\"Poker\",\"hasOfferings\":false}]}";

            var client = GetLinesClientMock(jsonForTest);
            
            // Act
            var result = await client.V1SportsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(54, result.Sports.Count);
            var badminton = result.Sports.Single(x => x.Id == 1);
            Assert.AreEqual("Badminton", badminton.Name);
            Assert.IsFalse(badminton.HasOfferings);
            var basket = result.Sports.Single(x => x.Id == 4);
            Assert.AreEqual("Basketball", basket.Name);
            Assert.IsTrue(basket.HasOfferings);
            var poker = result.Sports.Single(x => x.Id == 62);
            Assert.AreEqual("Poker", poker.Name);
            Assert.IsFalse(poker.HasOfferings);
        }

        [Test]
        public async Task GetAsyncLeagues_WithSports4_ReturnBasketLeagues()
        {
            // Arrange
            var jsonForTest = "{\"sportId\":4,\"leagues\":[{\"id\":267,\"name\":\"Europe - ABA League Adriatic All Star\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":268,\"name\":\"ABA - Adriatic League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":5135,\"name\":\"All African Games\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5131,\"name\":\"All African Games - Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5682,\"name\":\"America - CentroBasket Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199664,\"name\":\"American - Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":271,\"name\":\"International - Arab Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":280,\"name\":\"Australia - NBL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":282,\"name\":\"Australia - WNBL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":285,\"name\":\"Austria - OBL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":287,\"name\":\"Europe - Baltic BL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":4508,\"name\":\"Baltic - Basketball League Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":288,\"name\":\"Baltic - Challenge Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4616,\"name\":\"Asian Games\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":4627,\"name\":\"Asian Games - Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":290,\"name\":\"Brazil - Campeonato Nacional\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":297,\"name\":\"Bulgaria - Basketball League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":300,\"name\":\"Central America and Caribbean Games\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":301,\"name\":\"Central America and Caribbean Games - Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":303,\"name\":\"China - Basketball Association\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":306,\"name\":\"Croatia - A1 Liga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":293,\"name\":\"Brazil - Novo Basquete Brasil\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":317,\"name\":\"Denmark - Basketligaen\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":329,\"name\":\"Europe - Cup Allstar Game\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":197775,\"name\":\"Europe - Champions League Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197844,\"name\":\"Europe - Champions League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4489,\"name\":\"Europe - Club Friendlies\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4518,\"name\":\"Europe - Cup Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":335,\"name\":\"Europe - CEBL Final Four\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":339,\"name\":\"Europe - European U16 Championships Div. A Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":340,\"name\":\"Europe - European U16 Championships Div. B Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":344,\"name\":\"Europe - European U18 Championship Division A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":345,\"name\":\"Europe - European U18 Championship Division B\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":355,\"name\":\"Europe - European U16 Championships Division A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":356,\"name\":\"Europe - European U16 Championships Division B\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":357,\"name\":\"Europe - European U20 Championship Division A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":358,\"name\":\"Europe -European U20 Championship Division A Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":359,\"name\":\"European - U20 Championship Division B Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":360,\"name\":\"Europe -European U20 Championship Division B Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":367,\"name\":\"Europe - FIBA Europe Cup Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":369,\"name\":\"Europe - European Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":370,\"name\":\"Europe - Championship Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":372,\"name\":\"Europe - European Championship Qualification Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":4564,\"name\":\"Europe - European Super Cup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":374,\"name\":\"Europe - VTB United League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":4914,\"name\":\"Europe - EuroBasket Division B Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5141,\"name\":\"Europe - Eurobasket Low Juice\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":6099,\"name\":\"Europe - Eurocup Interval Betting\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":4520,\"name\":\"Europe - EuroChallenge Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5702,\"name\":\"European - Championship Small Countries\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":377,\"name\":\"Europe - Eurocup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":379,\"name\":\"Europe - Eurocup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":381,\"name\":\"Europe - Euroleague American Tour\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4504,\"name\":\"Europe - Euroleague Qualification Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":382,\"name\":\"Europe - Euroleague\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":4503,\"name\":\"Europe - Euroleague Qualifications\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":383,\"name\":\"Europe - Euroleague Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":385,\"name\":\"Europe -European U18 Championship Division A Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":387,\"name\":\"Europe -European U18 Championship Division B Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":321,\"name\":\"England - BBL Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":322,\"name\":\"England - BBL Trophy\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":325,\"name\":\"Estonia - Meistriliiga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":389,\"name\":\"FIBA - Africa Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":390,\"name\":\"FIBA - Americas League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":391,\"name\":\"FIBA - American Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":392,\"name\":\"FIBA - Americas Championships\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":197607,\"name\":\"FIBA - Asia Challenge\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197186,\"name\":\"FIBA - Asia Championship Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":191546,\"name\":\"FIBA Asia Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197746,\"name\":\"FIBA Europe - Champions League Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":397,\"name\":\"FIBA - World Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":4492,\"name\":\"FIBA - World Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":9585,\"name\":\"FIBA - World Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":199578,\"name\":\"FIBA - World Cup Africa Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199579,\"name\":\"FIBA - World Cup Americas Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199577,\"name\":\"FIBA - World Cup Asia Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":197187,\"name\":\"Europe - Eurobasket Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5836,\"name\":\"Europe - Eurobasket Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":398,\"name\":\"FIBA - Centrobasket Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":399,\"name\":\"FIBA - Centrobasket Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":400,\"name\":\"FIBA - Marchand Continental Championship Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4402,\"name\":\"FIBA - Asia Stankovic Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":412,\"name\":\"France - Ligue Féminine de Basketball\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":414,\"name\":\"France - Championnat Pro A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":423,\"name\":\"Germany - Bundesliga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":437,\"name\":\"Greece - A1\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":441,\"name\":\"Greek - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5189,\"name\":\"Holland - FEB Eredivisie Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":454,\"name\":\"International - Tournament Trentino Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":455,\"name\":\"International - William Jones Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":456,\"name\":\"International - William Jones Cup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":457,\"name\":\"International - Tournaments Torneo Super\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":458,\"name\":\"International - Tournaments CBC Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":459,\"name\":\"International - Tournaments Trofeo Mandela Forum\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":417,\"name\":\"French - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":451,\"name\":\"International - Friendlies\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":452,\"name\":\"International - Tournaments Super Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":462,\"name\":\"Israel - Premier League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":472,\"name\":\"Italy - Lega A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":198004,\"name\":\"Korea - Korea Basketball League Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":481,\"name\":\"Korea - WKBL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":482,\"name\":\"Korea - Korea Basketball League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":6542,\"name\":\"Live FIBA - Africa Championship Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":198606,\"name\":\"NBA All Star Game\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":200665,\"name\":\"NBA - Alternate Lines\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":488,\"name\":\"Europe - NBA Tour\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197215,\"name\":\"NBA Summer League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":493,\"name\":\"NCAA\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":497,\"name\":\"NCAA Overtime\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":501,\"name\":\"Norway - BLNO\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":197230,\"name\":\"Olympic - Basketball Matches Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":509,\"name\":\"Pan American Games Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5209,\"name\":\"Pan American Games\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5366,\"name\":\"Philippines - PBA Commissioners Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":512,\"name\":\"Poland - Tauron Basket Liga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":5433,\"name\":\"Puerto Rico - Superior Nacional\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5700,\"name\":\"Olympic - Qualifying Tournament Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":527,\"name\":\"Romania - Division A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":4598,\"name\":\"Russia - Professional Basketball League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":548,\"name\":\"Slovenia - 1.A SKL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":550,\"name\":\"South American - League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":551,\"name\":\"South American -Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4404,\"name\":\"South American - Championship Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":537,\"name\":\"Serbia - Nasa Liga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":556,\"name\":\"Spain - LEB Gold\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":559,\"name\":\"Spain - ACB\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":567,\"name\":\"Switzerland -  LNA\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":199587,\"name\":\"The Basketball Tournament\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":568,\"name\":\"World - Torneo Orense\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5177,\"name\":\"Turkey - Presidents Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":563,\"name\":\"FIBA - Stankovic Continental Champions Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":565,\"name\":\"Sweden - Basketligan\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":197913,\"name\":\"Turkey - Super League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":570,\"name\":\"Turkey - TBBL Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":575,\"name\":\"Ukraine - SuperLiga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":577,\"name\":\"ULEB - Summer League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":9270,\"name\":\"European Championship Small Countries Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":579,\"name\":\"WNBA - All Star\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":196753,\"name\":\"WNBA - Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":582,\"name\":\"WNBA - Pre Season\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":583,\"name\":\"WNCAA\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":588,\"name\":\"World - Diamond Ball Cup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":589,\"name\":\"World - Diamond Ball Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":590,\"name\":\"World - International Friendlies Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":6673,\"name\":\"World - Intercontinental Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":591,\"name\":\"World - Mediterranean Games\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":592,\"name\":\"World - Mediterranean Games Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":593,\"name\":\"World - Al Ramsay Shield Series\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":594,\"name\":\"International -Tournaments Trofeo Gianatti\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":597,\"name\":\"World - International Four NationsTournament\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":599,\"name\":\"World - International Four NationsTournament Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":600,\"name\":\"World - Albert Schweitzer Tournament Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":602,\"name\":\"World - Universiade\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":603,\"name\":\"World - Universiade Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":604,\"name\":\"World - Club Friendlies\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":273,\"name\":\"Argentina - Super 8\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":274,\"name\":\"Argentina - Liga Nacional\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":283,\"name\":\"Austria - Super Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":6739,\"name\":\"Austria - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4521,\"name\":\"Australia - NBL Top End Challenge Darwin\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4769,\"name\":\"Belgium - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":6211,\"name\":\"Bulgaria - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":291,\"name\":\"Brazil - Campeonato Nacional Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":566,\"name\":\"Switzerland - Coupe de la ligue\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":308,\"name\":\"Cyprus - 1st Division\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":309,\"name\":\"Cyprus - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":311,\"name\":\"Czech Republic - Extraliga Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":312,\"name\":\"Czech Republic - League Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":313,\"name\":\"Czech Republic - Mattoni NBL\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":424,\"name\":\"Germany - Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":426,\"name\":\"Germany - All Star\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":427,\"name\":\"Germany -  BBL Champions Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197189,\"name\":\"Germany - Bundesliga Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":420,\"name\":\"Germany - BBL Pokal\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":316,\"name\":\"Denmark - Division 1\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":318,\"name\":\"Denmark - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":319,\"name\":\"Denmark - Dameligaen Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":7350,\"name\":\"Estonia - Meistriliiga Cup Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":552,\"name\":\"Spain - Copa Del Rey\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":553,\"name\":\"Spain - Copa Príncipe de Asturias\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":554,\"name\":\"Spain - Copa Addeco LEB Bronze\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":555,\"name\":\"Spain -  Copa Addeco LEB Silver\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":561,\"name\":\"Spain - Super Copa\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":404,\"name\":\"Finland - SM Sarja Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":407,\"name\":\"Finland - Cup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":408,\"name\":\"Finland - Korisliiga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":4646,\"name\":\"France - All Star Game\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197188,\"name\":\"France - Championnat Pro A Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":415,\"name\":\"France - Championnat Pro B\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":416,\"name\":\"France - Leaders Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":413,\"name\":\"France - Match des Champions\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":453,\"name\":\"International - Tournaments Strasbourg\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5602,\"name\":\"France - Cup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":6891,\"name\":\"England - BBL Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":197191,\"name\":\"Greece - A1 Intermission\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":305,\"name\":\"Croatia - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":445,\"name\":\"Hungary - Division A\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":191728,\"name\":\"Israel - Chance Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":460,\"name\":\"Israel - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":461,\"name\":\"Israel - Premier League Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4530,\"name\":\"Israel - Winner Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":463,\"name\":\"Italy - Tim Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":468,\"name\":\"Italy - Coppa Italia\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":469,\"name\":\"Italy - Coppa Italia Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":470,\"name\":\"Italy - Lega A1 Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":474,\"name\":\"Italy - Lega Nazionale Pallacanestro Gold\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":475,\"name\":\"Italy - Super Coppa\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":197721,\"name\":\"Japan - B League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":478,\"name\":\"Japan - WJB League Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":484,\"name\":\"Lithuania - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":485,\"name\":\"Lithuania - Lietuvos Krepsinio Lyga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":483,\"name\":\"Latvia - LBL Division 1\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":486,\"name\":\"Mexico - Liga Nacional de Baloncesto Profesional\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":5130,\"name\":\"Peru - Superior League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":510,\"name\":\"Philippines - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5608,\"name\":\"Philippines - PBA Governors Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":7066,\"name\":\"Philippines - PBA Philippine Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":511,\"name\":\"Poland - Puchar Polski\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4531,\"name\":\"Poland - Super Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":519,\"name\":\"Romania - All Star Game\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":523,\"name\":\"Romania - Division A Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5392,\"name\":\"Romania - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":536,\"name\":\"Serbia - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":4599,\"name\":\"Russia - Professional Basketball League Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":535,\"name\":\"Russia - Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":531,\"name\":\"Russia - Division A Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":533,\"name\":\"Russia - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":543,\"name\":\"Slovakia - League Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":544,\"name\":\"Slovakia - Extraliga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":574,\"name\":\"Ukraine - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5251,\"name\":\"Uruguay - Liga\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":569,\"name\":\"Turkey - Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":578,\"name\":\"WNBA\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":487,\"name\":\"NBA\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":true,\"allowRoundRobins\":true},{\"id\":5270,\"name\":\"NBA Preseason\",\"homeTeamType\":\"TEAM2\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":198921,\"name\":\"NBA D-League\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5699,\"name\":\"Olympic - Qualifying Tournament Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":197229,\"name\":\"Olympic - Basketball Matches\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":388,\"name\":\"FIBA - Africa Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5912,\"name\":\"FIBA - Asia Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199778,\"name\":\"FIBA - Asia Champions Cup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":393,\"name\":\"FIBA - Asia Championship\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":376,\"name\":\"Europe - Eurobasket Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":192318,\"name\":\"Europe - Eurobasket Qualification Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":375,\"name\":\"FIBA - Eurobasket Men\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":true},{\"id\":199593,\"name\":\"FIBA - AmeriCup Women\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199670,\"name\":\"FIBA - AmeriCup\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":199576,\"name\":\"FIBA - World Cup Europe Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":true,\"allowRoundRobins\":false},{\"id\":199575,\"name\":\"FIBA - World Cup Qualification\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false},{\"id\":5680,\"name\":\"World - International Friendlies\",\"homeTeamType\":\"TEAM1\",\"hasOfferings\":false,\"allowRoundRobins\":false}]}";

            var client = GetLinesClientMock(jsonForTest);
            
            // Act
            var result = await client.V1LeaguesAsync("4");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(250, result.Leagues.Count);
            var abaLeagueAllStar = result.Leagues.Single(x => x.Id == 267);
            Assert.AreEqual("Europe - ABA League Adriatic All Star", abaLeagueAllStar.Name);
            Assert.AreEqual("TEAM1", abaLeagueAllStar.HomeTeamType);
            Assert.IsFalse(abaLeagueAllStar.HasOfferings);
            Assert.IsTrue(abaLeagueAllStar.AllowRoundRobins);
            var leagueFrance = result.Leagues.Single(x => x.Id == 412);
            Assert.AreEqual("France - Ligue Féminine de Basketball", leagueFrance.Name);
            Assert.AreEqual("TEAM1", leagueFrance.HomeTeamType);
            Assert.IsTrue(leagueFrance.HasOfferings);
            Assert.IsFalse(leagueFrance.AllowRoundRobins);
            var nba = result.Leagues.Single(x => x.Id == 487);
            Assert.AreEqual("NBA", nba.Name);
            Assert.AreEqual("TEAM2", nba.HomeTeamType);
            Assert.IsTrue(nba.HasOfferings);
            Assert.IsTrue(nba.AllowRoundRobins);
        }

        [Test]
        public async Task GetAsyncFixturesResponse_WithSports4AndLeague_ReturnAvailableLines()
        {
            // Arrange
            var jsonForTest = "{\"sportId\":4,\"last\":1519677810861,\"league\":[{\"id\":487,\"events\":[{\"id\":820394708,\"starts\":\"2018-02-27T03:05:00Z\",\"home\":\"Sacramento Kings\",\"away\":\"Minnesota Timberwolves\",\"rotNum\":\"719\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":820388947,\"starts\":\"2018-02-27T02:05:00Z\",\"home\":\"Utah Jazz\",\"away\":\"Houston Rockets\",\"rotNum\":\"717\",\"liveStatus\":2,\"status\":\"O\",\"parlayRestriction\":0},{\"id\":820381786,\"starts\":\"2018-02-27T01:05:00Z\",\"home\":\"New Orleans Pelicans\",\"away\":\"Phoenix Suns\",\"rotNum\":\"711\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":820381785,\"starts\":\"2018-02-27T01:05:00Z\",\"home\":\"Oklahoma City Thunder\",\"away\":\"Orlando Magic\",\"rotNum\":\"713\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":820378813,\"starts\":\"2018-02-27T00:35:00Z\",\"home\":\"Toronto Raptors\",\"away\":\"Detroit Pistons\",\"rotNum\":\"701\",\"liveStatus\":2,\"status\":\"O\",\"parlayRestriction\":0},{\"id\":820378812,\"starts\":\"2018-02-27T00:35:00Z\",\"home\":\"New York Knicks\",\"away\":\"Golden State Warriors\",\"rotNum\":\"709\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":820378815,\"starts\":\"2018-02-27T00:35:00Z\",\"home\":\"Atlanta Hawks\",\"away\":\"Los Angeles Lakers\",\"rotNum\":\"707\",\"liveStatus\":2,\"status\":\"O\",\"parlayRestriction\":0},{\"id\":820378814,\"starts\":\"2018-02-27T00:35:00Z\",\"home\":\"Boston Celtics\",\"away\":\"Memphis Grizzlies\",\"rotNum\":\"705\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":822194677,\"starts\":\"2018-02-27T00:30:00Z\",\"home\":\"Brooklyn Nets\",\"away\":\"Chicago Bulls\",\"rotNum\":\"703\",\"liveStatus\":2,\"status\":\"I\",\"parlayRestriction\":0},{\"id\":820385436,\"starts\":\"2018-02-27T01:35:00Z\",\"home\":\"Dallas Mavericks\",\"away\":\"Indiana Pacers\",\"rotNum\":\"715\",\"liveStatus\":2,\"status\":\"O\",\"parlayRestriction\":0}]}]}";

            var client = GetLinesClientMock(jsonForTest);
            
            // Act
            var result = await client.V1FixturesAsync(4, new List<int> {487}, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1519677810861, result.Last);
            Assert.AreEqual(4, result.SportId);
            var league = result.League.Single(x => x.Id == 487);
            Assert.AreEqual(10, league.Events.Count);
            var gameFixture = league.Events.Single(x => x.Id == 820378812);
            Assert.IsNotNull(gameFixture);
            Assert.AreEqual(new DateTimeOffset(2018, 02, 27, 00, 35, 00, TimeSpan.Zero), gameFixture.Starts);
            Assert.AreEqual("New York Knicks", gameFixture.Home);
            Assert.AreEqual("Golden State Warriors", gameFixture.Away);
            Assert.AreEqual("709", gameFixture.RotNum);
            Assert.AreEqual(FixtureV1LiveStatus._2, gameFixture.LiveStatus);
            Assert.AreEqual(FixtureV1Status.I, gameFixture.Status);
            Assert.AreEqual(FixtureV1ParlayRestriction._0, gameFixture.ParlayRestriction);
        }

        [Test]
        public async Task GetAsyncOddsResponse_WithSports4AndLeague487Event820378812_ReturnAvailableOddsInDecimal()
        {
            // Arrange
            var jsonForTest = @"{""sportId"":4,""last"":1519845131241,""leagues"":[{""id"":487,""events"":[{""id"":821011703,""periods"":[{""lineId"":476830290,""number"":0,""cutoff"":""2018-03-01T03:35:00Z"",""spreads"":[{""hdp"":7.5,""away"":1.934,""home"":1.970},{""altLineId"":5804430972,""hdp"":8.5,""away"":2.110,""home"":1.806},{""altLineId"":5804430974,""hdp"":8.0,""away"":2.020,""home"":1.877},{""altLineId"":5804430976,""hdp"":7.0,""away"":1.840,""home"":2.060},{""altLineId"":5804430978,""hdp"":6.5,""away"":1.775,""home"":2.160}],""totals"":[{""points"":226.5,""over"":1.925,""under"":1.980},{""altLineId"":5804430971,""points"":225.0,""over"":1.793,""under"":2.110},{""altLineId"":5804430973,""points"":225.5,""over"":1.833,""under"":2.060},{""altLineId"":5804430975,""points"":226.0,""over"":1.877,""under"":2.020},{""altLineId"":5804430977,""points"":227.0,""over"":1.961,""under"":1.925},{""altLineId"":5804430979,""points"":227.5,""over"":2.000,""under"":1.877},{""altLineId"":5804430981,""points"":228.0,""over"":2.040,""under"":1.833}],""moneyline"":{""away"":1.303,""home"":3.890},""teamTotal"":{""away"":{""points"":117.5,""over"":2.000,""under"":1.854},""home"":{""points"":109.5,""over"":1.925,""under"":1.925}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476830294,""number"":1,""cutoff"":""2018-03-01T03:34:34Z"",""spreads"":[{""hdp"":4.5,""away"":1.952,""home"":1.934},{""altLineId"":5804431019,""hdp"":5.5,""away"":2.130,""home"":1.775},{""altLineId"":5804431021,""hdp"":5.0,""away"":2.040,""home"":1.847},{""altLineId"":5804431023,""hdp"":4.0,""away"":1.862,""home"":2.020},{""altLineId"":5804431025,""hdp"":3.5,""away"":1.793,""home"":2.110}],""totals"":[{""points"":116.5,""over"":1.961,""under"":1.925},{""altLineId"":5804431018,""points"":115.0,""over"":1.735,""under"":2.190},{""altLineId"":5804431020,""points"":115.5,""over"":1.806,""under"":2.080},{""altLineId"":5804431022,""points"":116.0,""over"":1.869,""under"":2.000},{""altLineId"":5804431024,""points"":117.0,""over"":2.040,""under"":1.847},{""altLineId"":5804431026,""points"":117.5,""over"":2.120,""under"":1.793},{""altLineId"":5804431028,""points"":118.0,""over"":2.210,""under"":1.724}],""moneyline"":{""away"":1.448,""home"":2.940},""teamTotal"":{""away"":{""points"":60.5,""over"":1.961,""under"":1.884},""home"":{""points"":56.0,""over"":1.925,""under"":1.925}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476830298,""number"":3,""cutoff"":""2018-03-01T03:34:34Z"",""spreads"":[{""hdp"":3.0,""away"":1.952,""home"":1.934}],""totals"":[{""points"":58.5,""over"":1.900,""under"":1.990}],""moneyline"":{""away"":1.483,""home"":2.800},""teamTotal"":{""away"":{""points"":31.0,""over"":1.990,""under"":1.862},""home"":{""points"":28.0,""over"":1.961,""under"":1.884}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820981678,""periods"":[{""lineId"":476825156,""number"":0,""cutoff"":""2018-03-01T00:05:00Z"",""spreads"":[{""hdp"":-1.5,""away"":1.990,""home"":1.917},{""altLineId"":5804367852,""hdp"":-2.5,""away"":1.833,""home"":2.080},{""altLineId"":5804367854,""hdp"":-2.0,""away"":1.900,""home"":1.990},{""altLineId"":5804367856,""hdp"":-1.0,""away"":2.040,""home"":1.862},{""altLineId"":5804367858,""hdp"":0.0,""away"":2.090,""home"":1.826}],""totals"":[{""points"":207.0,""over"":1.884,""under"":2.020},{""altLineId"":5804367851,""points"":205.5,""over"":1.763,""under"":2.150},{""altLineId"":5804367853,""points"":206.0,""over"":1.800,""under"":2.100},{""altLineId"":5804367855,""points"":206.5,""over"":1.840,""under"":2.060},{""altLineId"":5804367857,""points"":207.5,""over"":1.917,""under"":1.961},{""altLineId"":5804367859,""points"":208.0,""over"":1.952,""under"":1.909},{""altLineId"":5804367861,""points"":208.5,""over"":1.990,""under"":1.862}],""moneyline"":{""away"":2.080,""home"":1.840},""teamTotal"":{""away"":{""points"":103.0,""over"":1.952,""under"":1.900},""home"":{""points"":104.5,""over"":1.847,""under"":2.010}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476825074,""number"":1,""cutoff"":""2018-03-01T00:04:46Z"",""spreads"":[{""hdp"":-0.5,""away"":2.000,""home"":1.892},{""altLineId"":5804366781,""hdp"":-1.5,""away"":1.833,""home"":2.060},{""altLineId"":5804366783,""hdp"":-1.0,""away"":1.900,""home"":1.970},{""altLineId"":5804366785,""hdp"":0.0,""away"":2.090,""home"":1.806},{""altLineId"":5804366787,""hdp"":0.5,""away"":2.200,""home"":1.735}],""totals"":[{""points"":106.0,""over"":1.952,""under"":1.934},{""altLineId"":5804366780,""points"":104.5,""over"":1.757,""under"":2.170},{""altLineId"":5804366782,""points"":105.0,""over"":1.813,""under"":2.110},{""altLineId"":5804366784,""points"":105.5,""over"":1.884,""under"":2.010},{""altLineId"":5804366786,""points"":106.5,""over"":2.030,""under"":1.854},{""altLineId"":5804366788,""points"":107.0,""over"":2.120,""under"":1.787},{""altLineId"":5804366790,""points"":107.5,""over"":2.220,""under"":1.729}],""moneyline"":{""away"":2.080,""home"":1.819},""teamTotal"":{""away"":{""points"":52.0,""over"":1.847,""under"":2.010},""home"":{""points"":53.5,""over"":1.934,""under"":1.917}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476825075,""number"":3,""cutoff"":""2018-03-01T00:04:46Z"",""spreads"":[{""hdp"":-0.5,""away"":2.000,""home"":1.892}],""totals"":[{""points"":53.5,""over"":1.934,""under"":1.961}],""moneyline"":{""away"":2.130,""home"":1.787},""teamTotal"":{""away"":{""points"":26.5,""over"":1.970,""under"":1.877},""home"":{""points"":27.0,""over"":1.884,""under"":1.961}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820981675,""periods"":[{""lineId"":476811960,""number"":0,""cutoff"":""2018-03-01T00:05:00Z"",""spreads"":[{""hdp"":9.5,""away"":1.980,""home"":1.925},{""altLineId"":5804193734,""hdp"":10.5,""away"":2.150,""home"":1.781},{""altLineId"":5804193736,""hdp"":10.0,""away"":2.060,""home"":1.840},{""altLineId"":5804193738,""hdp"":9.0,""away"":1.892,""home"":2.010},{""altLineId"":5804193740,""hdp"":8.5,""away"":1.819,""home"":2.100}],""totals"":[{""points"":221.0,""over"":1.970,""under"":1.934},{""altLineId"":5804193733,""points"":219.5,""over"":1.826,""under"":2.060},{""altLineId"":5804193735,""points"":220.0,""over"":1.869,""under"":2.010},{""altLineId"":5804193737,""points"":220.5,""over"":1.917,""under"":1.970},{""altLineId"":5804193739,""points"":221.5,""over"":2.010,""under"":1.884},{""altLineId"":5804193741,""points"":222.0,""over"":2.050,""under"":1.840},{""altLineId"":5804193743,""points"":222.5,""over"":2.090,""under"":1.800}],""moneyline"":{""away"":1.229,""home"":4.730},""teamTotal"":{""away"":{""points"":114.0,""over"":1.813,""under"":2.050},""home"":{""points"":105.5,""over"":1.909,""under"":1.943}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476811961,""number"":1,""cutoff"":""2018-03-01T00:04:50Z"",""spreads"":[{""hdp"":5.5,""away"":1.990,""home"":1.900},{""altLineId"":5804193749,""hdp"":6.5,""away"":2.180,""home"":1.746},{""altLineId"":5804193751,""hdp"":6.0,""away"":2.080,""home"":1.813},{""altLineId"":5804193753,""hdp"":5.0,""away"":1.892,""home"":1.980},{""altLineId"":5804193755,""hdp"":4.5,""away"":1.826,""home"":2.070}],""totals"":[{""points"":111.5,""over"":1.943,""under"":1.943},{""altLineId"":5804193748,""points"":110.0,""over"":1.729,""under"":2.200},{""altLineId"":5804193750,""points"":110.5,""over"":1.800,""under"":2.100},{""altLineId"":5804193752,""points"":111.0,""over"":1.862,""under"":2.020},{""altLineId"":5804193754,""points"":112.0,""over"":2.020,""under"":1.862},{""altLineId"":5804193756,""points"":112.5,""over"":2.100,""under"":1.800},{""altLineId"":5804193758,""points"":113.0,""over"":2.200,""under"":1.729}],""moneyline"":{""away"":1.374,""home"":3.300},""teamTotal"":{""away"":{""points"":58.0,""over"":1.877,""under"":1.970},""home"":{""points"":53.0,""over"":1.884,""under"":1.961}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476811963,""number"":3,""cutoff"":""2018-03-01T00:04:50Z"",""spreads"":[{""hdp"":3.0,""away"":1.900,""home"":1.990}],""totals"":[{""points"":56.5,""over"":1.917,""under"":1.970}],""moneyline"":{""away"":1.440,""home"":2.980},""teamTotal"":{""away"":{""points"":29.5,""over"":1.840,""under"":2.020},""home"":{""points"":26.5,""over"":1.862,""under"":1.990}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820990112,""periods"":[{""lineId"":476808525,""number"":0,""cutoff"":""2018-03-01T01:05:00Z"",""spreads"":[{""hdp"":8.5,""away"":1.884,""home"":2.020},{""altLineId"":5804155276,""hdp"":9.5,""away"":2.050,""home"":1.854},{""altLineId"":5804155278,""hdp"":9.0,""away"":1.961,""home"":1.925},{""altLineId"":5804155280,""hdp"":8.0,""away"":1.800,""home"":2.120},{""altLineId"":5804155282,""hdp"":7.5,""away"":1.735,""home"":2.220}],""totals"":[{""points"":225.5,""over"":1.961,""under"":1.943},{""altLineId"":5804155275,""points"":224.0,""over"":1.833,""under"":2.090},{""altLineId"":5804155277,""points"":224.5,""over"":1.877,""under"":2.040},{""altLineId"":5804155279,""points"":225.0,""over"":1.917,""under"":1.990},{""altLineId"":5804155281,""points"":226.0,""over"":2.010,""under"":1.900},{""altLineId"":5804155283,""points"":226.5,""over"":2.050,""under"":1.862},{""altLineId"":5804155285,""points"":227.0,""over"":2.100,""under"":1.826}],""moneyline"":{""away"":1.250,""home"":4.460},""teamTotal"":{""away"":{""points"":117.5,""over"":1.925,""under"":1.925},""home"":{""points"":108.0,""over"":1.909,""under"":1.943}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476809991,""number"":1,""cutoff"":""2018-03-01T01:04:23Z"",""spreads"":[{""hdp"":5.0,""away"":1.970,""home"":1.917},{""altLineId"":5804170487,""hdp"":6.0,""away"":2.160,""home"":1.757},{""altLineId"":5804170489,""hdp"":5.5,""away"":2.060,""home"":1.840},{""altLineId"":5804170491,""hdp"":4.5,""away"":1.884,""home"":2.000},{""altLineId"":5804170493,""hdp"":4.0,""away"":1.800,""home"":2.100}],""totals"":[{""points"":115.5,""over"":1.869,""under"":2.020},{""altLineId"":5804170486,""points"":114.0,""over"":1.671,""under"":2.300},{""altLineId"":5804170488,""points"":114.5,""over"":1.740,""under"":2.190},{""altLineId"":5804170490,""points"":115.0,""over"":1.793,""under"":2.100},{""altLineId"":5804170492,""points"":116.0,""over"":1.934,""under"":1.934},{""altLineId"":5804170494,""points"":116.5,""over"":2.010,""under"":1.869},{""altLineId"":5804170496,""points"":117.0,""over"":2.100,""under"":1.793}],""moneyline"":{""away"":1.406,""home"":3.140},""teamTotal"":{""away"":{""points"":60.5,""over"":1.980,""under"":1.869},""home"":{""points"":55.5,""over"":1.909,""under"":1.943}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476810001,""number"":3,""cutoff"":""2018-03-01T01:04:23Z"",""spreads"":[{""hdp"":3.0,""away"":1.943,""home"":1.943}],""totals"":[{""points"":58.5,""over"":1.925,""under"":1.961}],""moneyline"":{""away"":1.473,""home"":2.840},""teamTotal"":{""away"":{""points"":30.5,""over"":1.869,""under"":1.980},""home"":{""points"":28.0,""over"":1.980,""under"":1.869}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820986038,""periods"":[{""lineId"":476821059,""number"":0,""cutoff"":""2018-03-01T00:35:00Z"",""spreads"":[{""hdp"":-7.5,""away"":1.970,""home"":1.934},{""altLineId"":5804315566,""hdp"":-8.5,""away"":1.806,""home"":2.110},{""altLineId"":5804315568,""hdp"":-8.0,""away"":1.877,""home"":2.020},{""altLineId"":5804315570,""hdp"":-7.0,""away"":2.060,""home"":1.840},{""altLineId"":5804315572,""hdp"":-6.5,""away"":2.160,""home"":1.775}],""totals"":[{""points"":209.0,""over"":1.917,""under"":1.990},{""altLineId"":5804315565,""points"":207.5,""over"":1.787,""under"":2.120},{""altLineId"":5804315567,""points"":208.0,""over"":1.826,""under"":2.070},{""altLineId"":5804315569,""points"":208.5,""over"":1.869,""under"":2.030},{""altLineId"":5804315571,""points"":209.5,""over"":1.952,""under"":1.934},{""altLineId"":5804315573,""points"":210.0,""over"":1.990,""under"":1.884},{""altLineId"":5804315575,""points"":210.5,""over"":2.030,""under"":1.840}],""moneyline"":{""away"":3.950,""home"":1.296},""teamTotal"":{""away"":{""points"":100.0,""over"":1.833,""under"":2.030},""home"":{""points"":108.5,""over"":1.934,""under"":1.917}},""maxSpread"":15000.00,""maxTotal"":5000.00,""maxMoneyline"":7500.00,""maxTeamTotal"":2000.00},{""lineId"":476822370,""number"":1,""cutoff"":""2018-03-01T00:34:14Z"",""spreads"":[{""hdp"":-4.5,""away"":1.943,""home"":1.943},{""altLineId"":5804331529,""hdp"":-5.5,""away"":1.787,""home"":2.120},{""altLineId"":5804331531,""hdp"":-5.0,""away"":1.854,""home"":2.030},{""altLineId"":5804331533,""hdp"":-4.0,""away"":2.030,""home"":1.854},{""altLineId"":5804331535,""hdp"":-3.5,""away"":2.120,""home"":1.787}],""totals"":[{""points"":106.5,""over"":1.892,""under"":1.990},{""altLineId"":5804331528,""points"":105.0,""over"":1.680,""under"":2.290},{""altLineId"":5804331530,""points"":105.5,""over"":1.751,""under"":2.180},{""altLineId"":5804331532,""points"":106.0,""over"":1.806,""under"":2.070},{""altLineId"":5804331534,""points"":107.0,""over"":1.970,""under"":1.900},{""altLineId"":5804331536,""points"":107.5,""over"":2.050,""under"":1.862},{""altLineId"":5804331538,""points"":108.0,""over"":2.130,""under"":1.781}],""moneyline"":{""away"":2.990,""home"":1.438},""teamTotal"":{""away"":{""points"":50.5,""over"":1.819,""under"":2.040},""home"":{""points"":55.5,""over"":1.884,""under"":1.961}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476821582,""number"":3,""cutoff"":""2018-03-01T00:34:14Z"",""spreads"":[{""hdp"":-3.0,""away"":1.934,""home"":1.952}],""totals"":[{""points"":53.5,""over"":1.884,""under"":2.010}],""moneyline"":{""away"":2.800,""home"":1.487},""teamTotal"":{""away"":{""points"":25.5,""over"":1.961,""under"":1.884},""home"":{""points"":28.5,""over"":1.952,""under"":1.900}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820986036,""periods"":[{""lineId"":476825588,""number"":0,""cutoff"":""2018-03-01T00:35:00Z"",""spreads"":[{""hdp"":5.0,""away"":1.952,""home"":1.952},{""altLineId"":5804373500,""hdp"":6.0,""away"":2.150,""home"":1.787},{""altLineId"":5804373502,""hdp"":5.5,""away"":2.040,""home"":1.869},{""altLineId"":5804373504,""hdp"":4.5,""away"":1.869,""home"":2.040},{""altLineId"":5804373506,""hdp"":4.0,""away"":1.800,""home"":2.120}],""totals"":[{""points"":213.5,""over"":1.980,""under"":1.925},{""altLineId"":5804373499,""points"":212.0,""over"":1.833,""under"":2.050},{""altLineId"":5804373501,""points"":212.5,""over"":1.877,""under"":2.000},{""altLineId"":5804373503,""points"":213.0,""over"":1.925,""under"":1.961},{""altLineId"":5804373505,""points"":214.0,""over"":2.020,""under"":1.877},{""altLineId"":5804373507,""points"":214.5,""over"":2.060,""under"":1.833},{""altLineId"":5804373509,""points"":215.0,""over"":2.100,""under"":1.793}],""moneyline"":{""away"":1.507,""home"":2.770},""teamTotal"":{""away"":{""points"":108.0,""over"":1.806,""under"":2.060},""home"":{""points"":104.0,""over"":1.925,""under"":1.925}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476825589,""number"":1,""cutoff"":""2018-03-01T00:34:55Z"",""spreads"":[{""hdp"":2.5,""away"":1.952,""home"":1.934},{""altLineId"":5804373515,""hdp"":3.5,""away"":2.130,""home"":1.781},{""altLineId"":5804373517,""hdp"":3.0,""away"":2.040,""home"":1.847},{""altLineId"":5804373519,""hdp"":2.0,""away"":1.862,""home"":2.020},{""altLineId"":5804373521,""hdp"":1.5,""away"":1.793,""home"":2.110}],""totals"":[{""points"":109.0,""over"":1.917,""under"":1.970},{""altLineId"":5804373514,""points"":107.5,""over"":1.724,""under"":2.220},{""altLineId"":5804373516,""points"":108.0,""over"":1.781,""under"":2.140},{""altLineId"":5804373518,""points"":108.5,""over"":1.847,""under"":2.050},{""altLineId"":5804373520,""points"":109.5,""over"":2.000,""under"":1.892},{""altLineId"":5804373522,""points"":110.0,""over"":2.080,""under"":1.826},{""altLineId"":5804373524,""points"":110.5,""over"":2.160,""under"":1.763}],""moneyline"":{""away"":1.609,""home"":2.440},""teamTotal"":{""away"":{""points"":54.5,""over"":1.709,""under"":2.210},""home"":{""points"":53.5,""over"":1.952,""under"":1.900}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476825596,""number"":3,""cutoff"":""2018-03-01T00:34:55Z"",""spreads"":[{""hdp"":1.5,""away"":1.925,""home"":1.970}],""totals"":[{""points"":55.0,""over"":1.917,""under"":1.970}],""moneyline"":{""away"":1.671,""home"":2.320},""teamTotal"":{""away"":{""points"":28.0,""over"":1.854,""under"":2.000},""home"":{""points"":27.0,""over"":1.990,""under"":1.862}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820994312,""periods"":[{""lineId"":476825445,""number"":0,""cutoff"":""2018-03-01T01:35:00Z"",""spreads"":[{""hdp"":5.0,""away"":1.990,""home"":1.917},{""altLineId"":5804371548,""hdp"":6.0,""away"":2.190,""home"":1.757},{""altLineId"":5804371550,""hdp"":5.5,""away"":2.080,""home"":1.840},{""altLineId"":5804371552,""hdp"":4.5,""away"":1.900,""home"":2.000},{""altLineId"":5804371554,""hdp"":4.0,""away"":1.833,""home"":2.080}],""totals"":[{""points"":209.5,""over"":1.980,""under"":1.925},{""altLineId"":5804371547,""points"":208.0,""over"":1.847,""under"":2.070},{""altLineId"":5804371549,""points"":208.5,""over"":1.892,""under"":2.020},{""altLineId"":5804371551,""points"":209.0,""over"":1.934,""under"":1.970},{""altLineId"":5804371553,""points"":210.0,""over"":2.030,""under"":1.884},{""altLineId"":5804371555,""points"":210.5,""over"":2.070,""under"":1.847},{""altLineId"":5804371557,""points"":211.0,""over"":2.120,""under"":1.813}],""moneyline"":{""away"":1.512,""home"":2.750},""teamTotal"":{""away"":{""points"":106.5,""over"":1.884,""under"":1.961},""home"":{""points"":102.0,""over"":1.909,""under"":1.943}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476825450,""number"":1,""cutoff"":""2018-03-01T01:34:30Z"",""spreads"":[{""hdp"":2.5,""away"":1.943,""home"":1.943},{""altLineId"":5804371627,""hdp"":3.5,""away"":2.120,""home"":1.787},{""altLineId"":5804371629,""hdp"":3.0,""away"":2.030,""home"":1.854},{""altLineId"":5804371631,""hdp"":2.0,""away"":1.854,""home"":2.030},{""altLineId"":5804371633,""hdp"":1.5,""away"":1.787,""home"":2.120}],""totals"":[{""points"":106.5,""over"":1.934,""under"":1.952},{""altLineId"":5804371626,""points"":105.0,""over"":1.724,""under"":2.210},{""altLineId"":5804371628,""points"":105.5,""over"":1.793,""under"":2.110},{""altLineId"":5804371630,""points"":106.0,""over"":1.854,""under"":2.030},{""altLineId"":5804371632,""points"":107.0,""over"":2.010,""under"":1.869},{""altLineId"":5804371634,""points"":107.5,""over"":2.090,""under"":1.806},{""altLineId"":5804371636,""points"":108.0,""over"":2.190,""under"":1.735}],""moneyline"":{""away"":1.609,""home"":2.450},""teamTotal"":{""away"":{""points"":55.5,""over"":2.190,""under"":1.719},""home"":{""points"":52.0,""over"":1.917,""under"":1.934}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476825451,""number"":3,""cutoff"":""2018-03-01T01:34:30Z"",""spreads"":[{""hdp"":2.0,""away"":1.943,""home"":1.943}],""totals"":[{""points"":53.5,""over"":1.934,""under"":1.952}],""moneyline"":{""away"":1.613,""home"":2.440},""teamTotal"":{""away"":{""points"":28.0,""over"":2.010,""under"":1.847},""home"":{""points"":26.0,""over"":1.990,""under"":1.862}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820994310,""periods"":[{""lineId"":476810689,""number"":0,""cutoff"":""2018-03-01T01:35:00Z"",""spreads"":[{""hdp"":-5.0,""away"":1.909,""home"":2.000},{""altLineId"":5804178766,""hdp"":-6.0,""away"":1.746,""home"":2.200},{""altLineId"":5804178768,""hdp"":-5.5,""away"":1.833,""home"":2.090},{""altLineId"":5804178770,""hdp"":-4.5,""away"":1.990,""home"":1.909},{""altLineId"":5804178772,""hdp"":-4.0,""away"":2.070,""home"":1.840}],""totals"":[{""points"":218.5,""over"":1.884,""under"":2.020},{""altLineId"":5804178765,""points"":217.0,""over"":1.763,""under"":2.150},{""altLineId"":5804178767,""points"":217.5,""over"":1.800,""under"":2.100},{""altLineId"":5804178769,""points"":218.0,""over"":1.840,""under"":2.060},{""altLineId"":5804178771,""points"":219.0,""over"":1.917,""under"":1.961},{""altLineId"":5804178773,""points"":219.5,""over"":1.952,""under"":1.909},{""altLineId"":5804178775,""points"":220.0,""over"":1.990,""under"":1.862}],""moneyline"":{""away"":2.710,""home"":1.526},""teamTotal"":{""away"":{""points"":107.0,""over"":1.917,""under"":1.934},""home"":{""points"":112.0,""over"":1.934,""under"":1.917}},""maxSpread"":20000.00,""maxTotal"":5000.00,""maxMoneyline"":10000.00,""maxTeamTotal"":2000.00},{""lineId"":476810653,""number"":1,""cutoff"":""2018-03-01T01:34:27Z"",""spreads"":[{""hdp"":-2.5,""away"":1.952,""home"":1.934},{""altLineId"":5804178297,""hdp"":-3.5,""away"":1.793,""home"":2.110},{""altLineId"":5804178299,""hdp"":-3.0,""away"":1.862,""home"":2.020},{""altLineId"":5804178301,""hdp"":-2.0,""away"":2.040,""home"":1.847},{""altLineId"":5804178303,""hdp"":-1.5,""away"":2.130,""home"":1.781}],""totals"":[{""points"":112.0,""over"":1.892,""under"":2.000},{""altLineId"":5804178296,""points"":110.5,""over"":1.704,""under"":2.260},{""altLineId"":5804178298,""points"":111.0,""over"":1.751,""under"":2.170},{""altLineId"":5804178300,""points"":111.5,""over"":1.826,""under"":2.080},{""altLineId"":5804178302,""points"":112.5,""over"":1.970,""under"":1.917},{""altLineId"":5804178304,""points"":113.0,""over"":2.050,""under"":1.847},{""altLineId"":5804178306,""points"":113.5,""over"":2.130,""under"":1.787}],""moneyline"":{""away"":2.470,""home"":1.598},""teamTotal"":{""away"":{""points"":54.5,""over"":1.877,""under"":1.970},""home"":{""points"":57.5,""over"":1.934,""under"":1.917}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00},{""lineId"":476810659,""number"":3,""cutoff"":""2018-03-01T01:34:27Z"",""spreads"":[{""hdp"":-1.5,""away"":1.961,""home"":1.925}],""totals"":[{""points"":56.5,""over"":1.884,""under"":2.010}],""moneyline"":{""away"":2.320,""home"":1.671},""teamTotal"":{""away"":{""points"":27.5,""over"":1.917,""under"":1.934},""home"":{""points"":29.0,""over"":1.869,""under"":1.980}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]},{""id"":820990111,""periods"":[{""lineId"":476821760,""number"":0,""cutoff"":""2018-03-01T01:05:00Z"",""spreads"":[{""hdp"":-2.5,""away"":1.925,""home"":1.980},{""altLineId"":5804323890,""hdp"":-3.5,""away"":1.781,""home"":2.160},{""altLineId"":5804323892,""hdp"":-3.0,""away"":1.840,""home"":2.060},{""altLineId"":5804323894,""hdp"":-2.0,""away"":2.000,""home"":1.892},{""altLineId"":5804323896,""hdp"":-1.5,""away"":2.090,""home"":1.826}],""totals"":[{""points"":217.0,""over"":1.943,""under"":1.961},{""altLineId"":5804323889,""points"":215.5,""over"":1.806,""under"":2.090},{""altLineId"":5804323891,""points"":216.0,""over"":1.847,""under"":2.040},{""altLineId"":5804323893,""points"":216.5,""over"":1.892,""under"":2.000},{""altLineId"":5804323895,""points"":217.5,""over"":1.980,""under"":1.909},{""altLineId"":5804323897,""points"":218.0,""over"":2.020,""under"":1.862},{""altLineId"":5804323899,""points"":218.5,""over"":2.060,""under"":1.819}],""moneyline"":{""away"":2.180,""home"":1.769},""teamTotal"":{""away"":{""points"":106.5,""over"":1.840,""under"":2.020},""home"":{""points"":109.5,""over"":1.909,""under"":1.943}},""maxSpread"":5000.00,""maxTotal"":2000.00,""maxMoneyline"":2500.00,""maxTeamTotal"":1000.00},{""lineId"":476821763,""number"":1,""cutoff"":""2018-03-01T01:04:18Z"",""spreads"":[{""hdp"":-1.0,""away"":1.990,""home"":1.900},{""altLineId"":5804323937,""hdp"":-2.0,""away"":1.819,""home"":2.080},{""altLineId"":5804323939,""hdp"":-1.5,""away"":1.900,""home"":1.990},{""altLineId"":5804323941,""hdp"":-0.5,""away"":2.080,""home"":1.826},{""altLineId"":5804323943,""hdp"":0.0,""away"":2.190,""home"":1.746}],""totals"":[{""points"":105.5,""over"":1.990,""under"":1.909},{""altLineId"":5804323936,""points"":104.0,""over"":1.763,""under"":2.160},{""altLineId"":5804323938,""points"":104.5,""over"":1.833,""under"":2.040},{""altLineId"":5804323940,""points"":105.0,""over"":1.909,""under"":1.980},{""altLineId"":5804323942,""points"":106.0,""over"":2.070,""under"":1.833},{""altLineId"":5804323944,""points"":106.5,""over"":2.150,""under"":1.775},{""altLineId"":5804323946,""points"":107.0,""over"":2.260,""under"":1.704}],""moneyline"":{""away"":2.160,""home"":1.769},""teamTotal"":{""away"":{""points"":51.5,""over"":1.862,""under"":1.990},""home"":{""points"":53.5,""over"":1.961,""under"":1.884}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":1500.00,""maxTeamTotal"":1000.00},{""lineId"":476821766,""number"":3,""cutoff"":""2018-03-01T01:04:18Z"",""spreads"":[{""hdp"":-0.5,""away"":2.000,""home"":1.892}],""totals"":[{""points"":53.0,""over"":1.909,""under"":1.990}],""moneyline"":{""away"":2.120,""home"":1.793},""teamTotal"":{""away"":{""points"":26.0,""over"":1.877,""under"":1.970},""home"":{""points"":27.0,""over"":1.943,""under"":1.909}},""maxSpread"":3000.00,""maxTotal"":1000.00,""maxMoneyline"":2000.00,""maxTeamTotal"":1000.00}]}]}]}";
            
            var client = GetLinesClientMock(jsonForTest);

            // Act
            var result = await client.V3OddsAsync(4, new List<int> {487}, Ps3838.Lines.OddsFormat2.Decimal, 1519846285090, null,
                new List<long> {820378812}, "EUR");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1519845131241, result.Last);
            Assert.AreEqual(4, result.SportId);
            var league = result.Leagues.Single(x => x.Id == 487);
            Assert.AreEqual(9, league.Events.Count);
            var gameFixture = league.Events.Single(x => x.Id == 821011703);
            Assert.IsNotNull(gameFixture);
            Assert.AreEqual(3, gameFixture.Periods.Count);
            var line = gameFixture.Periods.Single(x => x.LineId == 476830290);
            Assert.AreEqual(0, line.Number);
            Assert.AreEqual(new DateTimeOffset(2018, 03, 01, 03, 35, 00, TimeSpan.Zero), line.Cutoff);
            Assert.AreEqual(5, line.Spreads.Count);
            var spread = line.Spreads.Single(x => x.Hdp == 8.5);
            Assert.AreEqual(5804430972, spread.AltLineId);
            Assert.AreEqual(2.110, spread.Away);
            Assert.AreEqual(1.806, spread.Home);
            Assert.AreEqual(7, line.Totals.Count);
            var total = line.Totals.Single(x => x.Points == 225.0);
            Assert.AreEqual(5804430971, total.AltLineId);
            Assert.AreEqual(1.793, total.Over);
            Assert.AreEqual(2.110, total.Under);
            Assert.AreEqual(1.303, line.Moneyline.Away);
            Assert.AreEqual(3.890, line.Moneyline.Home);
            Assert.AreEqual(117.5, line.TeamTotal.Away.Points);
            Assert.AreEqual(2.000, line.TeamTotal.Away.Over);
            Assert.AreEqual(1.854, line.TeamTotal.Away.Under);
            Assert.AreEqual(109.5, line.TeamTotal.Home.Points);
            Assert.AreEqual(1.925, line.TeamTotal.Home.Over);
            Assert.AreEqual(1.925, line.TeamTotal.Home.Under);
            Assert.AreEqual(20000.00, line.MaxSpread);
            Assert.AreEqual(5000.00, line.MaxTotal);
            Assert.AreEqual(10000.00, line.MaxMoneyline);
            Assert.AreEqual(2000.00, line.MaxTeamTotal);
        }

        [Test]
        public async Task StraightV1Async_WithPlaceBetRequest_ReturnPlaceBetResponseV1()
        {
            // Arrange
            var jsonForTest = "{\"status\":\"ACCEPTED\",\"errorCode\":null,\"betId\":761875754,\"uniqueRequestId\":\"0865697a-6e4e-49dc-b77a-190e71a57bc8\",\"betterLineWasAccepted\":false,\"price\":-147.0}";
            var placeBetRequest = new PlaceBetRequest();
            placeBetRequest.AcceptBetterLine = true;
            placeBetRequest.BetType = PlaceBetRequestBetType.MONEYLINE;
            placeBetRequest.EventId = 821011703;
            placeBetRequest.LineId = 476830290;
            placeBetRequest.SportId = 4;
            placeBetRequest.Team = PlaceBetRequestTeam.TEAM1;
            placeBetRequest.WinRiskStake = PlaceBetRequestWinRiskStake.RISK;
            placeBetRequest.Stake = 5.00;
            placeBetRequest.OddsFormat = OddsFormat.DECIMAL;

            var client = GetBetsClientMock(jsonForTest);

            // Act
#pragma warning disable 612
            var result = await client.V1BetsPlaceAsync(placeBetRequest);
#pragma warning restore 612

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PlaceBetResponseStatus.ACCEPTED, result.Status);
            Assert.AreEqual(null, result.ErrorCode);
            Assert.AreEqual("0865697a-6e4e-49dc-b77a-190e71a57bc8", result.UniqueRequestId.ToString());
            Assert.AreEqual(761875754, result.BetId);
            Assert.AreEqual(false, result.BetterLineWasAccepted);
            Assert.AreEqual(-147.0, result.Price);
        }

        [Test]
        public async Task ParlayAsync_WithPlaceBetRequest_ReturnPlaceParlayBetResponse()
        {
            // Arrange
            var jsonForTest = "{\"status\":\"ACCEPTED\",\"errorCode\":null,\"betId\":759629245,\"uniqueRequestId\":\"D5CC50E4-284D-4D50-8D49-429BDC4F2A48\",\"roundRobinOptionWithOdds\":[{\"roundRobinOption\":\"Parlay\",\"odds\":682,\"unroundedDecimalOdds\":7.8231}],\"maxRiskStake\":0,\"minRiskStake\":0,\"validLegs\":[{\"status\":\"VALID\",\"errorCode\":null,\"legId\":\"10924E23-A2FE-4317-BFFD-80504675F554\",\"lineId\":419715968,\"altLineId\":null,\"price\":167,\"correlatedLegs\":[\"10924E23-A2FE-4317-BFFD-80504675F554\"]}],\"invalidLegs\":[{\"status\":\"VALID\",\"errorCode\":null,\"legId\":\"10924E23-A2FE-4317-BFFD-80504675F554\",\"lineId\":419715968,\"altLineId\":null,\"price\":167,\"correlatedLegs\":[\"10924E23-A2FE-4317-BFFD-80504675F554\"]}]}";
            var placeBetRequest = new PlaceParlayBetRequest();
            placeBetRequest.AcceptBetterLine = true;
            placeBetRequest.RiskAmount = 10.5;
            placeBetRequest.OddsFormat = OddsFormat.DECIMAL;
            placeBetRequest.RoundRobinOptions = new ObservableCollection<RoundRobinOptions> {RoundRobinOptions.Parlay};
            placeBetRequest.Legs = new ObservableCollection<ParlayLegRequest>
            {
                new ParlayLegRequest
                {
                    LineId = 419715968,
                    AltLineId = null,
                    Pitcher1MustStart = false,
                    Pitcher2MustStart = false,
                    SportId = 29,
                    EventId = 758023991,
                    PeriodNumber = 0,
                    LegBetType = ParlayLegRequestLegBetType.MONEYLINE,
                    Team = "TEAM1",
                    Side = null
                }
            };

            var client = GetBetsClientMock(jsonForTest);

            // Act
            var result = await client.V1BetsParlayAsync(placeBetRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(PlaceParlayBetResponseStatus.ACCEPTED, result.Status);
            Assert.AreEqual(null, result.ErrorCode);
            Assert.AreEqual("d5cc50e4-284d-4d50-8d49-429bdc4f2a48", result.UniqueRequestId.ToString());
            Assert.AreEqual(759629245, result.BetId);
            Assert.AreEqual(0.0, result.MaxRiskStake);
            Assert.AreEqual(0.0, result.MinRiskStake);
            Assert.AreEqual(RoundRobinOptionWithOddsRoundRobinOption.Parlay, result.RoundRobinOptionWithOdds.Single().RoundRobinOption);
            Assert.AreEqual(682, result.RoundRobinOptionWithOdds.Single().Odds);
            Assert.AreEqual(7.8231, result.RoundRobinOptionWithOdds.Single().UnroundedDecimalOdds);
            Assert.AreEqual(ParlayLegResponseStatus.VALID, result.ValidLegs.Single().Status);
            Assert.AreEqual(null, result.ValidLegs.Single().ErrorCode);
            Assert.AreEqual("10924E23-A2FE-4317-BFFD-80504675F554", result.ValidLegs.Single().LegId.ToString().ToUpper());
            Assert.AreEqual(419715968, result.ValidLegs.Single().LineId);
            Assert.AreEqual(null, result.ValidLegs.Single().AltLineId);
            Assert.AreEqual(167, result.ValidLegs.Single().Price);
            Assert.AreEqual("10924E23-A2FE-4317-BFFD-80504675F554", result.ValidLegs.Single().CorrelatedLegs.Single().ToString().ToUpper());
            Assert.AreEqual(ParlayLegResponseStatus.VALID, result.InvalidLegs.Single().Status);
            Assert.AreEqual(null, result.InvalidLegs.Single().ErrorCode);
            Assert.AreEqual("10924E23-A2FE-4317-BFFD-80504675F554", result.InvalidLegs.Single().LegId.ToString().ToUpper());
            Assert.AreEqual(419715968, result.InvalidLegs.Single().LineId);
            Assert.AreEqual(null, result.InvalidLegs.Single().AltLineId);
            Assert.AreEqual(167, result.InvalidLegs.Single().Price);
            Assert.AreEqual("10924E23-A2FE-4317-BFFD-80504675F554", result.InvalidLegs.Single().CorrelatedLegs.Single().ToString().ToUpper());
        }

        [Test]
        public async Task SpecialAsync_WithPlaceBetRequest_ReturnMultiBetResponseOfSpecialBetResponse()
        {
            // Arrange
            var jsonForTest = "{\"bets\":[{\"status\":\"ACCEPTED\",\"errorCode\":null,\"betId\":760745142,\"uniqueRequestId\":\"10924E23-A2FE-4317-BFFD-80504675F554\",\"betterLineWasAccepted\":false}]}";
            var placeBetRequest = new MultiBetRequestOfSpecialBetRequest();
            placeBetRequest.Bets = new ObservableCollection<SpecialBetRequest>
            {
                new SpecialBetRequest
                {
                    AcceptBetterLine = true,
                    OddsFormat = OddsFormat.DECIMAL,
                    Stake = 10.5,
                    WinRiskStake = SpecialBetRequestWinRiskStake.RISK,
                    LineId = 51024304,
                    SpecialId = 726394409,
                    ContestantId = 726394411
                }
            };

            var client = GetBetsClientMock(jsonForTest);

            // Act
            var result = await client.V2BetsSpecialAsync(placeBetRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(SpecialBetResponseStatus.ACCEPTED, result.Bets.Single().Status);
            Assert.AreEqual(null, result.Bets.Single().ErrorCode);
            Assert.AreEqual("10924E23-A2FE-4317-BFFD-80504675F554", result.Bets.Single().UniqueRequestId.ToString().ToUpper());
            Assert.AreEqual(760745142, result.Bets.Single().BetId);
            Assert.AreEqual(false, result.Bets.Single().BetterLineWasAccepted);
        }
    }
}
