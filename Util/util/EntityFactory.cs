using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Entities.components;
using Entities.actions;

namespace Util.util
{
    public static class EntityFactory
    {
        // *** ADD TO THIS TEMPLATE AS WE GET NEW COMPONENTS ***
        public static Entity GetNewPlayerEntityTemplate(string username)
        {
            Entity newPlayer = new Entity();
            newPlayer.AddComponent(new Position(GameConstants.DefaultStartPosition));
            newPlayer.AddComponent(new Username(username));
            newPlayer.AddAction(new ChangeDeltaPosition());
            newPlayer.AddAction(new ChangeAbsPosition());
            newPlayer.AddComponent(new InputSequence());
            newPlayer.AddAction(new ChangeInputSequence());
            return newPlayer;
        }

        // *** MAP ENTITY TO SAVE FILE HERE ***
        public static void DumpEntityIntoSaveFile(Entity ent, PlayerSave save)
        {
            save.Position = (ent.GetComponent("Position") as Position).Vector2Pos;
        }
    }
}
