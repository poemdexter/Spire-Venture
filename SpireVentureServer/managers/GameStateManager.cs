﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.util;
using System.Net;
using Lidgren.Network;

namespace SpireVentureServer.managers
{
    class GameStateManager
    {
        public Dictionary<string, PlayerSave> PlayerSaves;
        public BiDictionary EndpointUsernames;

        private bool running = false;
        double now = 0;
        double nextUpdate = NetTime.Now;
        private double ticksPerSecond = 20.0;

        public GameStateManager()
        {
            PlayerSaves = new Dictionary<string, PlayerSave>();
            EndpointUsernames = new BiDictionary();
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
            // one tick of gametime
            // TODO: add game!
        }

        public void HandleDisconnect(bool isLocalGame, string username)
        {
            FileGrabber.SavePlayer(isLocalGame, PlayerSaves[username]);
            PlayerSaves.Remove(username);
            EndpointUsernames.Remove(username);
        }
    }
}