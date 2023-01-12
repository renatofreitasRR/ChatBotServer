using ChatService.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _connections = connections;
            _botUser = "ChatBot";
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);

                Clients
                   .Group(userConnection.Room)
                   .SendAsync("ReceivedMessage", _botUser, $"{userConnection.UserNickName} saiu do Chat");

                SendConnectedUsers(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups
                .AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            userConnection.SetId(Context.ConnectionId);

            await Clients
                .Group(userConnection.Room)
                .SendAsync("ReceivedMessage", new UserConnection(_botUser), $"{userConnection.UserNickName} entrou no grupo");

            await SendConnectedUsers(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                userConnection.SetId(Context.ConnectionId);

                await Clients
                    .Group(userConnection.Room)
                    .SendAsync("ReceivedMessage", userConnection, message);
            }
        }

        public Task SendConnectedUsers(string room)
        {
            var users = _connections
                .Values
                .Where(x => x.Room == room)
                .Select(x => x.UserNickName);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
