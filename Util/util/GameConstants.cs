using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Entities.framework;
using Entities.components;
using Entities.actions;

namespace Util.util
{
    public static class GameConstants
    {
        public const string MissingUserKeyCombo = "Please enter username and keyword in Options before playing Multiplayer.";
        public const string DuplicateProfile = "You cannot create another profile with the same name as a previous.";
        public static Vector2 DefaultStartPosition = new Vector2(40, 40);

        // *** ADD TO THIS TEMPLATE AS WE GET NEW COMPONENTS ***
        public static Entity GetNewPlayerEntityTemplate(string username)
        {
            Entity newPlayer = new Entity();
            newPlayer.AddComponent(new Position(GameConstants.DefaultStartPosition));
            newPlayer.AddComponent(new Username(username));
            newPlayer.AddAction(new ChangeDeltaPosition());
            newPlayer.AddAction(new ChangeAbsPosition());
            return newPlayer;
        }

        // *** MAP ENTITY TO SAVE FILE HERE
        public static void DumpEntityIntoSaveFile(Entity ent, PlayerSave save)
        {
            save.Position = (ent.GetComponent("Position") as Position).Vector2Pos;
        }
    }
}
