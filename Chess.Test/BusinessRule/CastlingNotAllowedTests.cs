using System.Collections.Generic;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using FluentResults;

namespace Chess.Test.BusinessRule;

[TestClass]
public class CastlingNotAllowedTests
{
    [TestMethod]
    [DataRow(File.G, 1, Color.White)]
    [DataRow(File.C, 1, Color.White)]
    [DataRow(File.G, 8, Color.Black)]
    [DataRow(File.C, 8, Color.Black)]
    public void TakeTurn_CheckRule_IsValid(File file, int rank, Color color)
    {
        //Arrange

        var command = new TakeTurn { StartPosition = new(File.D, rank), EndPosition = new(file, rank) };
        var pieces = new List<Piece>()
        {
            new Rook { Color = color, Position = new(File.A, rank)},
            new King { Color = color, Position = new(File.D, rank)},
            new Rook { Color = color, Position = new(File.H, rank)}
        };
        var turns = new List<Turn>();
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .IsSuccess.ShouldBeTrue();
    }

    [TestMethod]
    [DataRow(File.G, 1, Color.White)]
    [DataRow(File.C, 1, Color.White)]
    [DataRow(File.G, 8, Color.Black)]
    [DataRow(File.C, 8, Color.Black)]
    public void TakeTurn_CheckRule_IsNotValid_RookMoved(File file, int rank, Color color)
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, rank), EndPosition =  new(file, rank) };
        var pieces = new List<Piece>()
        {
            new Rook { Color = color, Position = new(File.A, rank)},
            new King { Color = color, Position = new(File.E, rank)},
            new Rook { Color = color, Position = new(File.H, rank)}
        };
        var turns = new List<Turn>
        {
            new() { StartPosition = new(File.A, 1), EndPosition = new(File.C, 1), PieceType = PieceType.Rook },
            new() { StartPosition = new(File.A, 8), EndPosition = new(File.C, 8), PieceType = PieceType.Rook },
            new() { StartPosition = new(File.H, 1), EndPosition = new(File.D, 1), PieceType = PieceType.Rook },
            new() { StartPosition = new(File.H, 8), EndPosition = new(File.D, 8), PieceType = PieceType.Rook },
        };
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }

    [TestMethod]
    [DataRow(File.G, 1, Color.White)]
    [DataRow(File.C, 1, Color.White)]
    [DataRow(File.G, 8, Color.Black)]
    [DataRow(File.C, 8, Color.Black)]
    public void TakeTurn_CheckRule_IsNotValid_KingMoved(File file, int rank, Color color)
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, rank), EndPosition =  new(file, rank) };
        var pieces = new List<Piece>()
        {
            new Rook { Color = color, Position = new(File.A, rank)},
            new King { Color = color, Position = new(File.E, rank)},
            new Rook { Color = color, Position = new(File.H, rank)}
        };
        var turns = new List<Turn>
        {
            new() { StartPosition = new(File.E, 1), EndPosition = new(File.F, 1), PieceType = PieceType.King },
            new() { StartPosition = new(File.F, 1), EndPosition = new(File.E, 1), PieceType = PieceType.King },
            new() { StartPosition = new(File.E, 8), EndPosition = new(File.D, 8), PieceType = PieceType.King },
            new() { StartPosition = new(File.D, 8), EndPosition = new(File.E, 8), PieceType = PieceType.King },
        };
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }

    [TestMethod]
    public void TakeTurn_CheckRule_IsNotValid_KingIsInCheck()
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, 1), EndPosition =  new(File.G, 1) };
        var pieces = new List<Piece>()
        {
            new Rook { Color = Color.Black, Position = new(File.E, 7)},
            new King { Color = Color.White, Position = new(File.E, 1)},
        };
        var turns = new List<Turn>();
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }

    [TestMethod]
    public void TakeTurn_CheckRule_IsNotValid_MoveIsBlocked()
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, 1), EndPosition =  new(File.G, 1) };
        var pieces = new List<Piece>()
        {
            new King { Color = Color.White, Position = new(File.E, 1)},
            new Bishop { Color = Color.White, Position = new(File.F, 1)},
            new Rook { Color = Color.Black, Position = new(File.H, 1)},
        };
        var turns = new List<Turn>();
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }

    [TestMethod]
    [DataRow(File.F)]
    [DataRow(File.G)]
    public void  Taketurn_CheckRule_CannotPassThroughCheck_Kingside(File fileOfBlackRook)
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, 1), EndPosition =  new(File.G, 1) };
        var pieces = new List<Piece>()
        {
            new King { Color = Color.White, Position = new(File.E, 1)},
            new Rook { Color = Color.White, Position = new(File.H, 1)},
            new Rook { Color = Color.Black, Position = new(fileOfBlackRook, 4)},
        };
        var turns = new List<Turn>();
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }

    [TestMethod]
    [DataRow(File.C)]
    [DataRow(File.D)]
    public void  Taketurn_CheckRule_CannotPassThroughCheck_Queenside(File fileOfBlackRook)
    {
        //Arrange
        var command = new TakeTurn { StartPosition = new(File.E, 1), EndPosition =  new(File.C, 1) };
        var pieces = new List<Piece>()
        {
            new King { Color = Color.White, Position = new(File.E, 1)},
            new Rook { Color = Color.White, Position = new(File.A, 1)},
            new Rook { Color = Color.Black, Position = new(fileOfBlackRook, 4)},
        };
        var turns = new List<Turn>();
        var sut = new CastlingNotAllowed(command, pieces, turns);

        //Act
        var result = sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<CastlingNotAllowedError>()
              .ShouldBeTrue();
    }
}
