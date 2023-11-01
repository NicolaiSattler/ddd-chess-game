using Microsoft.AspNetCore.SignalR;

namespace Chess.Web.Hubs;

public class MatchHub: Hub
{
    public const string HubUrl = "/matchhub";

    public async Task PurposeDraw(string memberId, string aggregateId)
    {
        await Clients.Group(aggregateId).SendAsync("DrawPurposed", memberId);
    }
}