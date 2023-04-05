using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Components.Field;

public partial class FieldComponent
{
    [Parameter, EditorRequired]
    public int Rank { get; set; }

    [Parameter, EditorRequired]
    public File File { get; set; }

    [Parameter, EditorRequired]
    public BackgroundColour Colour { get; set; }

    public string HtmlClasses => Colour == BackgroundColour.Dark
                              ? "dark-field field"
                              : "light-field field";
}

public enum BackgroundColour
{
    Light = 1,
    Dark = 2
}