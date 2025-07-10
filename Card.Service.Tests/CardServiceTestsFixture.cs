using Card.Service.Interfaces;
using Card.Service.Models;
using Card.Service.Models.Enums;
using Card.Service.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RulesEngine.Models;
using System.Text.Json;
using Card.Service.Models.Exceptions;
using Card.Service.Validators;

namespace Card.Service.Tests
{
    public class CardServiceTestsFixture
    {
        public CardService Service { get; }
        private readonly string _rulesPath = "Workflows/action-rules.json";

        public CardServiceTestsFixture()
        {
            var mockCardRepository = new Mock<ICardRepository>();
            var testData = new Dictionary<(string, string), CardDetails>
            {
                { ("User1", "Card11"), new CardDetails("Card11", CardType.Prepaid, CardStatus.Closed, false) },
                { ("User2", "Card22"), new CardDetails("Card22", CardType.Credit, CardStatus.Restricted, true) },
                { ("User3", "Card33"), new CardDetails("Card33", CardType.Debit, CardStatus.Ordered, true) },
                { ("User4", "Card44"), new CardDetails("Card44", CardType.Debit, CardStatus.Ordered, false) },
                { ("User5", "Card55"), new CardDetails("Card55", CardType.Prepaid, CardStatus.Active, true) },
                { ("User6", "Card66"), new CardDetails("Card66", CardType.Credit, CardStatus.Blocked, true) },
                { ("User7", "Card77"), new CardDetails("Card77", CardType.Debit, CardStatus.Restricted, true) },
                { ("User8", "Card88"), new CardDetails("Card88", CardType.Prepaid, CardStatus.Inactive, true) },
                { ("User9", "Card99"), new CardDetails("Card99", CardType.Debit, CardStatus.Active, true) },
                { ("User10", "Card10"), new CardDetails("Card10", CardType.Credit, CardStatus.Expired, true) },

            };
            mockCardRepository.Setup(x => x.GetCardDetails(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync((string userId, string cardNumber) =>
            {
                if (testData.TryGetValue((userId, cardNumber), out var cardDetails))
                    return cardDetails;
                return null;
            });
            var ccs = new CardCacheService(new MemoryCache(new MemoryCacheOptions()), new Mock<ILogger<CardCacheService>>().Object);
            var matchingEngine = new MatchingEngineService(SetuRulesEngine(), new Mock<ILogger<MatchingEngineService>>().Object);
            Service = new CardService(mockCardRepository.Object,matchingEngine, ccs, new CardValidator(), new Mock<ILogger<CardService>>().Object);
        }

        private RulesEngine.RulesEngine SetuRulesEngine(ReSettings reSettings = null)
        {
            var rulesJson = File.ReadAllText(_rulesPath);
            var doc = JsonDocument.Parse(rulesJson);
            var rules = JsonSerializer.Deserialize<List<Workflow>>(doc.RootElement.GetProperty("Workflows"));
            return new RulesEngine.RulesEngine( rules?.ToArray(), reSettings);
        }
    }

    public class CardServiceAndMatchingEngineTests(CardServiceTestsFixture fixture) : IClassFixture<CardServiceTestsFixture>
    {
        [Theory]
        [MemberData(nameof(AllowedActions))]
        public async Task GetAllowedActions_Should_ReturnCorrectActions_When_UserIdAndCardIdAreValid(string userId, string cardId, string[] actions)
        {
            var result = await fixture.Service.GetAllowedActions(userId, cardId);
            Assert.NotNull(result);
            Assert.Equal(actions, result);
        }


        [Fact]
        public async Task GetAllowedActions_Should_ReturnCorrectActions_When_UserIdAndCardIdAreValidFromCache()
        {
            var expectedActions = new[] { "Action3", "Action4", "Action9" };
            var result = await fixture.Service.GetAllowedActions("User1", "Card11");
            Assert.NotNull(result);
            Assert.Equal(expectedActions, result);

            var resultCache = await fixture.Service.GetAllowedActions("User1", "Card11");
            Assert.NotNull(resultCache);
            Assert.Equal(expectedActions, resultCache);
        }

        [Theory]
        [InlineData("User1", "Card12312")]
        [InlineData("User1123123", "Card11")]
        [InlineData(null, null)]
        [InlineData(null, "Card11")]
        [InlineData("User1", null)]
        [InlineData("User1", "")]
        [InlineData("", "Card11")]
        [InlineData("/@#!@#!@#", "Card11")]
        public async Task GetAllowedActions_Should_ThrowCardNotFoundExeception_When_UserIdOrCardNumberAreInvalid(string userId, string cardNumber)
        {
           await Assert.ThrowsAnyAsync<CardNotFoundException>(async () => await fixture.Service.GetAllowedActions(userId, cardNumber));
        }

        public static IEnumerable<object[]> AllowedActions => new List<object[]>
        {
            new object[] { "User1", "Card11", new[] { "Action3", "Action4", "Action9" } },
            new object[] { "User2", "Card22", new[] { "Action3", "Action4", "Action5", "Action9" } },
            new object[] { "User3", "Card33", new[] { "Action3", "Action4", "Action6", "Action8", "Action9", "Action10", "Action12", "Action13"} },
            new object[] { "User4", "Card44", new[] { "Action3", "Action4", "Action7", "Action8", "Action9", "Action10", "Action12", "Action13"} },
            new object[] { "User5", "Card55", new[] { "Action1", "Action3", "Action4", "Action6", "Action8", "Action9", "Action10", "Action11", "Action12", "Action13" } },
            new object[] { "User6", "Card66", new[] { "Action3", "Action4", "Action5","Action6", "Action7", "Action8", "Action9" } },
            new object[] { "User7", "Card77", new[] { "Action3", "Action4", "Action9" } },
            new object[] { "User8", "Card88", new[]  { "Action2", "Action3", "Action4", "Action6", "Action8", "Action9", "Action10", "Action11", "Action12", "Action13" } },
            new object[] { "User9", "Card99", new[] { "Action1", "Action3", "Action4", "Action6", "Action8", "Action9", "Action10", "Action11", "Action12", "Action13" } } ,
            new object[] { "User10", "Card10", new[] {"Action3", "Action4", "Action5", "Action9"  } }
        };

    }
}