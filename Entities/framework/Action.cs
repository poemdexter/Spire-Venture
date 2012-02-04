using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entities.framework
{
    public class EntityAction
    {
        public string Name { get; set; }
        public Entity Entity { get; set; }

        public virtual void Do() { }
        public virtual void Do(ActionArgs args) { }
    }
}
