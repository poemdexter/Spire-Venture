using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Entities.action_args;
using Entities.components;
using Microsoft.Xna.Framework;

namespace Entities.actions
{
    public class ChangeAbsPreviousPosition : EntityAction
    {
        public ChangeAbsPreviousPosition()
        {
            this.Name = "ChangeAbsPreviousPosition";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is ChangePositionArgs)
            {
                PreviousPosition position = this.Entity.GetComponent("PreviousPosition") as PreviousPosition;
                if (position != null)
                {
                    Vector2 newPosition = ((ChangePositionArgs)args).Delta;
                    position.Vector2Pos = newPosition;
                }
            }
        }
    }
}
