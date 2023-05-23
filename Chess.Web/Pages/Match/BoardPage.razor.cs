using Chess.Application.Models;
using Chess.Domain.Commands;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using Chess.Web.Components.Field;
using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Pages.Match;

public partial class BoardPage
{
    private Guid TestId = new Guid("00000000-0000-0000-0000-000000000003");

    [Inject]
    public ILogger<BoardPage>? Logger { get; set;}
    [Inject]
    public IApplicationService? ApplicationService { get; set; }
    public IEnumerable<Piece> Pieces { get; private set; } = Enumerable.Empty<Piece>();
    public Guid ActivePieceId { get; set; }
    public Guid ActiveMemberId { get; set; }
    public List<FieldComponent> Fields { get; } = new();

    private void SetFieldHighlight(IEnumerable<FieldComponent> fields, bool highlighted)
    {
        foreach (var item in fields)
        {
            item.Highlight(highlighted);
        }
    }

    public Piece? SelectPiece(int rank, int file)
        => Pieces?.FirstOrDefault(p => p.Position == new Square((File)file, rank));

    public void HideAvailableMoves() => SetFieldHighlight(Fields, false);
    public void ShowAvailableMoves(Guid pieceId)
    {
        SetFieldHighlight(Fields, false);

        var moves = GetAvailableMoves(pieceId);
        var fields = Fields.Where(f => moves.Any(m => m.File == f.File && m.Rank == f.Rank));

        SetFieldHighlight(fields, true);
    }

    public void AddChild(FieldComponent fieldComponent)
    {
        Fields.Add(fieldComponent);
    }

    public async Task UpdateBoardAsync(int rank, File file)
    {
        try
        {
            var activePiece = Pieces.FirstOrDefault(p => p.Id == ActivePieceId) ?? throw new InvalidOperationException("Piece could not be found!");
            var command = new TakeTurn(ActiveMemberId, activePiece.Position, new(file, rank), false);

            if (ApplicationService != null)
                await ApplicationService.TakeTurnAsync(TestId, command);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex,
                             "An error occurred while updating the board for piece {ActivePieceId} for member {ActiveMemberId}",
                             ActivePieceId,
                             ActiveMemberId);
        }
        finally
        {
            ActivePieceId = Guid.Empty;
            ActiveMemberId = Guid.Empty; //TODO: switch memberid's
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var aggregateId = TestId;
        //var command = new StartMatch() { AggregateId = aggregateId, MemberOneId = Guid.NewGuid(), MemberTwoId = Guid.NewGuid() };

        if (ApplicationService != null)
        {
            //await ApplicationService.StartMatchAsync(command);

            Pieces = await ApplicationService.GetPiecesAsync(aggregateId);
            // ActiveMemberId = ApplicationService.GetActiveMemberId();
        }
    }

    //TODO: add unit tests.
    private IEnumerable<Square> GetAvailableMoves(Guid pieceId)
    {
        var piece = Pieces.First(p => p.Id == pieceId);
        var moves = piece.GetAttackRange()
                         .Where(m => !Board.DirectionIsObstructed(Pieces, piece.Position, m))
                         .ToList();

        if (piece is Pawn)
        {
            var unvalidAttackMoves = moves.Where(m => m.File != piece.Position.File)
                                          .Where(m => !Pieces.Any(p => p.Color != piece.Color && p.Position == m));

            if (unvalidAttackMoves.Any())
            {
                return moves.Except(unvalidAttackMoves);
            }
        }

        return moves;
    }
}