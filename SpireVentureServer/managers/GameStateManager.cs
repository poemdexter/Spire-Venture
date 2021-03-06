﻿using System;
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
using Entities.action_args;

namespace SpireVentureServer.managers
{
    class GameStateManager
    {
        public Dictionary<string, PlayerSave> PlayerSaves;  // holds player data
        public Dictionary<string, Entity> PlayerEntities;  // holds player entities
        public List<Entity> Mobs;
        public BiDictionary RUIDUsernames;
        public MobFactory mobFactory;

        // TODO B: Fuck me this is temp
        public int mobCount = 0;

        private bool running = false;
        double now = 0;
        double nextUpdate = NetTime.Now;

        public GameStateManager()
        {
            PlayerSaves = new Dictionary<string, PlayerSave>();
            PlayerEntities = new Dictionary<string, Entity>();
            Mobs = new List<Entity>();
            RUIDUsernames = new BiDictionary();
            mobFactory = new MobFactory();
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
                    nextUpdate += (1.0 / GameConstants.SERVER_TICK_RATE);
                }
            }
        }

        private void Tick()
        {
            // TODO D: check if time to save players

            if (mobCount == 0)
            {
                Mobs.Add(mobFactory.getNewMob("skeleton"));
                mobCount++;
            }

            HandleMobsMoving();
        }

        private void HandleMobsMoving()
        {
            foreach (Entity mob in Mobs)
            {
                mob.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2((mob.GetComponent("Movement") as Movement).Velocity, 0)));
            }
        }

        // TODO D: This is really sloppy way to move players (5s should be part of velocity of player)
        public void HandlePlayerMoving(string username, InputsPacket inPacket)
        {
            Entity player = PlayerEntities[username];
            Vector2 delta = Vector2.Zero;
            Inputs input = inPacket.inputs;
            delta += (input.Up) ? new Vector2(0, -5) : Vector2.Zero;
            delta += (input.Down) ? new Vector2(0, 5) : Vector2.Zero;
            delta += (input.Left) ? new Vector2(-5, 0) : Vector2.Zero;
            delta += (input.Right) ? new Vector2(5, 0) : Vector2.Zero;
            player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(delta));
            player.DoAction("ChangeInputSequence", new ChangeInputSequenceArgs(inPacket.sequence));
        }

        public void HandleDisconnect(bool isLocalGame, string username)
        {
            if (PlayerEntities.ContainsKey(username))
            {
                EntityFactory.DumpEntityIntoSaveFile(PlayerEntities[username], PlayerSaves[username]);
                FileGrabber.SavePlayer(isLocalGame, PlayerSaves[username]);
                PlayerEntities.Remove(username);
                PlayerSaves.Remove(username);
                RUIDUsernames.Remove(username);
            }
        }

        public void SaveAllPlayerData(bool isLocalGame)
        {
            foreach (String username in PlayerEntities.Keys.ToList())
            {
                EntityFactory.DumpEntityIntoSaveFile(PlayerEntities[username], PlayerSaves[username]);
                FileGrabber.SavePlayer(isLocalGame, PlayerSaves[username]);
            }
        }

        public void createPlayerEntityFromSave(string username)
        {
            PlayerSave Save = PlayerSaves[username];
            Entity Player = EntityFactory.GetNewPlayerEntityTemplate(username);
            Player.DoAction("ChangeAbsPosition", new ChangePositionArgs(Save.Position));
            Player.DoAction("ChangeAbsPreviousPosition", new ChangePositionArgs(Save.Position));
            PlayerEntities.Add(username, Player);
        }
    }
}
