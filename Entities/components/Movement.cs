using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.components
{
    public class Movement : Component
    {
        public float Velocity { get; set; }

        public Movement(int velocity)
        {
            this.Name = "Movement";
            this.Velocity = velocity;
        }
    }
}
