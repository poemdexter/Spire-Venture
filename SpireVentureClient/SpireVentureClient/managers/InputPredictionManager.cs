using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpireVenture.managers
{
    public class InputPredictionManager
    {
        public class InputSnapshot
        {
            public byte Sequence { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Delta { get; set; }

            public InputSnapshot(byte sequence, Vector2 position, Vector2 delta)
            {
                Sequence = sequence;
                Position = position;
                Delta = delta;
            }
        }

        // copy of the input requests sent to server
        // needed for server reconciliation of input
        public Queue<InputSnapshot> InputRequestQueue;

        public InputPredictionManager()
        {
            InputRequestQueue = new Queue<InputSnapshot>();
        }

        public void addNewInput(Vector2 position, Vector2 delta)
        {
            InputRequestQueue.Enqueue(new InputSnapshot(ClientGameManager.Instance.NewSequenceKey, position, delta));
        }
    }
}
