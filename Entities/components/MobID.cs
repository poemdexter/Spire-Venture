using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.components
{
    public class MobID : Component
    {
        public int ID { get; set; }

        public MobID(int id)
        {
            this.Name = "MobID";
            this.ID = id;
        }
    }
}
