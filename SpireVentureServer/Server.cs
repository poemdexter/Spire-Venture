using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using SpireVentureServer.managers;
using System.Threading;
using Util.util;

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
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                            ServerMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.DebugMessage:
                            ServerMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            ServerMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            ServerMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            switch ((NetConnectionStatus)msg.ReadByte())
                            {
                                case NetConnectionStatus.Connected:
                                    ServerMessage("connected");
                                    break;
                                case NetConnectionStatus.Disconnecting:
                                    ServerMessage("disconnecting");
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    string user = gameManager.EndpointUsernames.GetValue(msg.SenderEndpoint);
                                    ServerMessage(user + " disconnected");
                                    gameManager.HandleDisconnect(isLocalGame, user);
                                    if (!isLocalGame)
                                    {
                                        // TODO: Tell everyone on server that someone disconnected 
                                    }
                                    else this.Stop();
                                    break;
                                case NetConnectionStatus.InitiatedConnect:
                                case NetConnectionStatus.RespondedConnect:
                                case NetConnectionStatus.RespondedAwaitingApproval:
                                case NetConnectionStatus.None:
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            HandlePacket(msg);
                            break;
                    }
                }
            }
            ServerMessage("Stopping Server...");
            this.server.Shutdown("");
        }

        private void HandlePacket(NetIncomingMessage msg)
        {
            PacketType type = (PacketType)msg.ReadByte();

            switch (type)
            {
                case PacketType.UsernameKeywordCombo:
                    UsernameKeywordComboPacket unkwPacket = new UsernameKeywordComboPacket();
                    unkwPacket.Unpack(msg);
                    if (gameManager.PlayerSaves.ContainsKey(unkwPacket.username))
                    {
                        // TODO: Create packet that tells client someone's already logged in with that name.
                    }
                    else
                    {
                        PlayerSave save = FileGrabber.getPlayerSave(isLocalGame, unkwPacket.username, unkwPacket.keyword);
                        if (unkwPacket.keyword.Equals(save.Keyword))
                        {
                            // TODO: Create packet saying that his keyword doesn't match the save file with that name.
                        }
                        else
                        {
                            gameManager.PlayerSaves.Add(unkwPacket.username, save);
                            gameManager.EndpointUsernames.Add(unkwPacket.username, msg.SenderEndpoint);
                            ServerMessage(unkwPacket.username + " has logged into the game.");
                        }
                    }
                    break;
            }
        }

        private void ServerMessage(string message)
        {
            if (!isLocalGame) Console.WriteLine(message);
        }
    }
}