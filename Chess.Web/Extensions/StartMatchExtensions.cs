using Chess.Domain.Commands;
using Chess.Web.Model;

namespace Chess.Web.Extensions;

public static class Extensions
{
    public static StartMatch CreateCommand(this SetupModel model)
    {
        if (!Guid.TryParse(model.MemberOne, out var memberOne)
            || !Guid.TryParse(model.MemberTwo, out var memberTwo))
        {
            throw new ArgumentException("Invalid setup", nameof(model));
        }

        var aggregateId = Guid.NewGuid();
        return new()
        {
            MemberOne = new() { MemberId = memberOne, Elo = model.MemberOneElo },
            MemberTwo = new() { MemberId = memberTwo, Elo = model.MemberTwoElo },
            AggregateId = aggregateId,
            Options = new()
            {
                DrawByRepition = model.DrawByRepition,
                MaxTurnTime = new TimeSpan(0, model.MaxTurnTimeInMinutes, 0)
            }
        };
    }
}