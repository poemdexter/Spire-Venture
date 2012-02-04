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
        private InputPredictionManager inputPredictionManager;
        public Dictionary<string, Entity> PlayerEntities;  // holds player entities
        public string Username { get; set; }

        public byte Key = 1;
        public byte NewSequenceKey
        {
            get
            {
                if (Key++ > 100)
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
            inputPredictionManager = new InputPredictionManager();
        }

        public void setUsername(string name)
        {
            Username = name;
        }

        public void PredictPlayerFromInput(Inputs input)
        {
            CheckForEntity(Username);
            
            // Input Prediction
            // http://www.gabrielgambetta.com/?p=22

            Vector2 delta = Vector2.Zero;
            delta += (input.Up) ? new Vector2(0, -5) : Vector2.Zero;
            delta += (input.Down) ? new Vector2(0, 5) : Vector2.Zero;
            delta += (input.Left) ? new Vector2(-5, 0) : Vector2.Zero;
            delta += (input.Right) ? new Vector2(5, 0) : Vector2.Zero;

            if (delta != Vector2.Zero)
            {
                Vector2 currentPosition = (PlayerEntities[Username].GetComponent("Position") as Position).Vector2Pos;
                // store snapshot of move
                inputPredictionManager.addNewInput(currentPosition, delta);
                // make the move
                PlayerEntities[Username].DoAction("ChangeDeltaPosition", new ChangePositionArgs(delta));
            }

        }

        public void HandleNewPlayerPosition(PlayerPositionPacket packet)
        {
            // http://www.gabrielgambetta.com/?p=22
            // See "Server reconciliation"
            CheckForEntity(packet.username);

            if (Username == packet.username) // remember this only applies to us, the player
            {
                Vector2 newPosition = inputPredictionManager.getReconciledPosition(packet.sequence, packet.position);
                PlayerEntities[packet.username].DoAction("ChangeAbsPosition", new ChangePositionArgs(newPosition));
            }
            else
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
