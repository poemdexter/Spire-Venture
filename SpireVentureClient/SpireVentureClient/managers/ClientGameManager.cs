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

        public void HandleNewPlayerPosition(string username, Vector2 newPosition)
        {
            // if player doesn't exist, create it
            if (!PlayerEntities.ContainsKey(username))
                PlayerEntities.Add(username, GameConstants.GetNewPlayerEntityTemplate(username));
            
            PlayerEntities[username].DoAction("ChangeAbsPosition", new ChangePositionArgs(newPosition));
        }
    }
}
