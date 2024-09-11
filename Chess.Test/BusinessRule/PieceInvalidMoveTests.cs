using System.Collections.Generic;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using FluentResults;

namespace Chess.Test.BusinessRule;

[TestClass]
public class PieceInvalidMoveTests
{
    private PieceInvalidMove _sut;

    [TestMethod]
    public void PawnA1_MoveToB2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn() { StartPosition = new(File.D, 1), EndPosition = new(File.C, 2) };
        var pieces = new List<Piece>
        {
            new Pawn { Position = new Square(File.D, 1), Color = Color.White }
        };
        var turns = new List<Turn>();

        _sut = new PieceInvalidMove(command, pieces, turns);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<InvalidMoveError>(m => m.Message == "A pawn must attack a filled square.")
              .ShouldBeTrue();
    }

    [TestMethod]
    public void PawnA1_MoveToH2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn() { StartPosition = new(File.D, 1), EndPosition =  new(File.H, 2) };
        var pieces = new List<Piece>
        {
            new Pawn { Position = new(File.D, 1), Color = Color.White }
        };
        var turns = new List<Turn>();

        _sut = new PieceInvalidMove(command, pieces, turns);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<InvalidMoveError>(m => m.Message == "A pawn must attack a filled square.")
              .ShouldBeTrue();
    }

    [TestMethod]
    public void BishopA1_MoveToA2_AttacksInvalidSquare()
    {
        //Arrange
        var command = new TakeTurn() { StartPosition = new(File.A, 1), EndPosition = new(File.A, 2) };
        var pieces = new List<Piece>
        {
            new Bishop { Position = new(File.A, 1), Color = Color.White }
        };
        var turns = new List<Turn>();
        _sut = new PieceInvalidMove(command, pieces, turns);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<InvalidMoveError>(m => m.Message == "Piece must move to designated squares.")
              .ShouldBeTrue();
    }

    [TestMethod]
    public void KingC5_MoveToC6_MoveIsInvalid()
    {
        //Arrange
        var command = new TakeTurn() { StartPosition = new(File.C, 5), EndPosition = new(File.C, 6) };
        var pieces = new List<Piece>
        {
            new King { Position = new(File.C, 5), Color = Color.White },
            new Rook { Position = new(File.H, 6), Color = Color.Black }
        };

        _sut = new PieceInvalidMove(command, pieces, new List<Turn>());

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<InvalidMoveError>(m => m.Message == "King cannot set itself check.")
              .ShouldBeTrue();
    }
}
