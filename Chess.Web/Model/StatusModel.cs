namespace Chess.Web.Model;

public record StatusModel(string Content, StatusType Type)
{
    public static StatusModel Empty() => new(string.Empty, StatusType.Undefined);
}

public enum StatusType
{
    Undefined = 0,
    Information = 1,
    Warning = 2,
    Error = 3
}