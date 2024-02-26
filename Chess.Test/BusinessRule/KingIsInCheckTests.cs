using System.Collections.Generic;
using System.Linq;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;

namespace Chess.Test.BusinessRule;

[TestClass]
public class KingIsInCheckTests
{
    private KingIsInCheck _sut;

    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public void CheckRule_KingLiftsCheck()
    {
        //Arrange
        var command = new TakeTurn
        {
            StartPosition = new(File.E, 1),
            EndPosition = new(File.D, 2)
        };

        var pieces = new List<Piece>()
        {
            new King { Position = new(File.E, 1), Color = Color.White },
            new Pawn { Position = new(File.D, 7), Color = Color.White },
            new King { Position = new(File.E, 8), Color = Color.Black },
            new Pawn { Position = new(File.D, 2), Color = Color.Black }
        };

        _sut = new KingIsInCheck(command, pieces);

        //Act
        var result = _sut.CheckRule();

        //Assert
        result.ShouldBeEmpty();
    }
}
