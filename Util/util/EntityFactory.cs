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
            newPlayer.AddComponent(new PreviousPosition(GameConstants.DefaultStartPosition));
            newPlayer.AddComponent(new InputSequence());

            newPlayer.AddAction(new ChangeDeltaPosition());
            newPlayer.AddAction(new ChangeAbsPosition());
            newPlayer.AddAction(new ChangeInputSequence());
            newPlayer.AddAction(new ChangeAbsPreviousPosition());
            newPlayer.AddAction(new ChangeCurrentSmoothing());

            return newPlayer;
        }

        public static Entity GetNewMobEntityTemplate()
        {
            return GetNewMobEntityTemplate(-1);
        }

        public static Entity GetNewMobEntityTemplate(int ID)
        {
            Entity newMob = new Entity();

            newMob.AddComponent(new Position(GameConstants.DefaultStartPosition));
            newMob.AddComponent(new PreviousPosition(GameConstants.DefaultStartPosition));

            if (ID == -1)
                newMob.AddComponent(new MobID(UMID));
            else
                newMob.AddComponent(new MobID(ID));

            newMob.AddAction(new ChangeDeltaPosition());
            newMob.AddAction(new ChangeAbsPosition());
            newMob.AddAction(new ChangeInputSequence());
            newMob.AddAction(new ChangeAbsPreviousPosition());
            newMob.AddAction(new ChangeCurrentSmoothing());

            return newMob;
        }

        // *** MAP ENTITY TO SAVE FILE HERE ***
        public static void DumpEntityIntoSaveFile(Entity ent, PlayerSave save)
        {
            save.Position = (ent.GetComponent("Position") as Position).Vector2Pos;
        }

        // for unique mob ids
        private static int CurrentMobID = 0;
        private static int UMID
        {
            get
            {
                if (CurrentMobID++ > 200)
                    CurrentMobID = 0;
                return CurrentMobID;
            }
        }
    }
}
