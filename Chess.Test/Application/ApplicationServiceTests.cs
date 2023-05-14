using Chess.Application.Models;
using Chess.Domain.Commands;
using Chess.Infrastructure.Repository;
using Chess.Test.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Test.Application;

[TestClass]
public class ApplicationServiceTests: TestBase
{
    private Guid WhiteId { get; }
    private Guid BlackId { get; }
    private Guid AggregateId { get; }
    private Mock<ITurnTimer> _mockedTimer;
    private MatchRepository _matchRepository;
    private MatchEventRepository _eventRepository;
    private ApplicationService _sut;

    [TestInitialize]
    public void Initialize()
    {
        _mockedTimer = new();
        _matchRepository = new(Mock.Of<ILogger<MatchRepository>>(), DbContext);
        _eventRepository = new(Mock.Of<ILogger<MatchEventRepository>>(), DbContext);

        _sut = new(_matchRepository, _eventRepository, _mockedTimer.Object);
    }

    [TestMethod]
    public async Task StartGame_ShouldAddEvent()
    {
        //Arrange
        var whiteId = Guid.NewGuid();
        var blackId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        var startMatchCommand = new StartMatch
        {
            AggregateId = aggregateId,
            MemberOneId = whiteId,
            MemberTwoId = blackId
        };

        var whiteTurn = new TakeTurn(whiteId, new(File.A, 2), new(File.A, 4), false);
        var blackTurn = new TakeTurn(blackId, new(File.C, 7), new(File.C, 5), false);

        //Act
        await _sut.StartMatchAsync(startMatchCommand);
        await _sut.TakeTurnAsync(aggregateId, whiteTurn);
        await _sut.TakeTurnAsync(aggregateId, blackTurn);

        //Assert
        DbContext.Matches.Any(m => m.AggregateId == aggregateId).ShouldBeTrue();
        DbContext.Events.Count().ShouldBe(3);
    }

    [TestMethod]
    public async Task GetPiecesAsync_KnownAggregateId_ShouldReturnThirdyTwoPieces()
    {
        //Arrange
        var whiteId = Guid.NewGuid();
        var blackId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        var startMatchCommand = new StartMatch
        {
            AggregateId = aggregateId,
            MemberOneId = whiteId,
            MemberTwoId = blackId
        };

        //Act
        await _sut.StartMatchAsync(startMatchCommand);
        var result = await _sut.GetPiecesAsync(aggregateId);

        //Assert
        result.Count().ShouldBe(32);
        result.Count(m => m.Color == Color.Black).ShouldBe(16);
        result.Count(m => m.Color == Color.White).ShouldBe(16);
    }

    [TestMethod]
    public async Task GetPiecesAsync_UnknownAggregateId_ShouldThrowAnException()
    {
        //Arrange
        var whiteId = Guid.NewGuid();
        var blackId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        var startMatchCommand = new StartMatch
        {
            AggregateId = aggregateId,
            MemberOneId = whiteId,
            MemberTwoId = blackId
        };

        await _sut.StartMatchAsync(startMatchCommand);

        //Act & Assert
        var result = Should.ThrowAsync<ApplicationException>(async () => await _sut.GetPiecesAsync(Guid.Empty));
    }
}
