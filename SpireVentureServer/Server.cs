using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using SpireVenture.util;
using SpireVentureServer.managers;
using System.Threading;

namespace SpireVentureServer
{
    public class Server
    {
        private NetServer server;
        private volatile bool running = false;
        private bool isLocalGame = false;

        private GameStateManager gameManager;

        public Server(bool local)
        {
            isLocalGame = local;
            NetPeerConfiguration config = new NetPeerConfiguration("SpireServer");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.NetworkThreadName = "Spire Server";
            config.Port = 9007;
            server = new NetServer(config);
            gameManager = new GameStateManager();
        }

        public void Stop()
        {
            this.running = false;
        }

        public void Start()
        {
            this.server.Start();

            Thread thread = new Thread(new ThreadStart(gameManager.Start));
            thread.Name = "GameStateManager";
            thread.Start();

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
                            HandlePacket(msg);
                            break;
                    }
                }
            }
            Console.Write("Stopping Server...");
            this.server.Shutdown("");
        }

        public void HandlePacket(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadByte();

            switch (type)
            {
                case PacketType.UsernameKeywordCombo:
                    UsernameKeywordComboPacket unkwPacket = new UsernameKeywordComboPacket();
                    unkwPacket.Unpack(msg);

                    if (isLocalGame && unkwPacket.keyword.Equals("local"))
                    {
                        // TODO: singleplayer so we need to get file from my documents
                        //http://www.java2s.com/Code/CSharp/File-Stream/CSerialization.htm
                    }
                    else 
                    {
                        // TODO: make sure client isn't logging in twice on same toon
                        // TODO: multiplayer so we need to get file from local storage
                        //http://www.java2s.com/Code/CSharp/File-Stream/CSerialization.htm
                    }

                    Console.Write("{0}:{1}", unkwPacket.username, unkwPacket.keyword);
                    break;
            }
        }
    }
}
