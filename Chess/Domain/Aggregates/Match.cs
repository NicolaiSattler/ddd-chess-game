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
        var violations = RuleFactory.GetTurnRules(command, Pieces, Turns)
                                    .SelectMany(r => r.CheckRule())
                                    .ToList();


        if (!violations.Any())
        {
            RaiseEvent(new TurnTaken
            {
                MemberId = command.MemberId,
                StartPosition = command.StartPosition,
                EndPosition =  command.EndPosition,
                PromotionType = command.PromotionType,
                EndTime = DateTime.UtcNow
            });
        }

        return new() { Violations = violations };
    }

    public void Forfeit(Forfeit command)
    {
        Guard.Against.Null(command, nameof(command));

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
        Guard.Against.Null(@event, nameof(@event));
        Guard.Against.Null(Turns, nameof(Turns));

        var movingPiece = Pieces.Find(p => p.Position == @event.StartPosition);

        if (movingPiece == null) return;

        var targetPiece = Pieces.Find(p => p.Position == @event.EndPosition);
        var isEnPassant = SpecialMoves.IsEnPassant(movingPiece, Turns);
        var pieceIsCaptured = Board.PieceIsCaptured(@event, Pieces) || isEnPassant;
        var castling = SpecialMoves.IsCastling(@event.StartPosition, @event.EndPosition, Pieces);

        if (isEnPassant)
        {
            var pieceId = Turns.Last().Id;
            targetPiece = Pieces.Find(p => p.Id == pieceId);
        }

        if (castling != CastlingType.Undefined) MoveCastingPieces(movingPiece, @event.EndPosition);

        if (targetPiece != null && pieceIsCaptured) Pieces.Remove(targetPiece);

        movingPiece.Position = @event.EndPosition;

        if (@event.PromotionType != PieceType.Undefined) PromotePiece(@event.PromotionType, movingPiece);

        var isCheckMate = IsCheckMate(@event);
        var isStalemate = Board.IsStalemate(movingPiece.Color, Pieces);
        var isCheck = OpponentIsInCheck(movingPiece.Color);
        var notation = DetermineNotation(movingPiece, targetPiece, castling, @event.PromotionType, isCheck, isCheckMate);

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

    private static string DetermineNotation(Piece movingPiece,
                                            Piece? targetPiece,
                                            CastlingType castling,
                                            PieceType promotionType,
                                            bool isCheck,
                                            bool isCheckMate)
    {
        var notation = new NotationBuilder();

        notation.HasPiece(movingPiece.Type);

        if (castling != CastlingType.Undefined)
            notation.IsCastling(castling);
        else if (targetPiece != null)
            notation.HasCapturedPiece(movingPiece);

        notation.EndsAtPosition(movingPiece);

        if (promotionType != PieceType.Undefined)
            notation.IsPromotion(promotionType);

        if (isCheck)
            notation.IsCheck();

        if (isCheckMate)
            notation.IsCheckMate();

        return notation.Build();
    }

    private bool OpponentIsInCheck(Color currentPlayerColor)
    {
        var piece =  Pieces.FirstOrDefault(p => p.Color != currentPlayerColor && p.Type == PieceType.King);
        return piece is King king && Board.IsCheck(king, Pieces);
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
    }

    private Player GetOpponent(Guid memberId) => memberId != White.MemberId ? White : Black;

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
        @event = Guard.Against.Null(@event, nameof(@event));
        pieceType = Guard.Against.Null(pieceType, nameof(pieceType));

        var turn = Turns.Last() ?? throw new InvalidOperationException("No turns found!");
        var player = White.MemberId == @event.MemberId ? White : Black;

        turn.StartPosition = @event.StartPosition;
        turn.EndPosition = @event.EndPosition;
        turn.PieceType = pieceType;
        turn.Hash = CalculateHash(player.Color);
        turn.Notation = notation;
    }

    //TODO: Unit Test in aggregate
    //TODO: User should be given a choice to which kind the piece it will be promoted to.
    private void PromotePiece(PieceType promotionType, Piece movingPiece)
    {
        var newPieceType = PiecesFactory.CreatePiece(promotionType, movingPiece.Position, movingPiece.Id, movingPiece.Color);

        Pieces.Remove(movingPiece);
        Pieces.Add(newPieceType);
    }

    //TODO: Unit Test in aggregate
    private void MoveCastingPieces(Piece king, Square endPosition)
    {
        if (king == null) return;

        var rank = king.Position.Rank;
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
    private bool IsCheckMate(TurnTaken @event)
    {
        var player = @event.MemberId == Black.MemberId ? Black : White;
        var piece = Pieces.Find(p => p.Color != player.Color && p.Type == PieceType.King);

        if (piece is King king)
        {
            return Board.IsCheckMate(king, Pieces);
        }

        return false;
    }

    private  string CalculateHash(Color color)
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

    private MatchResult GetMatchResult(Guid? memberId) => memberId == White.MemberId ? MatchResult.WhiteWins : MatchResult.BlackWins;
}
