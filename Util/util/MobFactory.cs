using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Entities.components;
using System.Xml.Linq;

namespace Util.util
{
    public class MobFactory
    {
        private Dictionary<string, Entity> MobDictionary;
        
        public MobFactory()
        {
            MobDictionary = new Dictionary<string, Entity>();

            var doc = XDocument.Load("data/Mobs.xml");
            var nodes = from node in doc.Root.Descendants("mob") select node;

            foreach (var n in nodes)
            {
                Entity mob = EntityFactory.GetNewMobEntityTemplate();
                string mobname = "";
                foreach (var child in n.Descendants())
                {
                    switch (child.Name.ToString())
                    {
                        case "name":
                            mob.AddComponent(new Username(child.Value));
                            mobname = child.Value;
                            break;
                        case "speed":
                            mob.AddComponent(new Movement(int.Parse(child.Value)));
                            break;
                    }
                }
                MobDictionary.Add(mobname, mob);
            }
        }

        public Entity getNewMob(string name)
        {
            if (MobDictionary.ContainsKey(name))
                return MobDictionary[name];
            else
                return null;
        }
    }
}
