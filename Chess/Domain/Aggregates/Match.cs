using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.Commands;
using Chess.Domain.Configuration;
using Chess.Domain.Determiners;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using Chess.Domain.Factories;
using Chess.Domain.ValueObjects;
using Chess.Domain.Models;
using Chess.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Chess.Domain.Aggregates;

public class Match : AggregateRoot, IMatch
{
    public MatchOptions Options { get; private set; } = new();
    public Player White { get; private set; } = new();
    public Player Black { get; private set; } = new();
    public List<Piece> Pieces { get; private set; } = new();
    public List<Turn> Turns { get; private set; } = new();

    public Match(): base(Guid.Empty) {}
    public Match(Guid id) : base(id) { }
    public Match(Guid id, IEnumerable<DomainEvent?>? events) : base(id, events) { }


    protected override void When(DomainEvent? domainEvent)
    {
        if (domainEvent is MatchStarted matchStarted) Handle(matchStarted);
        if (domainEvent is TurnTaken turnTaken) Handle(turnTaken);
        if (domainEvent is MatchEnded matchEnded) Handle(matchEnded);
    }

    public void Start(StartMatch command)
    {
        Guard.Against.InvalidInput(command,
                                   nameof(command),
                                   (cmd) => cmd.MemberOneId != cmd.MemberTwoId,
                                   Constants.InvalidStartMatchError);

        var colorPicker = new Random(1);
        var memberOneIsWhite = colorPicker.Next() == 0;
        var whiteId = memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var blackId = !memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var @event = new MatchStarted()
        {
            AggregateId = command.AggregateId,
            WhiteMemberId = whiteId,
            BlackMemberId = blackId,
            StartTime = DateTime.UtcNow
        };

        RaiseEvent(@event);
    }

    //TODO: Unit Test all scenario's
    //TODO: Draw by repitition
    public TurnResult TakeTurn(TakeTurn command)
    {
        var matchResult = MatchResult.Undefined;
        var violations = RuleFactory.GetTurnRules(command, Pieces, Turns)
                                    .SelectMany(r => r.CheckRule())
                                    .ToList();

        Func<Guid?, MatchResult> GetMatchResult = (memberId)
            => memberId == White.MemberId ? MatchResult.WhiteWins : MatchResult.BlackWins;

        if (!violations.Any())
        {
            var isCheckmate = IsCheckMate(command);

            if (isCheckmate)
            {
                matchResult = GetMatchResult(command.MemberId);
                var matchEndedEvent = new MatchEnded(White, Black, matchResult);

                RaiseEvent(matchEndedEvent);
                return new() { MatchResult = matchResult };
            }

            var isStalemate = IsStalemate(command);

            if (isStalemate)
            {
                RaiseEvent(new MatchEnded(White, Black, MatchResult.Draw));
                return new() { MatchResult = MatchResult.Stalemate };
            }

            var turnTakenEvent = new TurnTaken(command.MemberId, command.StartPosition, command.EndPosition);
            RaiseEvent(turnTakenEvent);
        }

        return new() { Violations = violations };
    }

    public void Forfeit(Forfeit command)
    {
        Guard.Against.Null<Forfeit>(command, nameof(command));

        var matchResult = command.MemberId == White.MemberId
                        ? MatchResult.WhiteForfeit
                        : MatchResult.BlackForfeit;

        var @event = new MatchEnded(White, Black, matchResult);

        RaiseEvent(@event);
    }

    //TODO: Unitt test aggregate
    public void Resign(Resign command)
    {
        Guard.Against.InvalidInput(command.MemberId,
                                   nameof(command.MemberId),
                                   (memberId) => memberId != Guid.Empty);

        var matchResult = command.MemberId == White.MemberId
                        ? MatchResult.WhiteWins
                        : MatchResult.BlackWins;

        var @event = new MatchEnded(White, Black, matchResult);

        RaiseEvent(@event);
    }


    //TODO: Unit test aggregate
    public void Draw(Draw command)
    {
        if (command.Accepted)
        {
            var @event = new MatchEnded(White, Black, MatchResult.Draw);

            RaiseEvent(@event);
        }
    }

