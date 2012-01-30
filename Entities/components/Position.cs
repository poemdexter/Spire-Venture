using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Microsoft.Xna.Framework;

namespace Entities.components
{
    public class Position : Component
    {
        public Vector2 Vector2Pos { get; set; }

        public Position(Vector2 pos)
        {
            this.Name = "Position";
            this.Vector2Pos = pos;
        }
    }
}
