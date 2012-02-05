using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;

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
        public ConcurrentQueue<InputSnapshot> InputRequestQueue;

        public InputPredictionManager()
        {
            InputRequestQueue = new ConcurrentQueue<InputSnapshot>();
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

            InputSnapshot snapshot; 
            InputRequestQueue.TryDequeue(out snapshot);
            while (snapshot.Sequence != sequence && InputRequestQueue.TryPeek(out snapshot)) // dump everything old until we are in sync
                InputRequestQueue.TryDequeue(out snapshot);

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
