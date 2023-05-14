using Chess.Application.Models;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Pages.Test;

public partial class Board
{
    private Guid TestId = new Guid("00000000-0000-0000-0000-000000000001");

    [Inject]
    public IApplicationService? ApplicationService { get; set; }
    public IEnumerable<Piece> Pieces { get; private set; } = Enumerable.Empty<Piece>();

    public Piece? SelectPiece(int rank, int file)
        => Pieces?.FirstOrDefault(p => p.Position == new Square((File)file, rank));

    public IEnumerable<Square> ShowAvailableMoves(Guid pieceId)
    {
        throw new NotImplementedException();
    }

    public void TakeTurn(Guid pieceId, Square endPosition)
    {
        throw new NotImplementedException();
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