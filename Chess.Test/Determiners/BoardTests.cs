using System;
using System.Collections.Generic;
using Chess.Domain.Determiners;
using Chess.Domain.Events;

namespace Chess.Test.Domain.Determiners
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void PawnC7_MovesToC8_IsPromoted()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.C, 8));
            var pawn = new Pawn { Color = Color.White, Position = new(File.C, 7) };
            var pieces = new List<Piece>() { pawn };

            //Act
            var result = SpecialMoves.PawnIsPromoted(pawn, command.EndPosition);

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

        [TestMethod]
        public void KingH8_IsCheckmate_ByRookG8_AndKnightF6()
        {
            //Arrange
            var king = new King { Position = new(File.H, 8), Color = Color.Black };
            var pieces = new List<Piece>
            {
                king,
                new Rook { Position = new(File.G, 8), Color = Color.White },
                new Knight { Position = new(File.F, 6), Color = Color.White }
            };

            //Act
            var result = Board.IsCheckMate(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void KingH8_IsCheckmate_ByRookE8_AndBlockedByOwnPieces()
        {
            //Arrange
            var king = new King { Position = new(File.F, 8), Color = Color.Black };
            var pieces = new List<Piece>
            {
                king,
                new Bishop { Position = new(File.D, 8), Color = Color.Black },
                new Rook { Position = new(File.H, 8), Color = Color.Black },
                new Pawn { Position = new(File.F, 7), Color = Color.Black },
                new Pawn { Position = new(File.G, 7), Color = Color.Black },
                new Rook { Position = new(File.E, 8), Color = Color.White },
                new Knight { Position = new(File.F, 6), Color = Color.White }
            };

            //Act
            var result = Board.IsCheckMate(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void KingH8_IsNotCheckmate_OpponentPieceCanBeAttacked()
        {
            //Arrange
            var king = new King { Position = new(File.F, 8), Color = Color.Black };
            var pieces = new List<Piece>
            {
                king,
                new Bishop { Position = new(File.D, 8), Color = Color.Black },
                new Bishop { Position = new(File.D, 7), Color = Color.Black },
                new Rook { Position = new(File.H, 8), Color = Color.Black },
                new Pawn { Position = new(File.F, 7), Color = Color.Black },
                new Pawn { Position = new(File.G, 7), Color = Color.Black },
                new Rook { Position = new(File.E, 8), Color = Color.White },
            };

            //Act
            var result = Board.IsCheckMate(king, pieces);

            //Assert
            result.ShouldBeFalse();
        }

        [TestMethod]
        public void KingH8_IsCheckmate_ByMultiplePieces()
        {
            //Arrange
            var king = new King { Position = new(File.F, 8), Color = Color.Black };
            var pieces = new List<Piece>
            {
                king,
                new Bishop { Position = new(File.D, 8), Color = Color.Black },
                new Bishop { Position = new(File.D, 7), Color = Color.Black },
                new Rook { Position = new(File.H, 8), Color = Color.Black },
                new Pawn { Position = new(File.F, 7), Color = Color.Black },
                new Pawn { Position = new(File.G, 7), Color = Color.Black },
                new Rook { Position = new(File.E, 8), Color = Color.White },
                new Rook { Position = new(File.G, 6), Color = Color.White },
            };

            //Act
            var result = Board.IsCheckMate(king, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void KingH8_IsCheck_CanBeLifted()
        {
            //Arrange
            var king = new King { Position = new(File.F, 8), Color = Color.Black };
            var pieces = new List<Piece>
            {
                king,
                new Pawn {Position = new(File.E, 8), Color = Color.Black},
                new Pawn {Position = new(File.E, 7), Color = Color.Black},
                new Pawn {Position = new(File.G, 8), Color = Color.Black},
                new Pawn {Position = new(File.G, 7), Color = Color.Black},
                new Rook {Position = new(File.A, 2), Color = Color.Black},
                new Rook {Position = new(File.F, 1), Color = Color.White},
            };

            //Act
            var result = Board.IsCheckMate(king, pieces);

            //Assert
            result.ShouldBeFalse();
        }

        [TestMethod]
        public void IsStalemate_ScenarioA()
        {
            //Arrange
            var blackKing = new King { Position = new(File.B, 6), Color = Color.Black };
            var pieces = new List<Piece>
            {
                blackKing,
                new Rook{Position = new(File.A, 7), Color = Color.White},
                new Rook{Position = new(File.E, 6), Color = Color.White},
                new Bishop{Position = new(File.C, 6), Color = Color.White},
                new Bishop{Position = new(File.F, 8), Color = Color.White},
                new Queen{Position = new(File.G, 7), Color = Color.White},
                new King{Position = new(File.H, 2), Color = Color.White},
            };

            //Act
            var result = Board.IsStalemate(Color.Black, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void IsStalemate_Scenario_EhrhardtVsNimzowitsch_ReturnsFalse()
        {
            //Arrange
            var pieces = new List<Piece>
            {
                new King { Position = new(File.B, 3), Color = Color.Black},
                new Pawn { Position = new(File.A, 2), Color = Color.Black},
                new King { Position = new(File.A, 1), Color = Color.White},
            };

            //Act
            var result = Board.IsStalemate(Color.White, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        /// <summary>
        /// https://www.chess.com/terms/stalemate-chess
        /// </summary>
        [TestMethod]
        public void IsStalemate_Scenario_PuzzleTwo()
        {
            //Arrange
            var pieces = new List<Piece>
            {
                new King { Position = new(File.H, 1), Color = Color.White },
                new Pawn { Position = new(File.H, 2), Color = Color.White },
                new Pawn { Position = new(File.H, 3), Color = Color.Black },
                new Pawn { Position = new(File.F, 4), Color = Color.Black },
                new King { Position = new(File.G, 8), Color = Color.Black },
                new Bishop { Position = new(File.A, 7), Color = Color.Black },
            };

            //Act
            var result = Board.IsStalemate(Color.White, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void IsStalemate_Scenario_EvansVsReshevsky()
        {
            //Arrange
            var pieces = new List<Piece>
            {
                new King { Position = new(File.H, 1), Color = Color.White },
                new Pawn { Position = new(File.B, 4), Color = Color.White },
                new Pawn { Position = new(File.E, 4), Color = Color.White },
                new Pawn { Position = new(File.F, 3), Color = Color.White },
                new Pawn { Position = new(File.H, 4), Color = Color.White },
                new King { Position = new(File.G, 8), Color = Color.Black },
                new Pawn { Position = new(File.B, 5), Color = Color.Black },
                new Pawn { Position = new(File.E, 5), Color = Color.Black },
                new Rook { Position = new(File.E, 2), Color = Color.Black },
                new Knight { Position = new(File.F, 4), Color = Color.Black },
                new Queen { Position = new(File.G, 3), Color = Color.Black },
                new Pawn { Position = new(File.H, 5), Color = Color.Black }
            };

            //Act
            var result = Board.IsStalemate(Color.White, pieces);

            //Assert
            result.ShouldBeTrue();
        }
    }
}
