using System.Text;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Utilities;

public class NotationBuilder
{
    private readonly StringBuilder _builder;

    public NotationBuilder()
    {
        _builder = new();
    }

    public NotationBuilder HasPiece(PieceType pieceType)
    {
        var pieceNotation = GetPieceNotation(pieceType);
        _builder.Append(pieceNotation);

        return this;
    }

    public NotationBuilder HasCapturedPiece(Piece piece)
    {
        if (piece is Pawn)
        {
            _builder.Append(piece.Position.File);
        }

        _builder.Append("x");

        return this;
    }

    public NotationBuilder IsCheck()
    {
        _builder.Append("+");

        return this;
    }

    public NotationBuilder IsCheckMate()
    {
        _builder.Append("#");

        return this;
    }

    public NotationBuilder IsCastling(CastlingType type)
    {
        if (type == CastlingType.KingSide)
        {
            _builder.Append("0-0");
        }
        else if (type == CastlingType.QueenSide)
        {
            _builder.Append("0-0-0");
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

    private string GetPieceNotation(PieceType pieceType) => pieceType switch
    {
        PieceType.King => "K",
        PieceType.Queen => "Q",
        PieceType.Rook => "R",
        PieceType.Bishop => "B",
        PieceType.Knight => "N",
        PieceType.Pawn => "P",
        _ => throw new IndexOutOfRangeException("Unknown PieceType")
    };
}
