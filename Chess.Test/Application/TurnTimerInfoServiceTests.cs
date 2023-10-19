using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chess.Application.Services;
using Chess.Domain.Configuration;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Repository;

namespace Chess.Test.Application;

[TestClass]
public class TurnTimerInfoServiceTests
{
    private IMatchRepository _mockedMatchRepository;
    private TurnTimerInfoService  _sut;
    private Fixture _fixture;

    [TestInitialize]
    public void Initialize()
    {
        _mockedMatchRepository = Substitute.For<IMatchRepository>();
        _fixture = new();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _sut = new(_mockedMatchRepository);
    }

    [TestMethod]
    [DataRow(-7, 179)]
    [DataRow(-11, -60)]
    public async Task GetRemainingTime_ShouldReturn_ThreeMinutes(int minutesPassed, int secondsRemaining)
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchOptions = _fixture.Build<MatchOptions>()
                                   .With(m => m.MaxTurnTime, new TimeSpan(0, 10, 0))
                                   .Create();

        var matchEvent = new MatchEvent()
        {
            AggregateId = aggregateId,
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(minutesPassed)
        };

        var match = _fixture.Build<Match>()
                            .With(m => m.AggregateId, aggregateId)
                            .With(m => m.Options, matchOptions)
                            .With(m => m.Events, new List<MatchEvent> { matchEvent })
                            .Create();

        _mockedMatchRepository.GetAsync(aggregateId, true)
                              .Returns(match);

        //Act
        var result = await _sut.GetRemainingTimeAsync(aggregateId);

        //Assert
        result.Should().BeGreaterThanOrEqualTo(secondsRemaining);
    }

    [TestMethod]
    public async Task GetRemainingTime_NoMatchFound_ShouldThrowException()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        _mockedMatchRepository.GetAsync(aggregateId, true)
                              .Returns((Match)null);

        //Act & Assert
        var result = await _sut.Invoking(m => m.GetRemainingTimeAsync(aggregateId))
                               .Should().ThrowAsync<ApplicationException>()
                               .WithMessage($"No match was found with id {aggregateId}");
    }

    [TestMethod]
    public async Task GetRemainingTime_WithoutEvents_ShouldReturn_Zero()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchOptions = _fixture.Build<MatchOptions>()
                                   .With(m => m.MaxTurnTime, new TimeSpan(0, 10, 0))
                                   .Create();

        var match = _fixture.Build<Match>()
                            .With(m => m.AggregateId, aggregateId)
                            .With(m => m.Options, matchOptions)
                            .With(m => m.Events, new List<MatchEvent>())
                            .Create();

        _mockedMatchRepository.GetAsync(aggregateId, true)
                              .Returns(match);

        //Act
        var result = await _sut.GetRemainingTimeAsync(aggregateId);

        //Assert
        result.Should().Be(0);
    }


    [TestMethod]
    public async Task GetRemainingTime_WithoutMaxTurnTime_ShouldReturn_Zero()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var matchOptions = _fixture.Build<MatchOptions>()
                                   .With(m => m.MaxTurnTime, TimeSpan.MinValue)
                                   .Create();

        var matchEvent = new MatchEvent()
        {
            AggregateId = aggregateId,
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-7)
        };

        var match = _fixture.Build<Match>()
                            .With(m => m.AggregateId, aggregateId)
                            .With(m => m.Options, matchOptions)
                            .With(m => m.Events, new List<MatchEvent> { matchEvent })
                            .Create();

        _mockedMatchRepository.GetAsync(aggregateId, true)
                              .Returns(match);

        //Act
        var result = await _sut.GetRemainingTimeAsync(aggregateId);

        //Assert
        result.Should().Be(0);
    }
}