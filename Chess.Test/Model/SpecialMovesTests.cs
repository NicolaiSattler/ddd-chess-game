using System.Collections.Generic;
using Chess.Domain.Entities;
using Chess.Domain.Determiners;

namespace Chess.Test.Domain.Model;

[TestClass]
public class SpecialMovesTests
{

    [TestMethod]
    [DataRow(File.C)]
    [DataRow(File.E)]
    public void EnPessant_IsApplicable(File file)
    {
        //Arrange
        var turns = new List<Turn>
        {
            new()
            {
                PieceType = PieceType.Pawn,
                Player = new Player { Color = Color.Black },
                StartPosition = new(file, 7),
                EndPosition = new(file, 5)
            }
        };

        var pawn = new Pawn()
        {
            Color = Color.White,
            Position = new(File.D, 5)
        };

        //Act
        var result = SpecialMoves.IsEnPassant(pawn, turns);

        //Assert
        result.ShouldBeTrue();
    }

    [TestMethod]
    [DataRow(File.G, 7, true)]
    [DataRow(File.A, 7, true)]
    [DataRow(File.D, 7, false)]
    public void EnPessant_IsNotApplicable(File file, int rank, bool moveTwoRanks)
    {
        //Arrange
        rank = moveTwoRanks ? rank - 2 : rank;
        var turns = new List<Turn>
        {
            new()
            {
                PieceType = PieceType.Pawn,
                Player = new Player { Color = Color.Black },
                StartPosition = new(file, rank),
                EndPosition = new(file, rank)
            }
        };

        var pawn = new Pawn()
        {
            Color = Color.White,
            Position = new(File.D, 5)
        };

        //Act
        var result = SpecialMoves.IsEnPassant(pawn, turns);

        //Assert
        result.ShouldBeFalse();
    }

}
