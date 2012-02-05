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

            // store previous position for lerp
            Vector2 currentPosition = (PlayerEntities[Username].GetComponent("Position") as Position).Vector2Pos;
            PlayerEntities[Username].DoAction("ChangeAbsPreviousPosition", new ChangePositionArgs(currentPosition));

            // store snapshot of move
            if (delta != Vector2.Zero)
                inputPredictionManager.addNewInput(currentPosition, delta);

            // make the move
            PlayerEntities[Username].DoAction("ChangeDeltaPosition", new ChangePositionArgs(delta));
            PlayerEntities[Username].DoAction("ChangeCurrentSmoothing", new ChangeCurrentSmoothingArgs(1));
        }

        public Vector2 LerpPlayer(Entity player)
        {

            Vector2 newPos = (player.GetComponent("Position") as Position).Vector2Pos;
            Vector2 oldPos = (player.GetComponent("PreviousPosition") as PreviousPosition).Vector2Pos;
            float smoothing = (player.GetComponent("PreviousPosition") as PreviousPosition).currentSmoothing;

            if ((player.GetComponent("Username") as Username).UserNm == Username)
            {
                smoothing -= 1.0f / GameConstants.CLIENT_LERP_LENGTH;
                if (smoothing < 0)
                    smoothing = 0;
            }
            else
            {
                smoothing -= 1.0f / GameConstants.SERVER_LERP_LENGTH;
                if (smoothing < 0)
                    smoothing = 0;
            }
            player.DoAction("ChangeCurrentSmoothing", new ChangeCurrentSmoothingArgs(smoothing));
            return Vector2.Lerp(newPos, oldPos, smoothing);
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
            {
                Vector2 oldPos = (PlayerEntities[packet.username].GetComponent("Position") as Position).Vector2Pos;
                PlayerEntities[packet.username].DoAction("ChangeAbsPreviousPosition", new ChangePositionArgs(oldPos));
                PlayerEntities[packet.username].DoAction("ChangeAbsPosition", new ChangePositionArgs(packet.position));
                PlayerEntities[packet.username].DoAction("ChangeCurrentSmoothing", new ChangeCurrentSmoothingArgs(1));
            }
        }

        public void HandlePlayerDisconnect(string name)
        {
            PlayerEntities.Remove(name);
        }

        public void CheckForEntity(string username)
        {
            // if player doesn't exist, create it
            if (!PlayerEntities.ContainsKey(username))
                PlayerEntities.Add(username, EntityFactory.GetNewPlayerEntityTemplate(username));
        }
    }
}
