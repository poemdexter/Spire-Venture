using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.util;

namespace SpireVenture.managers
{
    public class ChatManager
    {
        private static ChatManager instance;
        private Queue<ChatMessage> messageQueue;
        public const int NUMBER_OF_CHAT_LINES = 10;
        private const int SECONDS_MSG_LASTS = 10 * 1000; // * 1000 because milliseconds

        public static ChatManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChatManager();
                }
                return instance;
            }
        }

        private ChatManager()
        {
            messageQueue = new Queue<ChatMessage>();
        }

        public void addMessage(string message)
        {
            messageQueue.Enqueue(new ChatMessage(message, SECONDS_MSG_LASTS));
        }

        public List<ChatMessage> getTopMessagesToDisplay()
        {
            List<ChatMessage> messages = messageQueue.ToList();
            if (messages.Count > 0)
            {
                int lines = Math.Min(NUMBER_OF_CHAT_LINES, messages.Count);
                List<ChatMessage> temp = new List<ChatMessage>();
                for (int a = lines; a > 0; a--)
                {
                    temp.Add(messages[messages.Count - a]);
                }
                temp.Reverse();
                return temp;
            }
            return null;
        }

        public void updateQueue(int timeElapsed)
        {
            foreach (ChatMessage msg in messageQueue.ToList())
            {
                msg.TimeCreated -= timeElapsed;
                if (msg.TimeCreated <= 0)
                    messageQueue.Dequeue();
            }
        }
    }
}
