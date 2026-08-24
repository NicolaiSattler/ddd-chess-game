using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Ardalis.GuardClauses;

using Chess.Core;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Configuration;
using Chess.Domain.Determiners;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using Chess.Domain.Extensions;
using Chess.Domain.Factories;
using Chess.Domain.Models;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

using FluentResults;

namespace Chess.Domain.Aggregates;

public class Match : AggregateRoot, IMatch
{
    private List<Piece> _pieces = [];
    private List<Turn> _turns = [];

    public MatchOptions Options { get; private set; } = new();
    public Player White { get; private set; } = new();
    public Player Black { get; private set; } = new();

    public IReadOnlyList<Piece> Pieces => _pieces.AsReadOnly();
    public IReadOnlyList<Turn> Turns => _turns.AsReadOnly();

    public Match() : base(Guid.Empty) { }
    public Match(Guid id) : base(id) { }
    public Match(Guid id, List<DomainEvent> events) : base(id, events) { }

    protected override void When(DomainEvent? domainEvent)
    {
        if (domainEvent is MatchStarted matchStarted) Handle(matchStarted);
        if (domainEvent is TurnTaken turnTaken) Handle(turnTaken);
        if (domainEvent is PawnPromoted pawnPromoted) Handle(pawnPromoted);
        if (domainEvent is MatchEnded matchEnded) Handle(matchEnded);
    }

    public void Start(StartMatch command)
    {
        Guard.Against.InvalidInput(command,
                                   nameof(command),
                                   (cmd) => cmd.MemberOne.MemberId != cmd.MemberTwo.MemberId,
                                   Constants.InvalidStartMatchError);

        var colorPicker = new Random(1);
        var memberOneIsWhite = colorPicker.Next() == 0;
        var white = memberOneIsWhite ? command.MemberOne : command.MemberTwo;
        var black = !memberOneIsWhite ? command.MemberOne : command.MemberTwo;
        var @event = MatchStarted.CreateFrom(command, white, black);

        RaiseEvent(@event);
    }

    //TODO: Unit Test all scenario's
    //TODO: Draw by repitition
    public TurnResult TakeTurn(TakeTurn command)
    {
        // Optimistic concurrency control: check if aggregate version matches expected
        if (command.ExpectedVersion.HasValue && command.ExpectedVersion.Value != Version)
        {
            return new TurnResult
            {
                TurnValidation = Result.Fail(new ConcurrencyError())
            };
        }

        var validationResult = TurnRules.Validate(command, _pieces, _turns);

        if (!validationResult.IsSuccess)
        {
            return new TurnResult
            {
                TurnValidation = validationResult
            };
        }

        var @event = TurnTaken.CreateFrom(command);
        RaiseEvent(@event);

        var activePiece = _pieces.Find(p => p.Position == command.StartPosition);
        var castlingType = SpecialMoves.IsCastling(command.StartPosition, command.EndPosition, _pieces);
        var isEnPassant = activePiece is Pawn pawn && SpecialMoves.IsEnPassant(pawn, _turns);
        var isPromotion = activePiece is Pawn && SpecialMoves.PawnIsPromoted(activePiece, command.EndPosition);

        return new()
        {
            CastlingType = castlingType,
            IsEnPassant = isEnPassant,
            IsPromotion = isPromotion,
            TurnValidation = Result.Ok()
        };
    }

    //TODO: Unit test...
    public void PromotePiece(Promotion command)
    {
        var @event = PawnPromoted.CreateFrom(command);

        RaiseEvent(@event);
    }

    //TODO: Unit test...
    public void Forfeit(Forfeit command)
    {
        Guard.Against.Null(command, nameof(command));

        var matchResult = command.MemberId == White.MemberId
                        ? MatchResult.BlackWins
                        : MatchResult.WhiteWins;

        var @event = new MatchEnded(White, Black, matchResult);

        RaiseEvent(@event);
    }

