using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Entities.components;
using Microsoft.Xna.Framework;
using Entities.action_args;

namespace Entities.actions
{
    public class ChangeDeltaPosition : EntityAction
    {
        public ChangeDeltaPosition()
        {
            this.Name = "ChangeDeltaPosition";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is ChangePositionArgs)
            {
                Position position = this.Entity.GetComponent("Position") as Position;
                if (position != null)
                {
                    Vector2 delta = ((ChangePositionArgs)args).Delta;
                    position.Vector2Pos += delta;
                }
            }
        }
    }
}
