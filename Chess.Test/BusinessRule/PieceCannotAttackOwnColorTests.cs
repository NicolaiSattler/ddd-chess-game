using System.Collections.Generic;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using FluentResults;

namespace Chess.Test.BusinessRule;

[TestClass]
public class PieceCannotAttackOwnColorTests
{
    private PieceCannotAttackOwnColor _sut;
    private List<Piece> _pieces;
    private TakeTurn _command;

    [TestMethod]
    public void TakeTurn_CheckRule_IsValidMove()
    {
        //Arrange
        _pieces = new()
        {
            new Pawn() { Color = Color.Black, Position = new(File.B, 3) },
            new Pawn() { Color = Color.Black, Position = new(File.C, 2) }
        };

        _command = new() { StartPosition = new(File.C, 2), EndPosition = new(File.C, 3) };

        _sut = new PieceCannotAttackOwnColor(_command, _pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .IsSuccess.ShouldBeTrue();
    }

    [TestMethod]
    public void TakeTurn_CheckRule_IsInvalidMove()
    {
        //Arrange
        _pieces = new()
        {
            new Pawn() { Color = Color.Black, Position = new(File.C, 3) },
            new Pawn() { Color = Color.Black, Position = new(File.C, 2) }
        };

        _command = new() { StartPosition = new(File.C, 2), EndPosition = new(File.C, 3) };
        _sut = new PieceCannotAttackOwnColor(_command, _pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeOfType<Result>()
              .HasError<PieceCannotAttackOwnColorError>()
              .ShouldBeTrue();
    }
}
