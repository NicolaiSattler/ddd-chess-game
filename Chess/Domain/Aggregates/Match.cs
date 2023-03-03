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
using System.Collections.Generic;
using System.Linq;

namespace Chess.Domain.Aggregates;

public class Match : AggregateRoot<Guid>, IMatch
{
    private List<Piece>? Pieces { get; set; }
    private List<Turn>? Turns { get; set; }

    public MatchOptions? Options { get; private set; }
    public Player? White { get; private set; }
    public Player? Black { get; private set; }

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
                                   (cmd) => cmd.MemberOneId == cmd.MemberTwoId,
                                   Constants.InvalidStartMatchError);

        var colorPicker = new Random(1);
        var memberOneIsWhite = colorPicker.Next() == 0;
        var whiteId = memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var blackId = !memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var @event = new MatchStarted()
        {
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
                                    .SelectMany(r => r.CheckRule());

        Func<Guid?, MatchResult> GetMatchResult = (memberId)
            => memberId == White?.MemberId ? MatchResult.WhiteWins : MatchResult.BlackWins;

        if (!violations.Any())
        {
            var isCheckmate = IsCheckMate(command);

            if (isCheckmate)
            {
                matchResult = GetMatchResult(command.MemberId);
                var @event = new MatchEnded(White, Black, matchResult);

                RaiseEvent(@event);
                return new() { MatchResult = matchResult };
            }

            var isStalemate = IsStalemate(command);

            if (isStalemate)
            {
                RaiseEvent(new MatchEnded(White, Black, MatchResult.Draw));
                return new() { MatchResult = MatchResult.Stalemate };
            }

            RaiseEvent(new TurnTaken(command.MemberId, command?.StartPosition, command?.EndPosition));
        }

        return new() { Violations = violations };
    }

    public void Forfeit(ForfeitCommand command)
    {
        Guard.Against.Null<ForfeitCommand>(command, nameof(command));

        var matchResult = command.MemberId == White?.MemberId
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

        var matchResult = command.MemberId == White?.MemberId
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

        StartTurn(White, @event.StartTime);
    }

    private void Handle(TurnTaken @event)
    {
        Guard.Against.Null<TurnTaken>(@event, nameof(@event));

        var movingPiece = Pieces?.FirstOrDefault(p => p.Position == @event.StartPosition);

        if (movingPiece == null) return;

        var targetPiece = Pieces?.FirstOrDefault(p => p.Position == @event.EndPosition);
        var isEnPassant = SpecialMoves.IsEnPassant(movingPiece, Turns);
        var isCastling = SpecialMoves.IsCastling(@event?.StartPosition, @event?.EndPosition, Pieces);
        var pieceIsCaptured = Board.PieceIsCaptured(@event, Pieces) || isEnPassant;

        if (isEnPassant)
        {
            var pieceId = Turns?.Last().Id;
            targetPiece = Pieces?.FirstOrDefault(p => p.Id == pieceId);
        }

        if (isCastling)
            MoveCastingPieces(movingPiece, @event?.EndPosition);

        if (targetPiece != null && pieceIsCaptured)
            Pieces?.Remove(targetPiece);

        CheckPromotion(@event, movingPiece);

        movingPiece.Position = @event?.EndPosition;

        EndTurn(@event, movingPiece?.Type);
        StartTurn(GetOpponent(@event?.MemberId), DateTime.UtcNow);
    }

    private void Handle(MatchEnded @event)
    {
        var whiteId = Guard.Against.Null<Player?>(White, nameof(White))!.MemberId;
        var blackId = Guard.Against.Null<Player?>(Black, nameof(Black))!.MemberId;
        var result = Elo.Calculate(White?.Elo, Black?.Elo, @event.Result);

        if (result != null)
        {
            White = new() { MemberId = whiteId, Color = Color.White, Elo = result.WhiteElo };
            Black = new() { MemberId = blackId, Color = Color.Black, Elo = result.BlackElo };
        }
    }

    private Player? GetOpponent(Guid? memberId)
        => memberId != White?.MemberId ? White : Black;

    private void StartTurn(Player? player, DateTime startTime)
        => Turns?.Add(new() { Player = player, StartTime = startTime });

    //TODO: Unit Test in aggregate
    private void EndTurn(TurnTaken? @event, PieceType? pieceType)
    {
        @event = Guard.Against.Null<TurnTaken?>(@event, nameof(@event));
        pieceType = Guard.Against.Null<PieceType?>(pieceType, nameof(pieceType));

        var turn = Turns?.Last() ?? throw new InvalidOperationException("No turns found!");

        turn.StartPosition = @event?.StartPosition;
        turn.EndPosition = @event?.EndPosition;
        turn.PieceType = pieceType;
    }

    //TODO: Unit Test in aggregate
    //TODO: User should be given a choice to which kind the piece it will be promoted to.
    private void CheckPromotion(TurnTaken? @event, Piece? movingPiece)
    {
        if (SpecialMoves.PawnIsPromoted(movingPiece, @event?.EndPosition) && movingPiece != null)
        {
            var queen = PiecesFactory.CreatePiece(PieceType.Queen, @event?.EndPosition, movingPiece.Id, movingPiece.Color);

            Pieces?.Remove(movingPiece);
            Pieces?.Add(queen);
        }
    }

    //TODO: Unit Test in aggregate
    private void MoveCastingPieces(Piece? king, Square? endPosition)
    {
        if (king == null) return;

        var rank = king.Color == Color.Black ? 8 : 1;
        var file = endPosition?.File > File.E ? File.H : File.A;
        var newFilePosition = file == File.H ? File.F : File.D;
        var rookPosition = new Square(file, rank);
        var rook = Pieces?.FirstOrDefault(p => p.Position == rookPosition);

        if (rook != null)
        {
            rook.Position = new Square(newFilePosition, rank);
        }
    }

    //TODO: Unit Test in aggregate
    private bool IsCheckMate(TakeTurn turn)
    {
        var player = turn.MemberId == Black?.MemberId ? Black : White;
        var piece = Pieces?.FirstOrDefault(p => p.Color != player?.Color && p.Type == PieceType.King);

        if (piece is King king)
        {
            return Board.IsCheckMate(king, Pieces);
        }

        return false;
    }

    //TODO: Unit Test in aggregate
    private bool IsStalemate(TakeTurn command)
    {
        var movingPiece = Pieces?.FirstOrDefault(p => p.Position == command.StartPosition);

        return Board.IsStalemate(movingPiece?.Color, Pieces);
    }
}
