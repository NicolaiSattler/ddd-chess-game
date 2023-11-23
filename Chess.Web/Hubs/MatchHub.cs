using Microsoft.AspNetCore.SignalR;

namespace Chess.Web.Hubs;

public class MatchHub: Hub
{
    public const string HubUrl = "/matchhub";

    private readonly ConnectionMapping<string> _connections;

    public MatchHub()
    {
        _connections = new();
    }

    public async Task PurposeDraw(string opponentMemberId)
    {
        foreach(var connectionId in _connections.GetConnections(opponentMemberId))
        {
            await Clients.User(connectionId).SendAsync("DrawPurposed");
        }
    }

    public override Task OnConnectedAsync()
    {
        var memberId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(memberId))
            _connections.Add(memberId, Context.ConnectionId);

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var memberId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(memberId))
            _connections.Remove(memberId, Context.ConnectionId);

        return Task.CompletedTask;
    }
}