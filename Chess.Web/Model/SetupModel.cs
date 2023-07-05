namespace Chess.Web.Model;

public class SetupModel
{
    public string? MemberOne { get; set; }

    public string? MemberTwo { get; set; }

    public int MaxTurnTime { get; set; } = 10;

    public bool DrawByRepition { get; set; }
}