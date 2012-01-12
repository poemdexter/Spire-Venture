using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace SpireVentureServer
{
    class Server
    {
        private NetServer server;
        private bool running = false;

        public Server()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("SpireServer");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.NetworkThreadName = "Spire Server";
            config.Port = 9007;
            server = new NetServer(config);
        }

        public void Stop()
        {
            this.running = false;
        }

        public void Start()
        {
            this.server.Start();
            this.running = true;
            while (running)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            Console.WriteLine("discovery request");
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                        case NetIncomingMessageType.Data:
                            break;
                    }
                }
            }
        }
    }
}
