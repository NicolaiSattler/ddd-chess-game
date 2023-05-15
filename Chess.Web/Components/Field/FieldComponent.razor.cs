using Chess.Web.Pages.Test;
using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;
using Board = Chess.Web.Pages.Match.BoardPage;

namespace Chess.Web.Components.Field;

public partial class FieldComponent
{
    private const string DarkFieldCssClass = "dark-field field";
    private const string LightFieldCssClass = "light-field field";

    private string? BgClasses { get; set; }
    private string? DropClasses { get; set; }
    private bool ShowRank => this.File == File.A;
    private bool ShowFile => this.Rank == 8;

    [CascadingParameter]
    private Board? Parent { get; set; }

    [Parameter, EditorRequired]
    public int Rank { get; set; }

    [Parameter, EditorRequired]
    public File File { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set;}

    protected override void OnInitialized()
    {
        BgClasses = DetermineBackgroundColour();

        base.OnInitialized();
    }

    private string DetermineBackgroundColour()
    {
        var file = (int)this.File;

        return (Rank % 2) > 0
               ? (file % 2) > 0
                    ? LightFieldCssClass : DarkFieldCssClass
               : (file % 2) == 1
                    ? DarkFieldCssClass : LightFieldCssClass;
    }

    private void HandleDragEnter()
    {
        //TODO: Highlight Field
    }

    private void HandleDragLeave()
    {
        //TODO: Reset css
    }

    private async Task HandleDropAsync()
    {
        if (Parent != null)
        {
            await Parent.UpdateBoardAsync(Rank, File);
            Parent.ActivePieceId = Guid.Empty;
        }
    }
}