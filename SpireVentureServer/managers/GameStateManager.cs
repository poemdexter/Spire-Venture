using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.util;
using System.Net;
using Lidgren.Network;
using Entities.framework;
using Entities.components;
using Entities.actions;
using Microsoft.Xna.Framework;

namespace SpireVentureServer.managers
{
    class GameStateManager
    {
        public Dictionary<string, PlayerSave> PlayerSaves;  // holds player data
        public Dictionary<string, Entity> PlayerEntities;  // holds player entities
        public BiDictionary RUIDUsernames;

        private bool running = false;
        double now = 0;
        double nextUpdate = NetTime.Now;
        private double ticksPerSecond = 20.0;

        public GameStateManager()
        {
            PlayerSaves = new Dictionary<string, PlayerSave>();
            PlayerEntities = new Dictionary<string, Entity>();
            RUIDUsernames = new BiDictionary();
        }

        public void Start()
        {
            this.running = true;

            while (this.running)
            {
                now = NetTime.Now;
                if (now > nextUpdate)
                {
                    Tick();
                    nextUpdate += (1.0 / ticksPerSecond);
                }
            }
        }

        private void Tick()
        {
            // TODO D: check if time to save players
            // one tick of gametime
            // TODO C: add game tick
        }

        public void HandleDisconnect(bool isLocalGame, string username)
        {
            GameConstants.DumpEntityIntoSaveFile(PlayerEntities[username], PlayerSaves[username]);
            FileGrabber.SavePlayer(isLocalGame, PlayerSaves[username]);
            PlayerEntities.Remove(username);
            PlayerSaves.Remove(username);
            RUIDUsernames.Remove(username);
        }

        public void SaveAllPlayerData(bool isLocalGame)
        {
            foreach (String username in PlayerEntities.Keys.ToList())
            {
                GameConstants.DumpEntityIntoSaveFile(PlayerEntities[username], PlayerSaves[username]);
                FileGrabber.SavePlayer(isLocalGame, PlayerSaves[username]);
            }
        }

        public void createPlayerEntityFromSave(string username)
        {
            PlayerSave Save = PlayerSaves[username];
            Entity Player = new Entity();
            Player.AddComponent(new Position(Save.Position));
            Player.AddComponent(new Username(Save.Username));
            Player.AddAction(new ChangeAbsPosition());
            PlayerEntities.Add(username, Player);
        }
    }
}
