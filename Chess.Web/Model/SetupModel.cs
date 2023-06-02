namespace Chess.Web.Model;

public class SetupModel
{
    public string? MemberOne { get; set; }

    public string? MemberTwo { get; set; }

    public TimeOnly MaxTurnTime { get; set; } = new TimeOnly(0, 10);

    public bool DrawByRepition { get; set; }
}