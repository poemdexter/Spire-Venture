using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Entities.framework;
using Entities.components;
using Util.util;
using Entities.actions;
using Entities.action_args;

namespace SpireVenture.managers
{
    public class ClientGameManager
    {
        private static ClientGameManager instance;
        public Dictionary<string, Entity> PlayerEntities;  // holds player entities
        public string Username { get; set; }

        private Int16 Key = 1;
        public Int16 SequenceKey
        {
            get
            {
                if (Key++ > 500)
                    Key = 1;
                return Key;
            }
        }

        // i'm a singleton!
        public static ClientGameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientGameManager();
                }
                return instance;
            }
        }

        private ClientGameManager()
        {
            PlayerEntities = new Dictionary<string, Entity>();
        }

        public void setUsername(string name)
        {
            Username = name;
        }

        public void PredictPlayerFromInput(Inputs input)
        {
            CheckForEntity(Username);

            // TODO A: Input Prediction
            // http://www.gabrielgambetta.com/?p=22
            // need to store sequence number along with input
        }

        public void HandleNewPlayerPosition(PlayerPositionPacket packet)
        {
            // TODO B: Check sequence and do magic
            CheckForEntity(packet.username);
            PlayerEntities[packet.username].DoAction("ChangeAbsPosition", new ChangePositionArgs(packet.position));
        }

        private void CheckForEntity(string username)
        {
            // if player doesn't exist, create it
            if (!PlayerEntities.ContainsKey(username))
                PlayerEntities.Add(username, EntityFactory.GetNewPlayerEntityTemplate(username));
        }
    }
}
