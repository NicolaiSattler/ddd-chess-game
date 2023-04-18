using System;
using System.Collections.Generic;
using System.Threading;
using Chess.Application;
using Chess.Application.Models;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Events;
using Moq;

namespace Chess.Tests.Application;

[TestClass]
public class TurnTimerTests
{
    private TurnTimer _sut;
    private Mock<IMatchRepository> _mockedRepository;
    private Mock<IMatch> _mockedMatch;

    [TestInitialize]
    public void Initialize()
    {
        _mockedRepository = new Mock<IMatchRepository>();
        _mockedMatch = new Mock<IMatch>();
        _sut = new TurnTimer(_mockedRepository.Object);
    }

    [TestMethod]
    public void TurnTimer_ExceedsInterval_ResultsInPlayerForfeit()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var otherMemberId = Guid.NewGuid();
        var seconds = 1;
        var events = new List<DomainEvent>
        {
            new MatchStarted { WhiteMemberId = memberId, BlackMemberId = otherMemberId }
        };

        _mockedRepository.Setup(m => m.GetAsync(aggregateId)).ReturnsAsync(_mockedMatch.Object);
        _mockedRepository.Setup(m => m.SaveAsync(aggregateId, It.IsAny<DomainEvent>()));

        _mockedMatch.SetupGet(m => m.Events).Returns(events);
        _mockedMatch.Setup(m => m.Forfeit(It.IsAny<Forfeit>()));

        //Act
        _sut.Start(aggregateId, memberId, seconds);
        Thread.Sleep(1500);

        //Assert
        _mockedRepository.Verify(m => m.GetAsync(It.IsAny<Guid>()), Times.Once);
        _mockedRepository.Verify(m => m.SaveAsync(It.IsAny<Guid>(), It.IsAny<DomainEvent>()), Times.Once);

        _mockedMatch.Verify(m => m.Forfeit(It.IsAny<Forfeit>()), Times.Once);
    }


    [TestMethod]
    public void TurnTimer_StopsWithinTime()
    {
        //Arrange
        var aggregateId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var seconds = 1;

        //Act
        _sut.Start(aggregateId, memberId, seconds);
        Thread.Sleep(500);
        _sut.Stop();

        //Assert
        _mockedRepository.Verify(m => m.GetAsync(It.IsAny<Guid>()), Times.Never);
        _mockedRepository.Verify(m => m.SaveAsync(It.IsAny<Guid>(), It.IsAny<DomainEvent>()), Times.Never);

        _mockedMatch.Verify(m => m.Forfeit(It.IsAny<Forfeit>()), Times.Never);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _sut.Dispose();
    }
}
