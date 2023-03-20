using System;
using Chess.Domain.BusinessRules;
using Chess.Domain.Entities;

namespace Chess.Core.BusinessRules;

[TestClass]
[Ignore]
public class TurnExpiredTests
{
    private TurnExpired _sut;

    [TestMethod]
    public void CheckRule_FiftyNineMinutes_IsWithinTimeLimit()
    {
        //Arrange
        var turn = new Turn { StartTime = DateTime.Now.AddHours(-1) };
        var maxTurnLength = new TimeSpan(0, 59, 0);

        _sut = new(turn, maxTurnLength);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void CheckRule_SixtyMinutes_ExceedsTimeLimit()
    {
        //Arrange
        var turn = new Turn { StartTime = DateTime.Now.AddHours(-1) };
        var maxTurnLength = new TimeSpan(1, 0, 0);

        _sut = new(turn, maxTurnLength);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void Init_TurnIsNull_ThrowsException()
    {
        //Arrange
        var maxTurnLength = new TimeSpan(1, 0, 0);


        //Act & Assert
        Should.Throw<ArgumentNullException>(() => _sut = new(null, maxTurnLength));
    }

    [TestMethod]
    public void Init_StartTimeIsNull_ThrowsException()
    {
        //Arrange
        var maxTurnLength = new TimeSpan(1, 0, 0);
        var turn = new Turn();


        //Act & Assert
        Should.Throw<ArgumentException>(() => _sut = new(turn, maxTurnLength));
    }

    [TestMethod]
    public void Init_TurnLength_Zero_IsNotAllowed()
    {
        //Arrange

        var turn = new Turn { StartTime = DateTime.Now.AddHours(-1) };
        var maxTurnLength = new TimeSpan(0, 0, 0);

        //Act & Assert
        Should.Throw<ArgumentException>(() => _sut = new(turn, maxTurnLength));
    }
}
