using System.Net.WebSockets;
using System.Text;

namespace ALMA_API.Middleware;

public class WebSocketServerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketConnectionManager _manager;
    
    public WebSocketServerMiddleware(RequestDelegate next, WebSocketConnectionManager manager)
    {
        _next = next;
        _manager = manager;
    }

    public async Task SendConnId(WebSocket socket, string connId)
    {
        Console.WriteLine($"Websocket {connId} connected");
        var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task Disconnect(WebSocket socket, string connId, WebSocketReceiveResult? result = null)
    {
        Console.WriteLine($"Websocket disconnected {connId}");
        _manager.RemoveSocket(connId);
        if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
        {
            await socket.CloseAsync(
                result?.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                result?.CloseStatusDescription??"Timeout",
                CancellationToken.None);
        }
    }
    
    public async Task Invoke(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            if (context.Items["id"] is not int userId)
            {
                await HttpResponseWritingExtensions.WriteAsync(context.Response, "{\"message\": \"Unauthorized\"}");
                return;
            }
            var connId = _manager.AddSocket(socket, userId);
            await SendConnId(socket, connId);
            await RunAsync(socket, connId);
        }
        else
        {
            var request = context.Request;

            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestContent = Encoding.UTF8.GetString(buffer);
            Console.WriteLine($"{request.Path}{request.QueryString}: {requestContent}");
            request.Body.Position = 0;
            await _next(context);
        }
    }

    private async Task RunAsync(WebSocket socket, string connId)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            try
            {
                var task = socket.ReceiveAsync(buffer, CancellationToken.None);
                if (task.Wait(TimeSpan.FromMinutes(30)))
                {
                    var result = task.Result;
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            Console.WriteLine($"Message received from {connId}: {receivedMessage}");
                            break;
                        case WebSocketMessageType.Close:
                            await Disconnect(socket, connId, result);
                            return;
                        default:
                            continue;
                    }
                }
                else
                {
                    await Disconnect(socket, connId);
                }
            }
            catch (Exception ex)
            {
                await Disconnect(socket, connId);
            }
        }
    }

}