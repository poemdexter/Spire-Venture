using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.action_args
{
    public class ChangeCurrentSmoothingArgs : ActionArgs
    {
        public float Delta { get; set; }

        public ChangeCurrentSmoothingArgs(float delta)
        {
            this.Delta = delta;
        }
    }
}
