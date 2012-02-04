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
    public class ChangeCurrentSmoothing : EntityAction
    {
        public ChangeCurrentSmoothing()
        {
            this.Name = "ChangeCurrentSmoothing";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is ChangeCurrentSmoothingArgs)
            {
                PreviousPosition position = this.Entity.GetComponent("PreviousPosition") as PreviousPosition;
                if (position != null)
                {
                    position.currentSmoothing = ((ChangeCurrentSmoothingArgs)args).Delta;
                }
            }
        }
    }
}
