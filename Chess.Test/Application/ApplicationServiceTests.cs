using Chess.Application;
using Chess.Domain.Commands;
using Moq;
using System;
using System.Threading.Tasks;

namespace Chess.Test.Application;

[TestClass]
public class ApplicationServiceTests
{
    private Guid WhiteId { get; }
    private Guid BlackId { get; }
    private Guid AggregateId { get; }
    private InMemoryMatchRepository _repository;
    private Mock<ITurnTimer> _mockedTimer;
    private ApplicationService _sut;

    [TestInitialize]
    public void Initialize()
    {
        _repository = new();
        _mockedTimer = new();
        _sut = new(_repository, _mockedTimer.Object);
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
        var match = await _repository.GetAsync(aggregateId);
        match.Version.ShouldBe(3);
    }
}
