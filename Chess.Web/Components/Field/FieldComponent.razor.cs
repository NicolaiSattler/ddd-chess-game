using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Components.Field;

public partial class FieldComponent
{
    private BackgroundColour _colour;

    private string HtmlClasses => _colour == BackgroundColour.Dark
                               ? "dark-field field"
                               : "light-field field";
    private bool ShowRank => this.File == File.A;
    private bool ShowFile => this.Rank == 8;

    [Parameter, EditorRequired]
    public int Rank { get; set; }

    [Parameter, EditorRequired]
    public File File { get; set; }

    protected override void OnInitialized()
    {
        _colour = SelectColour();

        base.OnInitialized();
    }

    private BackgroundColour SelectColour()
    {
        var file = (int)this.File;

        return (Rank % 2) > 0
                ? (file % 2) > 0
                     ? BackgroundColour.Light : BackgroundColour.Dark
                : (file % 2) == 1
                     ? BackgroundColour.Dark : BackgroundColour.Light;
    }
}

public enum BackgroundColour
{
    Light = 1,
    Dark = 2
}