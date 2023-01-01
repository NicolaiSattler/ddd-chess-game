using System.Collections.Generic;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;

namespace Chess.Test.BusinessRule;

[TestClass]
public class PieceIsBlockedTests
{
    private PieceIsBlocked _sut;

    [TestMethod]
    public void RookH1_MoveToD1_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Rook { Color = Color.White, Position = new (File.H, 1)},
            new Queen { Color = Color.White, Position = new (File.D, 1)}
        };

        var command = new TakeTurn
        {
            StartPosition = new(File.H, 1),
            EndPosition = new(File.A, 1)
        };
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void  BishopA1_MoveToC3_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Bishop { Color = Color.White, Position = new (File.A, 1)},
            new Queen { Color = Color.White, Position = new (File.B, 2)}
        };

        var command = new TakeTurn
        {
            StartPosition = new(File.A, 1),
            EndPosition = new(File.C, 3)
        };
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void  PawnA1_MoveToA3_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Pawn { Color = Color.White, Position = new (File.A, 1)},
            new Queen { Color = Color.Black, Position = new (File.A, 2)}
        };

        var command = new TakeTurn
        {
            StartPosition = new(File.A, 1),
            EndPosition = new(File.A, 3)
        };
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();

    }
}