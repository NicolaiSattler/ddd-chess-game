using Chess.Application;
using Chess.Application.Models;
using Chess.Domain.Commands;
using Chess.Infrastructure.Repository;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Test.Application;

[TestClass]
public class ApplicationServiceTests
{
    private Guid WhiteId { get; }
    private Guid BlackId { get; }
    private Guid AggregateId { get; }
    private Mock<IMatchRepository> _mockedMatchRepository;
    private InMemoryMatchRepository _eventRepository;
    private Mock<ITurnTimer> _mockedTimer;
    private ApplicationService _sut;

    [TestInitialize]
    public void Initialize()
    {
        _eventRepository = new();
        _mockedTimer = new();
        _mockedMatchRepository = new();
        _sut = new(_mockedMatchRepository.Object, _eventRepository, _mockedTimer.Object);
    }

    [TestMethod]
    public async Task StartGame_ShouldAddEvent()
    {
        //Arrange
        var whiteId = Guid.NewGuid();
        var blackId = Guid.NewGuid();

        var startMatchCommand = new StartMatch
        {
            MemberOneId = whiteId,
            MemberTwoId = blackId
        };

        var whiteTurn = new TakeTurn(whiteId, new(File.A, 2), new(File.A, 4), false);
        var blackTurn = new TakeTurn(blackId, new(File.C, 7), new(File.C, 5), false);

        //Act
        var aggregateId = await _sut.StartMatchAsync(startMatchCommand);
        await _sut.TakeTurnAsync(aggregateId, whiteTurn);
        await _sut.TakeTurnAsync(aggregateId, blackTurn);

        //Assert
        var result = await _eventRepository.GetAsync(aggregateId);
        result.Max(m => m.Version).ShouldBe(3);
    }
}
