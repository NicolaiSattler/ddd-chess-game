namespace Chess.Web.Model;

public class SetupModel
{
    public string? MemberOne { get; set; }

    public string? MemberTwo { get; set; }

    public TimeOnly MaxTurnTime { get; set; }

    public bool DrawByRepition { get; set; }
}