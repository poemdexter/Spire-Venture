using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using SpireVentureServer;
using System.Threading;
using Util.util;

namespace SpireVenture.managers
{
    public class NetworkManager
    {
        private static NetworkManager instance;
        private NetClient client;
        private Server server;

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
                        client.Connect(msg.SenderEndpoint);
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

        public void SendData(iPacket packet)
        {
            NetOutgoingMessage sendMsg = client.CreateMessage();
            sendMsg = packet.Pack(sendMsg);
            client.SendMessage(sendMsg, NetDeliveryMethod.Unreliable);
        }

        public void SingleplayerStart()
        {
            // start Server
            server = new Server(true);
            Thread serverThread = new Thread(new ThreadStart(server.Start));
            serverThread.Name = "SpireVenture Singleplayer Server";
            serverThread.Start();
            
            // connect Client
            this.Start("localhost");

            // supery hackery way of waiting for discovery locally
            while (!this.Discovered) ;

            // we're connected to our local server now
        }

        public void StopSingleplayerServer()
        {
            client.Disconnect("");
            server.Stop();
        }
    }
}
