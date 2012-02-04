using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;
using Entities.action_args;
using Entities.components;

namespace Entities.actions
{
    public class ChangeInputSequence : EntityAction
    {
        public ChangeInputSequence()
        {
            this.Name = "ChangeInputSequence";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is ChangeInputSequenceArgs)
            {
                InputSequence sequence = this.Entity.GetComponent("InputSequence") as InputSequence;
                if (sequence != null)
                {
                    sequence.Sequence = ((ChangeInputSequenceArgs)args).NewSequence;
                }
            }
        }
    }
}
