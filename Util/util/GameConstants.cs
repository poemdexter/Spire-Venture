using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Util.util
{
    public static class GameConstants
    {
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
    }
}
