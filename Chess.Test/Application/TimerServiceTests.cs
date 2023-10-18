using System;
using System.Threading;
using System.Threading.Tasks;
using Chess.Application.Events;
using Chess.Application.Services;
using Chess.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chess.Test.Application;

[TestClass]
public class TimerServiceTests
{
    private TimerService _sut;
    private IOptions<MatchOptions> _mockedMatchOptions;

    [TestInitialize]
    public void Initialize()
    {
        var logger = Substitute.For<ILogger<TimerService>>();
        _mockedMatchOptions = Substitute.For<IOptions<MatchOptions>>();
        _mockedMatchOptions.Value.Returns(new MatchOptions() { MaxTurnTime = new TimeSpan(0, 10, 0)});
        _sut = new(logger, _mockedMatchOptions);
    }

    [TestMethod]
    public void TurnNotEndedInTime_ShouldRaise_TurnExpiredEvent()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var turnTimeInMilliSeconds = 100;
        var turnExpiredEventRaised = false;
        TurnExpiredEventArgs eventArgs = null;

        _sut.StartAsync(CancellationToken.None);
        _sut.TurnExpired += (sender, args) =>
        {
            eventArgs = args;
            turnExpiredEventRaised = true;
        };

        //Act
        _sut.Start(aggregateId, memberId, turnTimeInMilliSeconds);

        Thread.Sleep(turnTimeInMilliSeconds + 10);

        //Assert
        turnExpiredEventRaised.Should().BeTrue();

        eventArgs.AggregateId.Should().Be(aggregateId);
        eventArgs.MemberId.Should().Be(memberId);
        eventArgs.ExceededTime.Should().Be(new TimeSpan(0, 10, 0));
    }

    [TestMethod]
    public void TurnEndedInTime_ShouldNotRaise_TurnExpiredEvent()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var turnTimeInMilliSeconds = 100;
        var turnExpiredEventRaised = false;

        _sut.StartAsync(CancellationToken.None);
        _sut.TurnExpired += (sender, args) => turnExpiredEventRaised = true;

        //Act
        _sut.Start(aggregateId, memberId, turnTimeInMilliSeconds);
        Thread.Sleep(10);
        _sut.Stop();
        _sut.Start(aggregateId, memberId, turnTimeInMilliSeconds);
        _sut.Stop();

        //Assert
        turnExpiredEventRaised.Should().BeFalse();
    }

    [TestMethod]
    public async Task HostStopped_ShouldNotRaise_TurnExpiredEvent()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var turnTimeInMilliSeconds = 100;
        var turnExpiredEventRaised = false;

        await _sut.StartAsync(CancellationToken.None);
        _sut.TurnExpired += (sender, args) => turnExpiredEventRaised = true;

        //Act
        _sut.Start(aggregateId, memberId, turnTimeInMilliSeconds);
        Thread.Sleep(50);
        await _sut.StopAsync(CancellationToken.None);

        //Assert
        turnExpiredEventRaised.Should().BeFalse();
    }
}