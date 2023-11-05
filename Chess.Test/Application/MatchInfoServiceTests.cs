using System;
using System.Threading.Tasks;
using Chess.Application.Services;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Events;
using Chess.Infrastructure.Repository;
using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Test.Application;

[TestClass]
public class MatchInfoServiceTests
{
    private IMatchDataService _mockedMatchDataService;
    private IMatchRepository _mockedMatchRepository;
    private MatchInfoService _sut;
    private Fixture _fixture;

    [TestInitialize]
    public void Initialize()
    {
        _fixture = new();
        _mockedMatchDataService = Substitute.For<IMatchDataService>();
        _mockedMatchRepository = Substitute.For<IMatchRepository>();
        _sut = new(_mockedMatchDataService, _mockedMatchRepository);
    }

    [TestMethod]
    public async Task GetColorAtTurn_ShouldReturnWhite()
    {
        //Arrange

        var aggregateId = Guid.NewGuid();
        var command = _fixture.Create<StartMatch>();
        var match = new Match(aggregateId);
        match.Start(command);

        _mockedMatchDataService.GetAggregateAsync(aggregateId)
                               .Returns(match);

        //Act
        var result = await _sut.GetColorAtTurnAsync(aggregateId);

        //Assert
        result.Should().Be(Color.White);
    }

    [TestMethod]
    [DataRow(Color.White)]
    [DataRow(Color.Black)]
    public async Task GetPlayer_ShouldReturnPlayerByColor(Color color)
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var command = _fixture.Create<StartMatch>();
        var match = new Match(aggregateId);
        match.Start(command);

        _mockedMatchDataService.GetAggregateAsync(aggregateId)
                               .Returns(match);

        //Act
        var result = await _sut.GetPlayerAsync(aggregateId, color);

        //Assert
        result.Should().NotBeNull();
        result.Color.Should().Be(color);
    }

    [TestMethod]
    public async Task GetPlayerAtTurn_ShouldReturn_WhitePlayerId()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var command = _fixture.Create<StartMatch>();
        var match = new Match(aggregateId);
        match.Start(command);

        _mockedMatchDataService.GetAggregateAsync(aggregateId)
                               .Returns(match);

        //Act
        var result = await _sut.GetPlayerAtTurnAsync(aggregateId);

        //Assert
        result.Should().NotBe(Guid.Empty).And.Be(match.White.MemberId);
    }

    [TestMethod]
    public async Task GetMatchResult_ShouldReturn_Undefined()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var command = _fixture.Create<StartMatch>();
        var match = new Match(aggregateId);
        match.Start(command);

        _mockedMatchDataService.GetAggregateAsync(aggregateId)
                               .Returns(match);

        //Act
        var result = await _sut.GetMatchResult(aggregateId);

        //Assert
        result.Should().Be(MatchResult.Undefined);
    }

    [TestMethod]
    public async Task GetMatchResult_ShouldReturn_BlackWins()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var match = new Match(aggregateId);
        var startCommand = _fixture.Create<StartMatch>();
        match.Start(startCommand);

        var forfeitCommand = new Forfeit() { MemberId = match.White.MemberId };
        match.Forfeit(forfeitCommand);

        _mockedMatchDataService.GetAggregateAsync(aggregateId)
                               .Returns(match);

        //Act
        var result = await _sut.GetMatchResult(aggregateId);

        //Assert
        result.Should().Be(MatchResult.BlackWins);
    }

    [TestMethod]
    public async Task Name()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var match = new MatchEntity
        {
            AggregateId = aggregateId,
            Options = new()
            {
                UseTurnTimer = true,
                DrawByRepition = true
            }
        };

        _mockedMatchRepository.GetAsync(aggregateId)
                              .Returns(match);

        //Act
        var result = await _sut.GetMatchOptionsAsync(aggregateId);

        //Assert
        result.ShouldNotBeNull();
        result.UseTurnTimer.ShouldBeTrue();
        result.DrawByRepition.ShouldBeTrue();
    }
}
