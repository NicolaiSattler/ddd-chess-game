using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Chess.Application.Models;
using Chess.Core;
using Chess.Domain.Configuration;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.Options;
using Moq;

namespace Chess.Tests.Application;

[TestClass]
public class TurnTimerTests
{
    private TurnTimer _sut;
    private Mock<IMatchEventRepository> _mockedRepository;
    private Mock<IEnumerable<MatchEvent>> _mockedEvents;

    [TestInitialize]
    public void Initialize()
    {
        var options = Options.Create(new MatchOptions() { MaxTurnTime = new(0, 0, 1) });

        _mockedRepository = new();
        _mockedEvents = new();
        _sut = new TurnTimer(_mockedRepository.Object, options);
    }

    [TestMethod]
    public void TurnTimer_ExceedsInterval_ResultsInPlayerForfeit()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var otherMemberId = Guid.NewGuid();
        var events = new List<MatchEvent>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                Type = nameof(MatchStarted),
                Version = 1,
                Data = JsonSerializer.Serialize(new MatchStarted { WhiteMemberId = memberId, BlackMemberId = otherMemberId })
            }
        };

        _mockedRepository.Setup(m => m.GetAsync(aggregateId)).ReturnsAsync(events);
        _mockedRepository.Setup(m => m.AddAsync(aggregateId, It.IsAny<DomainEvent>(), It.IsAny<bool>()));

        //Act
        _sut.StartAsync(CancellationToken.None);
        _sut.Start(aggregateId, memberId);

        Thread.Sleep(1500);

        //Assert
        _mockedRepository.Verify(m => m.GetAsync(It.IsAny<Guid>()), Times.Once);
        _mockedRepository.Verify(m => m.AddAsync(It.IsAny<Guid>(), It.IsAny<MatchEnded>(), It.IsAny<bool>()), Times.Once);
    }


    [TestMethod]
    public void TurnTimer_StopsWithinTime()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        //Act
        _sut.Start(aggregateId, memberId);
        Thread.Sleep(500);
        _sut.StopAsync(CancellationToken.None);

        //Assert
        _mockedRepository.Verify(m => m.GetAsync(It.IsAny<Guid>()), Times.Never);
        _mockedRepository.Verify(m => m.AddAsync(It.IsAny<Guid>(), It.IsAny<MatchEnded>(), It.IsAny<bool>()), Times.Never);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _sut.Dispose();
    }
}
