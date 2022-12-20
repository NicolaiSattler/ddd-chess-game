using System;
using System.Linq;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Chess.Test.Pieces;

[TestClass]
public class PawnTests
{
    private Pawn _sut;

    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public void  WhiteAttackRange_A2_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.White,
            Position = new(File.A, 2),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 4);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 3);
    }

    [TestMethod]
    public void  BlackAttackRange_A7_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.A, 7),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 5);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 6);
    }

    [TestMethod]
    public void  WhiteAttackRange_H2_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.White,
            Position = new(File.H, 2),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.G && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.H && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.H && m.Rank == 4);
    }

    [TestMethod]
    public void  BlackAttackRange_H7_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.H, 7),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.G && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.H && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.H && m.Rank == 5);
    }

    [TestMethod]
    public void  WhiteAttackRange_C2_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.White,
            Position = new(File.C, 2),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(4);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 4);
    }

    [TestMethod]
    public void  BlackAttackRange_C7_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.C, 7),
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(4);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 5);
    }

    [TestMethod]
    public void  WhiteAttackRange_D4_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.White,
            Position = new(File.D, 4)
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 5);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 5);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 5);
    }

    [TestMethod]
    public void  BlackAttackRange_D4_ShouldBeValid()
    {
        //Arrange
        _sut = new(Guid.NewGuid())
        {
            Color = Color.Black,
            Position = new(File.D, 4)
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 3);
    }
}