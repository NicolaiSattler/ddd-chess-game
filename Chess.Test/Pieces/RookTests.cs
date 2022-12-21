using Chess.Core.Match.Entities.Pieces;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Chess.Test.Pieces;

[TestClass]
public class RookTests
{
    private Rook _sut;

    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public void  Name()
    {
        //Arrange
        _sut = new() { Position = new(File.A, 1), Color = Color.White };

        //Act
        var result = _sut.GetAttackRange();

        //Assert
        result.ShouldContain(s => s == new Square(File.A, 2));
        result.ShouldContain(s => s == new Square(File.A, 5));
        result.ShouldContain(s => s == new Square(File.A, 8));
        result.ShouldContain(s => s == new Square(File.B, 1));
        result.ShouldContain(s => s == new Square(File.E, 1));
        result.ShouldContain(s => s == new Square(File.H, 1));
    }
}