    //TODO: Unit test aggregate
    public void Surrender(Surrender command)
    {
        Guard.Against.InvalidInput(command.MemberId,
                                   nameof(command.MemberId),
                                   (memberId) => memberId != Guid.Empty);

        var matchResult = command.MemberId == White.MemberId
                        ? MatchResult.WhiteSurrenders
                        : MatchResult.BlackSurrenders;

        var @event = new MatchEnded(White, Black, matchResult);

        RaiseEvent(@event);
    }


    //TODO: Unit test aggregate
    public void Draw()
    {
        var @event = new MatchEnded(White, Black, MatchResult.Draw);

        RaiseEvent(@event);
    }

    private void Handle(MatchStarted @event)
    {
        Options = @event.Options;
        White = new() { Color = Color.White, MemberId = @event.WhiteMemberId, Elo = @event.WhiteElo };
        Black = new() { Color = Color.Black, MemberId = @event.BlackMemberId, Elo = @event.BlackElo };
        _turns = new();

        _pieces =
        [
            .. PieceFactory.CreatePiecesForColor(Color.White),
            .. PieceFactory.CreatePiecesForColor(Color.Black),
        ];

        StartTurn(@event.StartTime);
    }

    //TODO: test notation
    private void Handle(TurnTaken @event)
    {
        Guard.Against.Null(@event, nameof(@event));
        Guard.Against.Null(_turns, nameof(_turns));

        var movingPiece = _pieces.Find(p => p.Position == @event.StartPosition);

        if (movingPiece == null) return;

        var targetPiece = _pieces.Find(p => p.Position == @event.EndPosition);
        var isEnPassant = SpecialMoves.IsEnPassant(movingPiece, _turns);
        var pieceIsCaptured = Board.PieceIsCaptured(@event, _pieces) || isEnPassant;
        var castling = SpecialMoves.IsCastling(@event.StartPosition, @event.EndPosition, _pieces);

        if (isEnPassant)
        {
            var turnCount = _turns.Count;
            var position = _turns.ElementAt(turnCount - 2).EndPosition;
            targetPiece = _pieces.Find(p => p.Position == position);
        }

        if (castling != CastlingType.Undefined) MoveCastingPieces(movingPiece, @event.EndPosition);

        if (targetPiece != null && pieceIsCaptured) _pieces.Remove(targetPiece);

        movingPiece.MoveTo(@event.EndPosition);

        var isCheckMate = IsCheckMate(@event);
        var isStalemate = Board.IsStalemate(movingPiece.Color, _pieces);
        var isCheck = OpponentIsInCheck(movingPiece.Color);
        var notation = DetermineNotation(movingPiece, targetPiece, castling, isCheck, isCheckMate);

        EndTurn(@event, movingPiece.Type, notation);

        if (isCheckMate)
        {
            var matchResult = GetMatchResult(@event.MemberId);
            RaiseEvent(new MatchEnded(White, Black, matchResult));
        }
        else if (isStalemate)
        {
            RaiseEvent(new MatchEnded(White, Black, MatchResult.Stalemate));
        }
        else
        {
            StartTurn(@event.EndTime);
        }
    }

    private void Handle(PawnPromoted @event)
    {
        Guard.Against.Null(@event, nameof(@event));

        var pawn = _pieces.Find(p => p.Position == @event.PawnPosition && p.Type == PieceType.Pawn);
        var newPieceType = PieceFactory.CreatePiece(@event.PromotionType, pawn!.Position, pawn!.Id, pawn!.Color);

        _pieces.Remove(pawn);
        _pieces.Add(newPieceType);

        var lastTurn = _turns.ElementAt(_turns.Count - 2);
        lastTurn.UpdateNotation(lastTurn.Notation + $"={@event.PromotionType.GetPieceNotation()}");
    }

