﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Util.util
{
    [Serializable()]
    public class PlayerSave
    {
        public string Username { get; set; }
        public string Keyword { get; set; }
        public Vector2 Position { get; set; }

        public PlayerSave(string username, string keyword)
        {
            Username = username;
            Keyword = keyword;
            Position = GameConstants.DefaultStartPosition; // default spot if new player
        }
    }
}
