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

        // takes server authorized position and applies client inputs yet to be handled so we
        // have most up to date, server authorized position for player
        public Vector2 getReconciledPosition(byte sequence, Vector2 position)
        {
            if (InputRequestQueue.Count <= 1)
                return position;

            InputSnapshot snapshot = InputRequestQueue.Dequeue();
            while (snapshot.Sequence != sequence) // dump everything old until we are in sync
                snapshot = InputRequestQueue.Dequeue();

            Vector2 newPosition = position;
            List<InputSnapshot> tempSnapshotList = InputRequestQueue.ToList();

            // apply input still not seen from server to client
            foreach (InputSnapshot shot in tempSnapshotList)
            {
                newPosition += shot.Delta;
            }

            return newPosition;
        }
    }
}
