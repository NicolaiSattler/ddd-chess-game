using Chess.Web.Components.Board;
using Chess.Web.Components.Piece;
using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Components.Field;

public partial class FieldComponent
{
    private const string DarkFieldCssClass = "dark-field field";
    private const string LightFieldCssClass = "light-field field";
    private const string HighlightCssClass = "highlight";

    private string? BgClasses { get; set; }
    private string? DropClasses { get; set; }
    private bool ShowRank => this.File == File.A;
    private bool ShowFile => this.Rank == 8;

    [CascadingParameter]
    private BoardComponent? Parent { get; set; }

    [Parameter, EditorRequired]
    public int Rank { get; set; }

    [Parameter, EditorRequired]
    public File File { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set;}

    public void AddChild(Domain.Entities.Pieces.Piece piece)
    {
        ChildContent = builder =>
        {
            builder.OpenElement(0, nameof(PieceComponent));
            builder.AddAttribute(0, nameof(PieceComponent.PieceId), piece.Id);
            builder.AddAttribute(1, nameof(PieceComponent.Color), piece.Color);
            builder.AddAttribute(2, nameof(PieceComponent.Type), piece.Type);
            builder.CloseElement();
        };
    }

    public void RemoveChild()
    {
        ChildContent = builder => { };

        StateHasChanged();
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

    private async Task HandleDropAsync()
    {
        if (Parent != null)
        {
            await Parent.UpdateBoardAsync(Rank, File);

            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        BgClasses = DetermineBackgroundColour();

        Parent?.AddChild(this);

        base.OnInitialized();
    }

    public void Highlight(bool enabled)
    {
        if (enabled && string.IsNullOrEmpty(DropClasses))
        {
            DropClasses = HighlightCssClass;
            StateHasChanged();
        }
        else if (!enabled && !string.IsNullOrEmpty(DropClasses))
        {
            DropClasses = string.Empty;
            StateHasChanged();
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Square square)
        {
            return square.File == File && square.Rank == Rank;
        }

        return base.Equals(obj);
    }

}