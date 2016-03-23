using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace WebClient
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addMessage(name, message, DateTime.Now.ToLongTimeString());
        }
    }

    public class PlayerHub : Hub
    {
        public void Send(string name)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addPlayer(name);
        }
    }
    public class GameHub : Hub
    {
    }

    public class PlayHub : Hub
    {
        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        public void ChangeGameState(string gameName, string state)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.Group(gameName).changeGameState(state);
            //Clients.All.changeGameState(state);
        }
    }
}