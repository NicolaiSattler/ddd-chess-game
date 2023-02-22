using Chess.Application;
using Chess.Domain.Commands;
using System;

namespace Chess.Test.Application;

[TestClass]
public class ApplicationServiceTests
{
    private Guid WhiteId { get; }
    private Guid BlackId { get; }
    private Guid AggregateId { get; }
    private InMemoryMatchRepository _repository;
    private ApplicationService _sut;

    [TestInitialize]
    public void Initialize()
    {
        _repository = new();
        _sut = new(_repository);
    }

    [TestMethod]
    [Ignore("Needs to be fixed")]
    public void StartGame_ShouldAddEvent()
    {
        //Arrange
        var whiteId = Guid.NewGuid();
        var blackId = Guid.NewGuid();

        var startMatchCommand = new StartMatch
        {
            MemberOneId = whiteId,
            MemberTwoId = blackId
        };
        var whiteTurn = new TakeTurn
        {
            MemberId = whiteId,
            StartPosition = new Square(File.A, 2),
            EndPosition = new Square(File.A, 4)
        };

        var blackTurn = new TakeTurn
        {
            MemberId = blackId,
            StartPosition = new Square(File.C, 7),
            EndPosition = new Square(File.C, 5)
        };

        //Act
        var aggregateId = _sut.StartMatch(startMatchCommand);
        _sut.TakeTurn(aggregateId, whiteTurn);
        _sut.TakeTurn(aggregateId, blackTurn);

        //Assert
        var match = _repository.Get(aggregateId);
        match.Version.ShouldBe(3);
    }
}
