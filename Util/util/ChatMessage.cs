using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Util.util
{
    public class ChatMessage
    {
        private string Message { get; set; }
        private string Username { get; set; }
        public int TimeCreated { get; set; }

        public ChatMessage(string username, string message)
        {
            Message = message;
            Username = username;
        }

        public ChatMessage(string username, string message, int timecreated)
        {
            Message = message;
            Username = username;
            TimeCreated = timecreated;
        }

        public ChatMessage(string message, int timecreated)
        {
            Message = message;
            TimeCreated = timecreated;
            Username = "";
        }

        public ChatMessage(string message)
        {
            Message = message;
            Username = "";
        }

        // for server
        public string getChatString()
        {
            return "[" + Username + "] " + Message;
        }

        public string getRealNameString()
        {
            return Username;
        }

        public string getChatNameString()
        {
            if (!Username.Equals(""))
                return "[" + Username + "] ";
            else
                return "";
        }

        public string getChatMessageString()
        {
            return Message;
        }
    }
}