    private void Handle(MatchStarted @event)
    {
        Options = @event.Options;
        White = new() { Color = Color.White, MemberId = @event.WhiteMemberId, Elo = @event.EloOfWhite };
        Black = new() { Color = Color.Black, MemberId = @event.BlackMemberId, Elo = @event.EloOfBlack };
        Turns = new();

        Pieces = new();
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.White));
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.Black));

        StartTurn(@event.StartTime);
    }

    //TODO: test notation
    private void Handle(TurnTaken @event)
    {
        Guard.Against.Null<TurnTaken>(@event, nameof(@event));
        Guard.Against.Null<List<Turn>>(Turns, nameof(Turns));

        var movingPiece = Pieces.FirstOrDefault(p => p.Position == @event.StartPosition);

        if (movingPiece == null) return;

        var targetPiece = Pieces.FirstOrDefault(p => p.Position == @event.EndPosition);
        var isEnPassant = SpecialMoves.IsEnPassant(movingPiece, Turns);
        var castling = SpecialMoves.IsCastling(@event.StartPosition, @event.EndPosition, Pieces);
        var pieceIsCaptured = Board.PieceIsCaptured(@event, Pieces) || isEnPassant;
        var isCheck = PlayerIsInCheck(movingPiece.Color);
        var promotionType = Promotion(@event, movingPiece);
        var notation = DetermineNotation(movingPiece, targetPiece, castling, promotionType, isCheck);

        if (isEnPassant)
        {
            var pieceId = Turns.Last().Id;
            targetPiece = Pieces.FirstOrDefault(p => p.Id == pieceId);
        }

        if (castling != CastlingType.Undefined)
        {
            MoveCastingPieces(movingPiece, @event.EndPosition);
        }

        if (targetPiece != null && pieceIsCaptured)
        {
            Pieces.Remove(targetPiece);
        }

        movingPiece.Position = @event.EndPosition;

        EndTurn(@event, movingPiece.Type, notation);
        StartTurn(DateTime.UtcNow);
    }

    private string DetermineNotation(Piece movingPiece,
                                     Piece? targetPiece,
                                     CastlingType castling,
                                     PieceType? promotionType,
                                     bool isCheck)
    {
        var notation = new NotationBuilder();

        notation.HasPiece(movingPiece.Type);

        if (castling != CastlingType.Undefined)
            notation.IsCastling(castling);
        else if (targetPiece != null)
            notation.HasCapturedPiece(targetPiece);

        notation.EndsAtPosition(movingPiece);

        if (promotionType != null)
            notation.IsPromotion(promotionType.Value);

        if (isCheck)
            notation.IsCheck();

        return notation.Build();
    }

    private bool PlayerIsInCheck(Color color)
    {
        var king = Pieces.FirstOrDefault(p => p.Color == color && p.Type == PieceType.King) as King;
        return king != null ? Board.IsCheck(king, Pieces) : false;
    }

    private void Handle(MatchEnded @event)
    {
        var whiteId = Guard.Against.Null<Player>(White, nameof(White)).MemberId;
        var blackId = Guard.Against.Null<Player>(Black, nameof(Black)).MemberId;
        var result = Elo.Calculate(White.Elo, Black.Elo, @event.Result);

        if (result != null)
        {
            White = new() { MemberId = whiteId, Color = Color.White, Elo = result.WhiteElo };
            Black = new() { MemberId = blackId, Color = Color.Black, Elo = result.BlackElo };
        }
    }

    private Player GetOpponent(Guid memberId)
        => memberId != White.MemberId ? White : Black;

    private void StartTurn(DateTime startTime)
    {
        var player = White;

        if (Turns.Any())
        {
            var playerAtTurn = Turns.Last().Player.MemberId;
            player = GetOpponent(playerAtTurn);
        }

         Turns.Add(new() { Player = player, StartTime = startTime });
    }

    //TODO: Unit Test in aggregate
    private void EndTurn(TurnTaken @event, PieceType pieceType, string notation)
    {
        @event = Guard.Against.Null<TurnTaken>(@event, nameof(@event));
        pieceType = Guard.Against.Null<PieceType>(pieceType, nameof(pieceType));

        var turn = Turns.LastOrDefault() ?? throw new InvalidOperationException("No turns found!");
        var player = White.MemberId == @event.MemberId ? White : Black;

        turn.StartPosition = @event.StartPosition;
        turn.EndPosition = @event.EndPosition;
        turn.PieceType = pieceType;
        turn.Hash = CalculateHash(player.Color);
        turn.Notation = notation;
    }

    //TODO: Unit Test in aggregate
    //TODO: User should be given a choice to which kind the piece it will be promoted to.
    private PieceType? Promotion(TurnTaken @event, Piece movingPiece)
    {
        if (SpecialMoves.PawnIsPromoted(movingPiece, @event.EndPosition) && movingPiece != null)
        {
            var queen = PiecesFactory.CreatePiece(PieceType.Queen, @event.EndPosition, movingPiece.Id, movingPiece.Color);

            Pieces.Remove(movingPiece);
            Pieces.Add(queen);

            return queen.Type;
        }

        return null;
    }

    //TODO: Unit Test in aggregate
    private void MoveCastingPieces(Piece king, Square endPosition)
    {
        if (king == null) return;

        var rank = king.Color == Color.Black ? 8 : 1;
        var file = endPosition.File > File.E ? File.H : File.A;
        var newFilePosition = file == File.H ? File.F : File.D;
        var rookPosition = new Square(file, rank);
        var rook = Pieces.FirstOrDefault(p => p.Position == rookPosition);

        if (rook != null)
        {
            rook.Position = new Square(newFilePosition, rank);
        }
    }

    //TODO: Unit Test in aggregate
    private bool IsCheckMate(TakeTurn turn)
    {
        var player = turn.MemberId == Black.MemberId ? Black : White;
        var piece = Pieces.FirstOrDefault(p => p.Color != player.Color && p.Type == PieceType.King);

        if (piece is King king)
        {
            return Board.IsCheckMate(king, Pieces);
        }

        return false;
    }

    //TODO: Unit Test in aggregate
    private bool IsStalemate(TakeTurn command)
    {
        var movingPiece = Pieces.First(p => p.Position == command.StartPosition)
                ?? throw new InvalidOperationException("Piece not found!");

        return Board.IsStalemate(movingPiece.Color, Pieces);
    }

    public string CalculateHash(Color color)
    {
        const string separator = "";
        using var md5 = MD5.Create();

        var pieceNotations = Pieces.Where(p => p.Color == color)
                                   .Select(p => p.ToString());
        var aggregate = string.Join(separator, pieceNotations);
        var inputBytes = Encoding.UTF8.GetBytes(aggregate);

        var hexdecimalCollection = md5.ComputeHash(inputBytes)
                                      .Select(m => m.ToString("x2"));

        return string.Join(separator, hexdecimalCollection);
    }
}
