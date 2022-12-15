using System.Collections.Generic;
using System.Linq;
using Chess.Core.Match.Commands;
using Chess.Core.Match.Entities;
using Chess.Core.Match.Events;
using Chess.Core.Match.Factory;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match;

public class Match : AggregateRoot<Guid>
{
    private Player White { get; set; }
    private Player Black { get; set; }
    private List<Piece> Pieces { get; set; }
    private List<Move> Moves { get; set; }

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

        AssignPlayers(command);

        RaiseEvent(new MatchStarted(White.MemberId, Black.MemberId, DateTime.UtcNow));
    }

    public void TakeTurn(TakeTurn command)
    {
        //TODO: create business rule
        // - Is move valid
        //      - Can piece move to that square?
        //      - Can piece jump over other pieces?
        //      - Castling
        //      - En Passant
        var movingPiece = Pieces.FirstOrDefault(p => p.Position == command.Piece);
        var availableMoves = movingPiece.GetAttackRange();
        var isValidSquare = availableMoves.Any(m => m == command.NewPosition);
        var targetPiece = Pieces.FirstOrDefault(p => p.Position == command.NewPosition);
        var newPositionContainsPiece = targetPiece != null;

        if (isValidSquare && newPositionContainsPiece && targetPiece.Color != movingPiece.Color)
        {
            Pieces.Remove(targetPiece);
            movingPiece.Position = command.NewPosition;
        }

        Moves.Add(new Move()
        {
            Piece = movingPiece,
        });

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
        Moves = new()
        {
            new() { Player = White, StartTime = @event.StartTime }
        };

        var whitePieces = PiecesFactory.CreatePiecesForColor(Color.White);
        var blackPiece = PiecesFactory.CreatePiecesForColor(Color.Black);

        //TODO: Should pieces be saved?
        Pieces = new();
        Pieces.AddRange(whitePieces);
        Pieces.AddRange(blackPiece);
    }

    private void Handle(TurnTaken @event)
    {

    }

    private void AssignPlayers(StartMatch command)
    {
        var colorPicker = new Random(1);

        if (colorPicker.Next() == 0)
        {
            White = new Player { MemberId = command.MemberOneId };
            Black = new Player { MemberId = command.MemberTwoId };

        }
        else
        {
            White = new Player { MemberId = command.MemberTwoId };
            Black = new Player { MemberId = command.MemberOneId };
        }
    }
}
