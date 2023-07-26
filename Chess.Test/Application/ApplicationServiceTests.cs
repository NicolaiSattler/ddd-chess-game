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

        var whiteTurn = new TakeTurn{ MemberId = whiteId, StartPosition = new(File.A, 2), EndPosition = new(File.A, 4) };
        var blackTurn = new TakeTurn{ MemberId = blackId, StartPosition = new(File.C, 7), EndPosition = new(File.C, 5) };

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
        result.Count.ShouldBe(32);
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

    [TestMethod]
    public async Task GetColorAtTurn_ShouldReturnCorrectColor()
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
        var firstAtTurn = await _sut.GetColorAtTurnAsync(aggregateId);

        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = whiteId, StartPosition = new(File.B, 2), EndPosition = new(File.B, 3) });
        var secondAtTurn = await _sut.GetColorAtTurnAsync(aggregateId);

        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = blackId, StartPosition = new(File.B, 7), EndPosition = new(File.B, 5) });
        var thirdAtTurn = await _sut.GetColorAtTurnAsync(aggregateId);

        //Assert
        firstAtTurn.ShouldBe(Color.White);
        secondAtTurn.ShouldBe(Color.Black);
        thirdAtTurn.ShouldBe(Color.White);
    }

    [TestMethod]
    public async Task GetPiecesAsync_ShouldReturnCorrectAmountOfPieces()
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

        var firstCount = (await _sut.GetPiecesAsync(aggregateId)).Count;

        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = whiteId, StartPosition = new(File.B, 2), EndPosition =  new(File.B, 3) });
        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = blackId, StartPosition = new(File.C, 7), EndPosition =  new(File.C, 5) });
        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = whiteId, StartPosition = new(File.B, 3), EndPosition =  new(File.B, 4) });
        await _sut.TakeTurnAsync(aggregateId, new() { MemberId = blackId, StartPosition = new(File.C, 5), EndPosition =  new(File.B, 4) });

        var secondCount = (await _sut.GetPiecesAsync(aggregateId)).Count;

        //Assert
        firstCount.ShouldBe(32);
        secondCount.ShouldBe(31);
    }
}
