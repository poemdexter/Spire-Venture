using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Entities.framework;

namespace Entities.components
{
    public class PreviousPosition : Component
    {
        public Vector2 Vector2Pos { get; set; }
        public float currentSmoothing { get; set; }

        public PreviousPosition(Vector2 pos)
        {
            this.Name = "PreviousPosition";
            this.Vector2Pos = pos;
            this.currentSmoothing = 0;
        }
    }
}
