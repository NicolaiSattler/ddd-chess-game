using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ardalis.GuardClauses;
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

        RaiseEvent(new MatchStarted(whiteId, blackId, DateTime.UtcNow));
    }

    //TODO:
    // - return a response object to the end user.
    // - create business rule
    public void TakeTurn(TakeTurn command)
    {
        try
        {
            var violations = RuleFactory.GetTurnRules(command, Pieces, Turns)
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
        Turns = new();

        Pieces = new();
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.White));
        Pieces.AddRange(PiecesFactory.CreatePiecesForColor(Color.Black));

        StartTurn(White, @event.StartTime);
    }

    //      - En Passant (done)
    //      - Castling
    //      - Captured (done)
    //      - Promotion (done)
    //      - Check
    //      - Check Mate
    private void Handle(TurnTaken @event)
    {
        Guard.Against.Null<TurnTaken>(@event, nameof(@event));

        var movingPiece = Pieces?.FirstOrDefault(p => p.Position == @event.StartPosition);
        var targetPiece = Pieces?.FirstOrDefault(p => p.Position == @event.EndPosition);

        if (movingPiece == null) return;

        var isEnPassant = SpecialMoves.IsEnPassant(movingPiece, Turns);

        if (isEnPassant)
        {
            var pieceId = Turns?.Last().Id;
            targetPiece = Pieces?.FirstOrDefault(p => p.Id == pieceId);
        }

        if ((Board.PieceIsCaptured(@event, Pieces) || isEnPassant) && targetPiece != null)
        {
            Pieces?.Remove(targetPiece);

            movingPiece.Position = @event?.EndPosition;
        }

        CheckPromotion(@event, movingPiece);

        EndTurn(@event, movingPiece?.Type);
        StartTurn(GetOpponent(@event?.MemberId), DateTime.UtcNow);
    }

    private Player? GetOpponent(Guid? memberId) => memberId != White?.MemberId ? White : Black;
    private void StartTurn(Player? player, DateTime startTime) => Turns?.Add(new() { Player = player, StartTime = startTime });
    private void EndTurn(TurnTaken? @event, PieceType? pieceType)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));
        _ = pieceType ?? throw new ArgumentNullException(nameof(pieceType));

        var turn = Turns?.Last() ?? throw new InvalidOperationException("No turns found!");

        turn.StartPosition = @event.StartPosition;
        turn.EndPosition = @event.EndPosition;
        turn.PieceType = pieceType;
    }
    private void CheckPromotion(TurnTaken? @event, Piece? movingPiece)
    {
        if (Board.PawnIsPromoted(@event, Pieces) && movingPiece != null)
        {
            var queen = PiecesFactory.CreatePiece(PieceType.Queen, @event?.EndPosition, movingPiece.Id, movingPiece.Color);

            Pieces?.Remove(movingPiece);
            Pieces?.Add(queen);
        }
    }
}
