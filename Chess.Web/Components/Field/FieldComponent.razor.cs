using Chess.Web.Pages.Test;
using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Components.Field;

public partial class FieldComponent
{
    private const string DarkFieldCssClass = "dark-field field";
    private const string LightFieldCssClass = "light-field field";

    private string? HtmlClasses { get; set; }
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
        HtmlClasses = DetermineBackgroundColour();

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
}