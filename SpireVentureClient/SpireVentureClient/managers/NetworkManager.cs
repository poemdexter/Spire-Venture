using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace SpireVenture.managers
{
    public class NetworkManager
    {
        private static NetworkManager instance;
        private NetClient client;

        // i'm a singleton!
        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }

        public bool Discovered
        {
            get
            {
                NetIncomingMessage msg;
                while ((msg = client.ReadMessage()) != null)
                {
                    if (msg.MessageType == NetIncomingMessageType.DiscoveryResponse)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private NetworkManager()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("SpireServer");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.NetworkThreadName = "Spire Client";
            client = new NetClient(config);
        }

        public void Start(String ip)
        {
            client.Start();
            client.DiscoverKnownPeer(ip, 9007);
        }
    }
}
