namespace Chess.Web.Model;

public class SetupModel
{
    public string? MemberOne { get; set; }
    //For testing purposes set to 900
    public int MemberOneElo { get; set; } = 900;
    public string? MemberTwo { get; set; }
    //For testing purposes set to 900
    public int MemberTwoElo { get; set; } = 900;
    //For testing purposes set to 10 minutes
    public int MaxTurnTimeInMinutes { get; set; } = 10;
    public bool DrawByRepition { get; set; }
}