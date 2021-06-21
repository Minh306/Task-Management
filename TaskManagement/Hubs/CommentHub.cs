using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagement.Hubs
{
    public class CommentHub: Hub
    {
        public void Send(string userId, int taskId, string nextCommnetId, string fullName, string message)
        {
             Clients.All.SendComment(userId, taskId, nextCommnetId, fullName, message);
        }
        public void RemoveComment(int taskId, int commentId)
        {
            Clients.All.RemoveComment(taskId, commentId);
        }
        public void SendNotifier(string changeBy, string assignFrom, string assignTo, string changeByFullName, string assignFromFullName, string assignToFullName, int taskId, string taskName)
        {
            Clients.All.SendNotifier(changeBy, assignFrom, assignTo, changeByFullName, assignFromFullName, assignToFullName, taskId, taskName);
        }
    }
}