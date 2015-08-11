// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAH.Model;
using Microsoft.AspNet.SignalR;

namespace CardsAgainstHumanity.Hubs
{
    public class LobbyHub : Hub
    {
        public static readonly List<Lobby> Lobbies = new List<Lobby>();

        public void CreateNewLobby(string userName)
        {
            if (Lobbies.Any(x => x.Users.Any(y => y.ConnectionId == Context.ConnectionId)))
            {
                RemoveUserFromLobby(Context.ConnectionId);
            }

            string lobbyCode;
            do
            {
                var random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                lobbyCode = new string(
                Enumerable.Repeat(chars, 6)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            } while (Lobbies.Any(x => x.Code == lobbyCode));

            var lobby = new Lobby();
            lobby.Code = lobbyCode;

            lobby.Users.Add(new User { ConnectionId = Context.ConnectionId, Name = userName });

            Lobbies.Add(lobby);

            Clients.Caller.lobbyGotoLobby(lobbyCode);
            Clients.Caller.lobbyUpdateLobbyUserList(lobby.Users.Select(x => x.Name));
        }

        public void JoinLobby(string name, string lobbyCode)
        {
            var code = lobbyCode.ToUpper();

            if (Lobbies.All(x => x.Code != code))
            {
                Clients.Caller.lobbyErrorJoiningLobby("Can't find lobby with specified code.");
                return;
            }

            var lobby = Lobbies.Single(x => x.Code == code);

            if (lobby.ActiveGame)
            {
                Clients.Caller.lobbyErrorJoiningLobby("Can't join a game in progress");
                return;
            }

            lobby.Users.Add(new User { ConnectionId = Context.ConnectionId, Name = name });

            Clients.Caller.lobbyGotoLobby(code);

            foreach (var user in lobby.Users)
            {
                Clients.Client(user.ConnectionId).lobbyUpdateLobbyUserList(lobby.Users.Select(x => x.Name));
            }
            
        }

        public void LeaveLobby()
        {
            RemoveUserFromLobby(Context.ConnectionId);
        }

        private void RemoveUserFromLobby(string connectionId)
        {
            var lobby = Lobbies.SingleOrDefault(x => x.Users.Any(y => y.ConnectionId == connectionId));

            if (lobby == null)
                return;

            lobby.Users.Remove(lobby.Users.Single(x => x.ConnectionId == connectionId));
            if (lobby.Users.Count == 0)
            {
                Lobbies.Remove(lobby);
            }
            else
            {
                foreach (var user in lobby.Users)
                {
                    Clients.Client(user.ConnectionId).gameRecievedScores(lobby.Users.Select(x => new { x.Name, x.NumberOfPoints }));
                    Clients.Client(user.ConnectionId).lobbyUpdateLobbyUserList(lobby.Users.Select(x => x.Name));
                }
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            RemoveUserFromLobby(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}