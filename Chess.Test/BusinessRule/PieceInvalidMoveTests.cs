using System.Collections.Generic;
using System.Linq;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;

namespace Chess.Test.BusinessRule;

[TestClass]
public class PieceInvalidMoveTests
{
    private PieceInvalidMove _sut;

    [TestMethod]
    public void  PawnA1_MoveToB2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn
        {
            StartPosition = new(File.D, 1),
            EndPosition = new(File.C, 2)
        };
        var pieces = new List<Piece>
        {
            new Pawn { Position = new Square(File.D, 1), Color = Color.White }
        };
        _sut = new PieceInvalidMove(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
        result.FirstOrDefault().ViolationMessage.ShouldBe("A pawn must attack a filled square.");
    }

    [TestMethod]
    public void  PawnA1_MoveToH2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn
        {
            StartPosition = new(File.D, 1),
            EndPosition = new(File.H, 2)
        };
        var pieces = new List<Piece>
        {
            new Pawn { Position = new(File.D, 1), Color = Color.White }
        };
        _sut = new PieceInvalidMove(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
        result.FirstOrDefault().ViolationMessage.ShouldBe("Piece must move to designated squares.");
    }

    [TestMethod]
    public void  BishopA1_MoveToA2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn
        {
            StartPosition = new(File.A, 1),
            EndPosition = new(File.A, 2)
        };
        var pieces = new List<Piece>
        {
            new Bishop { Position = new(File.A, 1), Color = Color.White }
        };
        _sut = new PieceInvalidMove(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
        result.FirstOrDefault().ViolationMessage.ShouldBe("Piece must move to designated squares.");
    }
}