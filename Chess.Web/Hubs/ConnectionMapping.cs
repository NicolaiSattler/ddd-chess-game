namespace Chess.Web.Hubs;

public class ConnectionMapping<T>
{
    private readonly Dictionary<T, HashSet<string>> _connections;

    public ConnectionMapping()
    {
        _connections = new();
    }

    public int Count => _connections.Count;

    public void Add(T key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out var connections))
            {
                connections = new HashSet<string>();
                _connections.Add(key, connections);
            }

            lock (connections)
            {
                connections.Add(connectionId);
            }
        }
    }
    public IEnumerable<string> GetConnections(T key)
    {
        return _connections.TryGetValue(key, out var connections)
               ? connections
               : Enumerable.Empty<string>();
    }

    public void Remove(T key, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.TryGetValue(key, out var connections))
            {
                return;
            }

            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}