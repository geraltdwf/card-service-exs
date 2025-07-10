using Card.Service.Models.Enums;
using Card.Service.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Card.Service.Interfaces;

namespace Card.Service.Tests;


public class CardRepositoryTestsFixture
{
    public ICardRepository Repository { get; }

    public CardRepositoryTestsFixture()
    {
        var mockLogger = new Mock<ILogger<CardRepository>>();
        Repository = new CardRepository(mockLogger.Object);
    }
}

public class CardRepositoryTests : IClassFixture<CardRepositoryTestsFixture>
{
    private readonly CardRepositoryTestsFixture _fixture;
    public CardRepositoryTests(CardRepositoryTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("User1", "Card11", CardType.Prepaid, CardStatus.Ordered)]
    [InlineData("User2", "Card29", CardType.Debit, CardStatus.Inactive)]
    [InlineData("User3", "Card321", CardType.Credit, CardStatus.Closed)]
    public async Task GetCardDetails_Should_ReturnCardDetails_When_UserIdAndCardNumberAreValid(string userId, string cardNumber, CardType cardType, CardStatus cardStatus)
    {
        var result = await _fixture.Repository.GetCardDetails(userId, cardNumber);
        Assert.NotNull(result);
        Assert.Equal(cardNumber, result.CardNumber);
        Assert.Equal(cardType, result.CardType);
        Assert.Equal(cardStatus, result.CardStatus);
    }


    [Theory]
    [InlineData("UknownUser", "Card11")]
    [InlineData("User1", "UknownCard")]
    [InlineData("UknownUser", "UknownCard")]
    public async Task GetCardDetails_Should_ReturnNull_When_UserIdAndCardNumberAreInvalid(string userId, string cardNumber)
    {
        var result = await _fixture.Repository.GetCardDetails(userId, cardNumber);
        Assert.Null(result);
    }


  
}