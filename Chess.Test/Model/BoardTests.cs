using System;
using System.Collections.Generic;
using Chess.Core.Match.Events;
using Chess.Domain.Model;

namespace Chess.Test.Domain.Model
{
    [TestClass]
    public class BoardTests
    {

        [TestMethod]
        public void PawnC7_MovesToC8_IsPromoted()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.C, 8));
            var pieces = new List<Piece>()
            {
                new Pawn
                {
                    Color = Color.White,
                    Position = new(File.C, 7)
                }
            };


            //Act
            var result = Board.PawnIsPromoted(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void PawnC7_MovesToD8_Captures_BishopD8()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.D, 8));
            var pieces = new List<Piece>()
            {
                new Pawn { Color = Color.White, Position = new(File.C, 7) },
                new Bishop { Color = Color.Black, Position = new(File.D, 8)}
            };

            //Act
            var result = Board.PieceIsCaptured(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void PawnC7_MovesToD8_DoesNotCapture_BishopD8()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.D, 8));
            var pieces = new List<Piece>()
            {
                new Pawn { Color = Color.White, Position = new(File.C, 7) },
                new Bishop { Color = Color.Black, Position = new(File.D, 8)}
            };

            //Act
            var result = Board.PieceIsCaptured(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.B, 2)]
        [DataRow(File.D, 4)]
        [DataRow(File.D, 4)]
        public void A1_To_E5_IsBlocked(File file, int rank)
        {
            //Arrange
            var pieces = new List<Piece>
            {
                new Pawn { Color = Color.Black, Position = new(file, rank)}
            };

            //Act
            var result = Board.DirectionIsObstructed(pieces, new(File.A, 1), new(File.E, 5));

            //Assert
            result.ShouldNotBeNull().ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.D, 5)]
        [DataRow(File.E, 5)]
        public void A1_To_E5_IsNotBlocked(File file, int rank)
        {
            //Arrange
            var pieces = new List<Piece>
            {
                new Pawn { Color = Color.Black, Position = new(file, rank) }
            };

            //Act
            var result = Board.DirectionIsObstructed(pieces, new(File.A, 1), new(File.E, 5));

            //Assert
            result.ShouldNotBeNull().ShouldBeFalse();
        }
        [TestMethod]
        public void KingC7_IsNotInCheck_IsProtected_ByPawnC6()
        {
            //Arrange
            var king = new King { Position = new(File.C, 7), Color = Color.Black };
            var pieces = new List<Piece>
            {
                new Pawn { Position = new(File.C, 6), Color = Color.Black },
                new Rook { Position = new(File.C, 1), Color = Color.White }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeFalse();
        }

        [TestMethod]
        [DataRow(File.C, 1)]
        [DataRow(File.A, 7)]
        public void KingC7_IsCheck_ByRook(File file, int rank)
        {
            //Arrange
            var king = new King { Position = new(File.C, 7), Color = Color.Black };
            var pieces = new List<Piece>()
            {
                new Rook { Position = new(file, rank), Color = Color.White }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.A, 1)]
        [DataRow(File.C, 5)]
        public void KingC1_IsCheck_ByRook(File file, int rank)
        {
            //Arrange
            var king = new King { Position = new(File.C, 1), Color = Color.White };
            var pieces = new List<Piece>()
            {
                new Rook { Position = new(file, rank), Color = Color.Black }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }
        [TestMethod]
        [DataRow(File.A, 5)]
        [DataRow(File.E, 5)]
        public void KingC7_IsInCheck_ByQueen(File file, int rank)
        {
            //Arrange
            var king = new King { Position = new(File.C, 7), Color = Color.Black };
            var pieces = new List<Piece>
            {
                new Queen { Position = new(file, rank), Color = Color.White }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.A, 3)]
        [DataRow(File.E, 3)]
        public void KingC1_IsInCheck_ByQueen(File file, int rank)
        {
            //Arrange
            var king = new King { Position = new(File.C, 1), Color = Color.White };
            var pieces = new List<Piece>
            {
                new Queen { Position = new(file, rank), Color = Color.Black }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.B, 6)]
        [DataRow(File.D, 6)]
        public void KingC7_IsInCheck_ByPawn(File file, int rank)
        {
            var king = new King { Position = new(File.C, 7), Color = Color.Black };
            var pieces = new List<Piece>
            {
                new Pawn { Position = new(File.C, 6), Color = Color.Black },
                new Queen { Position = new(File.D, 7), Color = Color.Black },
                new Bishop { Position = new(File.D, 7), Color = Color.Black },
                new Pawn { Position = new(file, rank), Color = Color.White }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        [DataRow(File.B, 5)]
        [DataRow(File.D, 5)]
        public void KingC7_IsInCheck_ByKnight(File file, int rank)
        {
            var king = new King { Position = new(File.C, 7), Color = Color.Black };
            var pieces = new List<Piece>
            {
                new Pawn { Position = new(File.B, 6), Color = Color.Black },
                new Pawn { Position = new(File.C, 6), Color = Color.Black },
                new Queen { Position = new(File.D, 7), Color = Color.Black },
                new Pawn { Position = new(File.D, 6), Color = Color.Black },
                new Bishop { Position = new(File.D, 7), Color = Color.Black },
                new Knight { Position = new(file, rank), Color = Color.White }
            };

            //Act
            var result = Board.IsCheck(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

    }
}
