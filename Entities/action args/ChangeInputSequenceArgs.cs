using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.action_args
{
    public class ChangeInputSequenceArgs : ActionArgs
    {
        public byte NewSequence { get; set; }

        public ChangeInputSequenceArgs(byte seq)
        {
            this.NewSequence = seq;
        }
    }
}
