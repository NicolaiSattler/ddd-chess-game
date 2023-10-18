using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.Application.Services;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Repository;
using System.Text.Json;
using System.Linq;

namespace Chess.Test.Application;

[TestClass]
public class MatchDataServiceTests
{
    private IMatchRepository _mockedMatchRepository;
    private IMatchEventRepository _mockedMatchEventRepository;
    private MatchDataService _sut;
    private Fixture _fixture;

    [TestInitialize]
    public void Initialize()
    {
        _fixture = new();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _mockedMatchRepository = Substitute.For<IMatchRepository>();
        _mockedMatchEventRepository = Substitute.For<IMatchEventRepository>();
        _sut = new(_mockedMatchRepository, _mockedMatchEventRepository);
    }

    [TestMethod]
    public async Task GetAggregate_KnownId_ShouldReturn_Aggregate()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchStarted = _fixture.Create<MatchStarted>();
        _mockedMatchEventRepository.GetAsync(aggregateId)
                                   .Returns(new List<MatchEvent>()
                                            {
                                                new()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    AggregateId = aggregateId,
                                                    Type = nameof(MatchStarted),
                                                    Version = 1,
                                                    CreatedAtUtc = DateTime.UtcNow,
                                                    Data = JsonSerializer.Serialize( matchStarted, JsonSerializerOptions.Default)
                                                }
                                            });

        //Act
        var result = await _sut.GetAggregateAsync(aggregateId);

        //Assert
        result.Should().NotBe(null);
        result.Turns.Should().NotBeEmpty().And.HaveCount(1);
    }

    [TestMethod]
    public async Task GetAggregate_InvalidEventData_ShouldThrow_InvalidOperationException()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        _mockedMatchEventRepository.GetAsync(aggregateId)
                                   .Returns(new List<MatchEvent>()
                                            {
                                                new()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    AggregateId = aggregateId,
                                                    Type = "Unknown Type",
                                                    Version = 1,
                                                    CreatedAtUtc = DateTime.UtcNow
                                                }
                                            });

        //Act & Assert
        await _sut.Invoking(m => m.GetAggregateAsync(aggregateId))
                  .Should().ThrowAsync<InvalidOperationException>()
                  .WithMessage("Type is unknown.");
    }

    [TestMethod]
    public async Task GetAggregate_UnknownId_ShouldThrow_ApplicationException()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchStarted = _fixture.Create<MatchStarted>();
        _mockedMatchEventRepository.GetAsync(aggregateId)
                                   .Returns(Enumerable.Empty<MatchEvent>());

        //Act & Assert
        await _sut.Invoking(m => m.GetAggregateAsync(aggregateId))
                  .Should().ThrowAsync<ApplicationException>()
                  .Where(m => m.Message.Contains(aggregateId.ToString()));
    }

    [TestMethod]
    public async Task GetMatches_ShouldReturnAllMatchEntities()
    {
        //Arrange
        var matches = _fixture.Build<Match>()
                              .With(m => m.AggregateId, Guid.NewGuid())
                              .With(m => m.WhitePlayerId, Guid.NewGuid())
                              .With(m => m.BlackPlayerId, Guid.NewGuid())
                              .CreateMany(5);

        _mockedMatchRepository.GetAsync()
                              .Returns(matches);

        //Act
        var result = await _sut.GetMatchesAsync();

        //Assert
        result.Should().NotBeEmpty().And.HaveSameCount(matches);
    }

    [TestMethod]
    public async Task GetPieces_OfKnownAggregate_ShouldReturn_ThirdyTwoPieces()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchStarted = _fixture.Create<MatchStarted>();
        _mockedMatchEventRepository.GetAsync(aggregateId)
                                   .Returns(new List<MatchEvent>()
                                            {
                                                new()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    AggregateId = aggregateId,
                                                    Type = nameof(MatchStarted),
                                                    Version = 1,
                                                    CreatedAtUtc = DateTime.UtcNow,
                                                    Data = JsonSerializer.Serialize(matchStarted, JsonSerializerOptions.Default)
                                                }
                                            });

        //Act
        var result = await _sut.GetPiecesAsync(aggregateId);

        //Assert
        result.Should().NotBeNullOrEmpty().And.HaveCount(32);
    }

    [TestMethod]
    public async Task GetTurns_OfKnownAggregate_ShouldReturn_TwoTurns()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchStarted = _fixture.Create<MatchStarted>();
        var matchEnded = _fixture.Create<TurnTaken>();

        _mockedMatchEventRepository.GetAsync(aggregateId)
                                   .Returns(new List<MatchEvent>()
                                            {
                                                new()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    AggregateId = aggregateId,
                                                    Type = nameof(MatchStarted),
                                                    Version = 1,
                                                    CreatedAtUtc = DateTime.UtcNow,
                                                    Data = JsonSerializer.Serialize(matchStarted, JsonSerializerOptions.Default)
                                                },
                                                new()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    AggregateId = aggregateId,
                                                    Type = nameof(TurnTaken),
                                                    Version = 2,
                                                    CreatedAtUtc = DateTime.UtcNow,
                                                    Data = JsonSerializer.Serialize(matchStarted, JsonSerializerOptions.Default)
                                                }
                                            });

        //Act
        var result = await _sut.GetTurns(aggregateId);

        //Assert
        result.Should().NotBeNullOrEmpty().And.HaveCount(1);
    }
}