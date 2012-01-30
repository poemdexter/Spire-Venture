using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Microsoft.Xna.Framework;

namespace Entities.action_args
{
    public class ChangePositionArgs : ActionArgs
    {
        public Vector2 Delta { get; set; }

        public ChangePositionArgs(Vector2 delta)
        {
            this.Delta = delta;
        }
    }
}
