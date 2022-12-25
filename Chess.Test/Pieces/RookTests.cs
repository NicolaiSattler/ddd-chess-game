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
    public void WhiteAttackRange_A1_ShouldBeValid()
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

    [TestMethod]
    public void BlackAttackRange_A8_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Position = new(File.A, 8), Color = Color.Black };

        //Act
        var result = _sut.GetAttackRange();

        //Assert
        result.ShouldContain(s => s == new Square(File.A, 7));
        result.ShouldContain(s => s == new Square(File.A, 5));
        result.ShouldContain(s => s == new Square(File.A, 1));
        result.ShouldContain(s => s == new Square(File.B, 8));
        result.ShouldContain(s => s == new Square(File.E, 8));
        result.ShouldContain(s => s == new Square(File.H, 8));
    }

    [TestMethod]
    public void WhiteAttackRange_D4_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Position = new(File.D, 4), Color = Color.White };

        //Act
        var result = _sut.GetAttackRange();

        //Assert
        result.ShouldContain(s => s == new Square(File.D, 1));
        result.ShouldContain(s => s == new Square(File.D, 8));
        result.ShouldContain(s => s == new Square(File.A, 4));
        result.ShouldContain(s => s == new Square(File.H, 4));
    }
}