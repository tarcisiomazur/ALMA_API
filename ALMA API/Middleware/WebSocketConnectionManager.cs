using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ALMA_API.Middleware;

public class WebSocketConnectionManager
{
    private ConcurrentDictionary<string, Tuple<WebSocket, int>> _unAuthSockets = new ();
    private ConcurrentDictionary<int, Dictionary<string,WebSocket>> _authSockets = new ();

    public string AddSocket(WebSocket socket)
    {
        string connId;
        var unAuth = new Tuple<WebSocket,int>(socket, 0);
        do
        {
            connId = Guid.NewGuid().ToString();
        } while (!_unAuthSockets.TryAdd(connId, unAuth));

        return connId;
    }

    public bool RemoveSocket(string connId)
    {
        if (_unAuthSockets.TryRemove(connId, out var tuple) && _authSockets.TryGetValue(tuple.Item2, out var auth))
        {
            return auth.Remove(connId) && (auth.Count != 0 || _authSockets.TryRemove(tuple.Item2, out _));
        }

        return true;
    }

    public void Vinculate(string connId, int userId)
    {
        if (_unAuthSockets.TryGetValue(connId, out var tuple))
        {
            var socket = tuple.Item1;
            _unAuthSockets[connId] = new Tuple<WebSocket, int>(tuple.Item1, userId);
            Dictionary<string, WebSocket> auths;
            if (!_authSockets.TryGetValue(userId, out auths))
            {
                _authSockets.AddOrUpdate(userId,
                    _ => new Dictionary<string, WebSocket>()
                    {
                        {connId, socket}
                    },
                    (_, sockets) =>
                    {
                        sockets.Add(connId, socket);
                        return sockets;
                    }
                );
            }
        }
    }

    public void SendToAllClients(int userId, string message)
    {
        if(_authSockets.TryGetValue(userId, out var sockets))
        {
            foreach (var (_, socket) in sockets)
            {
                socket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}