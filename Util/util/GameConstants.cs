using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Util.util
{
    public static class GameConstants
    {
        public static SpriteFont FONT { get; set; }

        // *** VERSION NUMBER ***
        public const string VERSION = "0.0.3a";

        // *** UI ELEMENT CONSTANTS ***
        public const string MissingUserKeyCombo = "Please enter username and keyword in Options before playing Multiplayer.";
        public const string DuplicateProfile = "You cannot create another profile with the same name as a previous.";

        // *** IN GAME CONSTANTS ***
        public static Vector2 DefaultStartPosition = new Vector2(40, 40);

        // *** NETCODE CONSTANTS ***
        public const double SERVER_TICK_RATE = 50.0;
        public const double SERVER_UPDATE_RATE = 20.0; 
        public const double CLIENT_INPUT_RATE = 30.0;
        public const double CLIENT_UPDATE_RATE = 20.0;
        public const float CLIENT_LERP_LENGTH = 2.0f;
        public const float SERVER_LERP_LENGTH = 3.0f;

        // *** CHAT CONSTANTS ***
        public const int MAX_CHARACTERS_IN_MSG = 140;
        public const int NUMBER_OF_CHAT_LINES = 10;
        public const float CHAT_SCALE = 2f;
        public const int SECONDS_MSG_LASTS = 10 * 1000; // * 1000 because milliseconds
    }
}
