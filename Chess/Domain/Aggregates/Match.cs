using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Core.Match.Events;
using Chess.Core.Match.Factories;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Factories;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Aggregates;

public class Match : AggregateRoot<Guid>
{
    private Player? White { get; set; }
    private Player? Black { get; set; }
    private List<Piece>? Pieces { get; set; }
    private List<Turn>? Turns { get; set; }

    public Match(Guid id) : base(id) { }
    public Match(Guid id, IEnumerable<DomainEvent?>? events) : base(id, events) { }

    protected override void When(DomainEvent? domainEvent)
    {
        if (domainEvent is MatchStarted gameStarted) Handle(gameStarted);
        if (domainEvent is TurnTaken turnTaken) Handle(turnTaken);
    }

    public void Start(StartMatch command)
    {
        if (command.MemberOneId == command.MemberTwoId) throw new InvalidOperationException("Member Ids are the same.");

        var colorPicker = new Random(1);
        var memberOneIsWhite = colorPicker.Next() == 0;
        var whiteId = memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var blackId = !memberOneIsWhite ? command.MemberOneId : command.MemberTwoId;
        var @event = new MatchStarted() { WhiteMemberId = whiteId, BlackMemberId = blackId, StartTime = DateTime.UtcNow };

        RaiseEvent(@event);
    }

    //TODO: Unit Test.
    public void TakeTurn(TakeTurn command)
    {
        var violations = RuleFactory.GetTurnRules(command, Pieces, Turns)
                                    .SelectMany(r => r.CheckRule());

        if (!violations.Any())
        {
            var isCheckmate = KingIsInCheck(command);
            var isStalemate = IsStalemate(command);

            //TODO: calculate ELO
            if (isCheckmate)
            {
                RaiseEvent(new MatchEnded());
            }
            else if (isStalemate)
            {
                RaiseEvent(new MatchEnded());
            }
            else
            {
                RaiseEvent(new TurnTaken(command.MemberId, command?.StartPosition, command?.EndPosition));
            }
        }
    }

    public void Resign(Guid resigningPlayerId)
    {
        throw new NotImplementedException();
    }

    public void ProposeDraw(Guid proposingPlayerId)
    {
        throw new NotImplementedException();
    }

    private void Handle(MatchStarted @event)
    {
        White = new() { Color = Color.White, MemberId = @event.WhiteMemberId };
        Black = new() { Color = Color.Black, MemberId = @event.BlackMemberId };
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
        var targetPiece = Pieces?.FirstOrDefault(p => p.Position == @event.EndPosition);

        if (movingPiece == null) return;

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

    private Player? GetOpponent(Guid? memberId)
        => memberId != White?.MemberId ? White : Black;

    private void StartTurn(Player? player, DateTime startTime)
        => Turns?.Add(new() { Player = player, StartTime = startTime });

    //TODO: Unit Test
    private void EndTurn(TurnTaken? @event, PieceType? pieceType)
    {
        @event = Guard.Against.Null<TurnTaken?>(@event, nameof(@event));
        pieceType = Guard.Against.Null<PieceType?>(pieceType, nameof(pieceType));

        var turn = Turns?.Last() ?? throw new InvalidOperationException("No turns found!");

        turn.StartPosition = @event?.StartPosition;
        turn.EndPosition = @event?.EndPosition;
        turn.PieceType = pieceType;
    }

    //TODO: Unit Test
    private void CheckPromotion(TurnTaken? @event, Piece? movingPiece)
    {
        if (SpecialMoves.PawnIsPromoted(movingPiece, @event?.EndPosition) && movingPiece != null)
        {
            var queen = PiecesFactory.CreatePiece(PieceType.Queen, @event?.EndPosition, movingPiece.Id, movingPiece.Color);

            Pieces?.Remove(movingPiece);
            Pieces?.Add(queen);
        }
    }

    //TODO: Unit Test
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

    //TODO: Unit Test
    private bool KingIsInCheck(TakeTurn turn)
    {
        var player = turn.MemberId == Black?.MemberId ? Black : White;
        var piece = Pieces?.FirstOrDefault(p => p.Color == player?.Color && p.Type == PieceType.King);

        if (piece is King king)
        {
            return Board.IsCheckMate(king, Pieces);
        }

        return false;
    }

    //TODO: Unit Test
    private bool IsStalemate(TakeTurn command)
    {
        var movingPiece = Pieces?.FirstOrDefault(p => p.Position == command.StartPosition);

        return Board.IsStalemate(movingPiece?.Color, Pieces);
    }
}
