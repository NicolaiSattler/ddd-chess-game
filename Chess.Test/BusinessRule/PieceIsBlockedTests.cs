using System;
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

        var command = new TakeTurn(Guid.Empty, new(File.H, 1), new(File.A, 1), false);
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void BishopA1_MoveToC3_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Bishop { Color = Color.White, Position = new (File.A, 1)},
            new Queen { Color = Color.White, Position = new (File.B, 2)}
        };

        var command = new TakeTurn(Guid.Empty, new(File.A, 1), new(File.C, 3), false);
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void PawnA1_MoveToA3_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Pawn { Color = Color.White, Position = new (File.A, 1)},
            new Queen { Color = Color.Black, Position = new (File.A, 2)}
        };

        var command = new TakeTurn(Guid.Empty, new(File.A, 1), new(File.A, 3), false);
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void BishopC8_MoveToE6_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Bishop { Color = Color.White, Position = new (File.C, 8)},
            new Queen { Color = Color.White, Position = new (File.D, 7)}
        };

        var command = new TakeTurn(Guid.Empty, new(File.C, 8), new(File.E, 6), false);
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void KingC8_MoveToC7_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new King { Color = Color.White, Position = new (File.C, 8)},
            new Queen { Color = Color.White, Position = new (File.C, 7)}
        };

        var command = new TakeTurn(Guid.Empty, new(File.C, 8), new(File.C, 7), false);
        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }

    [TestMethod]
    public void PawnH6_MoveToG5_IsBlockedByQueen()
    {
        //Arrange
        var pieces = new List<Piece>
        {
            new Pawn { Color = Color.Black, Position = new (File.H, 6)},
            new Queen { Color = Color.Black, Position = new (File.G, 5)}
        };

        var command = new TakeTurn(Guid.Empty, new(File.H, 6), new(File.G, 5), false);

        _sut = new PieceIsBlocked(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldNotBeEmpty();
    }
}
