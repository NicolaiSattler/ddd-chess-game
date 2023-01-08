using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chess.Core;
using Chess.Core.Match.Events;
using Chess.Core.Match.Factory;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

namespace Chess.Domain;

public class Match : AggregateRoot<Guid>
{
    private Player? White { get; set; }
    private Player? Black { get; set; }
    private List<Piece>? Pieces { get; set; }
    private List<Move>? Moves { get; set; }

    public Match(Guid id) : base(id) { }
    public Match(Guid id, IEnumerable<DomainEvent> events) : base(id, events) { }

    protected override void When(DomainEvent domainEvent)
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

        RaiseEvent(new MatchStarted(whiteId, blackId, DateTime.UtcNow));
    }

    //TODO:
    // - return a response object to the end user.
    // - create business rule
    public void TakeTurn(TakeTurn command)
    {
        try
        {
            var violations = RuleFactory.GetTurnRules(command, Pieces)
                                        .SelectMany(r => r.CheckRule());

            if (!violations.Any())
            {
                RaiseEvent(new TurnTaken(command.MemberId, command?.StartPosition, command?.EndPosition));
            }
            else
            {
                //....
            }
        }
        catch (Exception ex)
        {
            //TODO: Replace with ILogger
            Debug.Print(ex.ToString());
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

    public void End(MatchEnded command)
    {
        throw new NotImplementedException();
    }

    private void Handle(MatchStarted @event)
    {
        White = new() { Color = Color.White, MemberId = @event.WhiteMemberId };
        Black = new() { Color = Color.Black, MemberId = @event.BlackMemberId };
        Moves = new() { new() { Player = White, StartTime = @event.StartTime } };

        Pieces = new();
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.White));
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.Black));
    }

    //      - En Passant
    //      - Castling
    //      - Captured
    //      - Promotion
    private void Handle(TurnTaken @event)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        var movingPiece = Pieces?.FirstOrDefault(p => p.Position == @event.StartPosition);
        var targetPiece = Pieces?.FirstOrDefault(p => p.Position == @event.EndPosition);

        if (movingPiece == null) return;

        if (Board.PieceIsCaptured(@event, Pieces) && targetPiece != null)
        {
            Pieces?.Remove(targetPiece);

            movingPiece.Position = @event?.EndPosition;
        }

        if (Board.PawnIsPromoted(@event, Pieces))
        {
            var queen = PiecesFactory.CreatePiece(PieceType.Queen, @event?.EndPosition, movingPiece.Id, movingPiece.Color);

            Pieces?.Remove(movingPiece);
            Pieces?.Add(queen);
        }

        NewMoveMade(movingPiece, @event);
    }

    private void NewMoveMade(Piece? movingPiece, TurnTaken? @event)
    {
        Moves?.Add(new()
        {
            Piece = movingPiece,
            Position = @event?.EndPosition,
            Player = White?.MemberId == @event?.MemberId ? White : Black
        });
    }
}