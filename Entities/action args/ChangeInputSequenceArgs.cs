using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.action_args
{
    public class ChangeInputSequenceArgs : ActionArgs
    {
        public Int16 NewSequence { get; set; }

        public ChangeInputSequenceArgs(Int16 seq)
        {
            this.NewSequence = seq;
        }
    }
}
