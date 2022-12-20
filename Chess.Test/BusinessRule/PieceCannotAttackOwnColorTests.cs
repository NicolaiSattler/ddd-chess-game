using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Chess.Test.BusinessRule;

[TestClass]
public class PieceCannotAttackOwnColorTests
{
    private PieceCannotAttackOwnColor _sut;
    private List<Piece> _pieces;
    private TakeTurn _command;

    [TestMethod]
    public void  TakeTurn_CheckRule_IsValidMove ()
    {
        //Arrange
        _pieces = new()
        {
            new Pawn(Guid.NewGuid()) { Color = Color.Black, Position = new(File.B, 3) },
            new Pawn(Guid.NewGuid()) { Color = Color.Black, Position = new(File.C, 2) }
        };

        _command = new()
        {
            MemberId = Guid.NewGuid(),
            StartPosition = new(File.C, 2),
            EndPosition = new(File.C, 3)
        };

        _sut = new PieceCannotAttackOwnColor(_command, _pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void  TakeTurn_CheckRule_IsInvalidMove()
    {
        //Arrange
        _pieces = new();
        _pieces.Add(new Pawn(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.C, 3)
        });

        _pieces.Add(new Pawn(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.C, 2)
        });

        _command = new()
        {
            MemberId = Guid.NewGuid(),
            StartPosition = new(File.C, 2),
            EndPosition = new(File.C, 3)
        };

        _sut = new PieceCannotAttackOwnColor(_command, _pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.Count().ShouldBe(1);
    }
}