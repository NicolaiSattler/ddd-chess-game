using System.Text;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Utilities;

public class NotationBuilder
{
    private const char CapturedSymbol = 'x';
    private const char CheckSymbol = '+';
    private const char CheckMateSymbol = '#';
    private const string CastlingKingsideSymbols = "0-0";
    private const string CastlingQueensideSymbols = "0-0-0";

    private readonly StringBuilder _builder;

    public NotationBuilder()
    {
        _builder = new();
    }

    public NotationBuilder HasPiece(PieceType pieceType)
    {
        if (pieceType == PieceType.Pawn) return this;

        var pieceNotation = GetPieceNotation(pieceType);
        _builder.Append(pieceNotation);

        return this;
    }

    public NotationBuilder HasCapturedPiece(Piece movingPiece)
    {
        if (movingPiece is Pawn)
        {
            _builder.Append(movingPiece.Position.File);
        }

        _builder.Append(CapturedSymbol);

        return this;
    }

    public NotationBuilder IsCheck()
    {
        _builder.Append(CheckSymbol);

        return this;
    }

    public NotationBuilder IsCheckMate()
    {
        _builder.Append(CheckMateSymbol);

        return this;
    }

    public NotationBuilder IsCastling(CastlingType type)
    {
        if (type == CastlingType.KingSide)
        {
            _builder.Append(CastlingKingsideSymbols);
        }
        else if (type == CastlingType.QueenSide)
        {
            _builder.Append(CastlingQueensideSymbols);
        }

        return this;
    }

    public NotationBuilder IsPromotion(PieceType promotionType)
    {
        var pieceNotation = GetPieceNotation(promotionType);

        _builder.Append($"={pieceNotation}");

        return this;
    }

    public NotationBuilder EndsAtPosition(Piece piece)
    {
        _builder.Append(piece.Position.ToString());

        return this;
    }

    public string Build() => _builder.ToString();

    private static char GetPieceNotation(PieceType pieceType) => pieceType switch
    {
        PieceType.King => 'K',
        PieceType.Queen => 'Q',
        PieceType.Rook => 'R',
        PieceType.Bishop => 'B',
        PieceType.Knight => 'N',
        _ => throw new IndexOutOfRangeException("Unknown PieceType")
    };
}
