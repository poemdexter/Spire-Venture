﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.util;
using Microsoft.Xna.Framework.Graphics;

namespace SpireVenture.managers
{
    public class ChatManager
    {
        private static ChatManager instance;
        private Queue<ChatMessage> messageQueue;

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

        public void addMessage(string username, string message)
        {
            SpriteFont font = GameConstants.FONT;
            int maxSize = ClientOptions.Instance.ResolutionWidth - 20;

            //if text length > screen width
            if (font.MeasureString(username + message).X * GameConstants.CHAT_SCALE > maxSize)
            {
                // make list of word strings
                List<string> words = message.Split(' ').ToList();
                StringBuilder str1 = new StringBuilder("");
                StringBuilder str2 = new StringBuilder("");
                bool switchCheck = false;

                foreach (string word in words)
                {
                    if (!switchCheck)
                    {
                        // should we go to next line?
                        if (font.MeasureString(username + str1.ToString() + word).X * GameConstants.CHAT_SCALE >= maxSize)
                        {
                            switchCheck = true;
                            str2.Append(word + " ");
                        }
                        else str1.Append(word + " ");
                    }
                    else str2.Append(word + " ");
                }

                Console.WriteLine(str1);
                Console.WriteLine(str2);
                messageQueue.Enqueue(new ChatMessage(username, str1.ToString(), GameConstants.SECONDS_MSG_LASTS));
                messageQueue.Enqueue(new ChatMessage("<<", str2.ToString(), GameConstants.SECONDS_MSG_LASTS));
            }
            else
                messageQueue.Enqueue(new ChatMessage(username, message, GameConstants.SECONDS_MSG_LASTS));
        }

        public List<ChatMessage> getTopMessagesToDisplay()
        {
            List<ChatMessage> messages = messageQueue.ToList();
            if (messages.Count > 0)
            {
                int lines = Math.Min(GameConstants.NUMBER_OF_CHAT_LINES, messages.Count);
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

        // does the fancy move chat while typing so you can see
        public string getTruncatedChatInput(string input)
        {
            SpriteFont font = GameConstants.FONT;
            int maxSize = ClientOptions.Instance.ResolutionWidth - 20;

            int x = 1;
            while(font.MeasureString(input + "_").X * GameConstants.CHAT_SCALE > maxSize)
            {
                input = input.Substring(x, input.Length - x);
                x++;
            }
            return input + "_";
        }
    }
}
