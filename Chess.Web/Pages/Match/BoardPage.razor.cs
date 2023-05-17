using Chess.Application.Models;
using Chess.Domain.Commands;
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
    public IApplicationService? ApplicationService { get; set; }
    public IEnumerable<Piece> Pieces { get; private set; } = Enumerable.Empty<Piece>();
    public Guid ActivePieceId { get; set; }
    public List<FieldComponent> Fields { get; } = new();

    private void SetFieldHighlight(IEnumerable<FieldComponent> fields, bool highlighted)
    {
        foreach (var item in fields)
            item.Highlight(highlighted);
    }

    public Piece? SelectPiece(int rank, int file)
        => Pieces?.FirstOrDefault(p => p.Position == new Square((File)file, rank));

    public void HideAvailableMoves() => SetFieldHighlight(Fields, false);
    public void ShowAvailableMoves(Guid pieceId)
    {
        SetFieldHighlight(Fields, false);

        var piece = Pieces.First(p => p.Id == pieceId);
        var moves = piece.GetAttackRange();
        var fields = Fields.Where(f => moves.Any(m => m.File == f.File && m.Rank == f.Rank));

        SetFieldHighlight(fields, true);
    }

    public void AddChild(FieldComponent fieldComponent)
    {
        Fields.Add(fieldComponent);
    }

    public async Task UpdateBoardAsync(int rank, File file)
    {


        //reset after turn is finished
        ActivePieceId = Guid.Empty;
        await Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        var aggregateId = TestId;
        //var command = new StartMatch() { AggregateId = aggregateId, MemberOneId = Guid.NewGuid(), MemberTwoId = Guid.NewGuid() };

        if (ApplicationService != null)
        {
            //await ApplicationService.StartMatchAsync(command);

            Pieces = await ApplicationService.GetPiecesAsync(aggregateId);
        }
    }

}