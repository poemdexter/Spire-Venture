using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.util
{
    public struct Inputs
    {
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Space { get; set; }

        public Inputs(int up, int down, int left, int right, int space)
            : this()
        {
            this.Up = (up.Equals(1)) ? true : false;
            this.Down = (down.Equals(1)) ? true : false;
            this.Left = (left.Equals(1)) ? true : false;
            this.Right = (right.Equals(1)) ? true : false;
            this.Space = (space.Equals(1)) ? true : false;
        }

        public bool HasKeyDown()
        {
            return this.Up || this.Down || this.Left || this.Right || this.Space;
        }

        public void resetStates()
        {
            this.Up = false;
            this.Down = false;
            this.Left = false;
            this.Right = false;
            this.Space = false;
        }

        public void setStates(int up, int down, int left, int right, int space)
        {
            this.Up = (up.Equals(1)) ? true : false;
            this.Down = (down.Equals(1)) ? true : false;
            this.Left = (left.Equals(1)) ? true : false;
            this.Right = (right.Equals(1)) ? true : false;
            this.Space = (space.Equals(1)) ? true : false;
        }

        public bool[] getStateList()
        {
            return new bool[] { this.Up, this.Down, this.Left, this.Right, this.Space };
        }
    }
}
