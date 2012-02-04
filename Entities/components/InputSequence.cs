using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.components
{
    public class InputSequence : Component
    {
        public Int16 Sequence { get; set; }

        public InputSequence()
        {
            this.Name = "InputSequence";
            this.Sequence = 1;
        }
    }
}