    //TODO: Notify Client that the match has ended
    private void Handle(MatchEnded @event)
    {
        var whiteId = Guard.Against.Null(White, nameof(White)).MemberId;
        var blackId = Guard.Against.Null(Black, nameof(Black)).MemberId;
        var result = Elo.Calculate(White.Elo, Black.Elo, @event.Result);

        if (result != null)
        {
            White = new() { MemberId = whiteId, Color = Color.White, Elo = result.WhiteElo };
            Black = new() { MemberId = blackId, Color = Color.Black, Elo = result.BlackElo };
        }

        //TODO: Save match event..
    }

    private static string DetermineNotation(Piece movingPiece, Piece? targetPiece, CastlingType castling, bool isCheck, bool isCheckMate)
    {
        var notation = new NotationBuilder();

        notation.HasPiece(movingPiece.Type);

        if (castling != CastlingType.Undefined)
            notation.IsCastling(castling);
        else if (targetPiece != null)
            notation.HasCapturedPiece(movingPiece);

        notation.EndsAtPosition(movingPiece);

        if (isCheck)
            notation.IsCheck();

        if (isCheckMate)
            notation.IsCheckMate();

        return notation.Build();
    }

    private bool OpponentIsInCheck(Color currentPlayerColor)
    {
        var piece = _pieces.FirstOrDefault(p => p.Color != currentPlayerColor && p.Type == PieceType.King);
        return piece is King king && Board.IsCheck(king, _pieces);
    }

    private Player GetOpponent(Guid memberId) => memberId != White.MemberId ? White : Black;

    private void StartTurn(DateTime startTime)
    {
        var player = White;

        if (_turns.Count != 0)
        {
            var playerAtTurn = _turns.Last().Player.MemberId;
            player = GetOpponent(playerAtTurn);
        }

        _turns.Add(new() { Player = player, StartTime = startTime });
    }

    //TODO: Unit Test in aggregate
    private void EndTurn(TurnTaken @event, PieceType pieceType, string notation)
    {
        @event = Guard.Against.Null(@event, nameof(@event));
        pieceType = Guard.Against.Null(pieceType, nameof(pieceType));

        var turn = _turns.Last() ?? throw new InvalidOperationException("No turns found!");
        var player = White.MemberId == @event.MemberId ? White : Black;

        turn.UpdateMoveData(pieceType, @event.StartPosition, @event.EndPosition, CalculateHash(player.Color), notation);
    }

    //TODO: Unit Test in aggregate
    private void MoveCastingPieces(Piece king, Square endPosition)
    {
        if (king == null) return;

        var rank = king.Position.Rank;
        var file = endPosition.File > File.E ? File.H : File.A;
        var newFilePosition = file == File.H ? File.F : File.D;
        var rookPosition = new Square(file, rank);
        var rook = _pieces.FirstOrDefault(p => p.Position == rookPosition);

        if (rook != null)
        {
            rook.MoveTo(new Square(newFilePosition, rank));
        }
    }

    //TODO: Unit Test in aggregate
    private bool IsCheckMate(TurnTaken @event)
    {
        var player = @event.MemberId == Black.MemberId ? Black : White;
        var piece = _pieces.Find(p => p.Color != player.Color && p.Type == PieceType.King);

        if (piece is King king)
        {
            return Board.IsCheckMate(king, _pieces);
        }

        return false;
    }

    private string CalculateHash(Color color)
    {
        const string separator = "";

        var pieceNotations = _pieces.Where(p => p.Color == color)
                                   .Select(p => p.ToString());

        var aggregate = string.Join(separator, pieceNotations);
        var inputBytes = Encoding.UTF8.GetBytes(aggregate);
        var hexdecimalCollection = MD5.HashData(inputBytes)
                                      .Select(m => m.ToString("x2"));

        return string.Join(separator, hexdecimalCollection);
    }

    private MatchResult GetMatchResult(Guid? memberId) => memberId == White.MemberId ? MatchResult.WhiteWins : MatchResult.BlackWins;
}