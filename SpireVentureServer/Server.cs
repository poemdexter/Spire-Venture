using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using SpireVentureServer.managers;
using System.Threading;
using Util.util;
using System.Net;

namespace SpireVentureServer
{
    public class Server
    {
        private NetServer server;
        private volatile bool running = false;
        private bool isLocalGame = false;

        double now = 0;
        double nextUpdate = NetTime.Now;
        private double ticksPerSecond = 20.0;

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
                // HANDLE INCOMING
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
                                    string user = gameManager.RUIDUsernames.GetValue(msg.SenderConnection.RemoteUniqueIdentifier);
                                    ServerMessage(user + " disconnected");
                                    gameManager.HandleDisconnect(isLocalGame, user);
                                    if (!isLocalGame)
                                    {
                                        // TODO F: tell everyone on server that someone disconnected 
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

                // HANDLE OUTGOING
                now = NetTime.Now;
                if (now > nextUpdate)
                {
                    // TODO A: add sending
                    nextUpdate += (1.0 / ticksPerSecond);
                }

                Thread.Sleep(1);
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
                        LoginVerificationPacket packet = new LoginVerificationPacket();
                        packet.message = "You are already logged in.";
                        ServerMessage(unkwPacket.username + " login error: Already logged in.");
                        SendAsOneTimeMessage(packet, msg.SenderEndpoint);
                    }
                    else
                    {
                        PlayerSave save = FileGrabber.getPlayerSave(isLocalGame, unkwPacket.username, unkwPacket.keyword);
                        if (!unkwPacket.keyword.Equals(save.Keyword))
                        {
                            LoginVerificationPacket packet = new LoginVerificationPacket();
                            packet.message = "Keyword does not match.";
                            ServerMessage(unkwPacket.username + " login error: Bad keyword.");
                            SendAsOneTimeMessage(packet, msg.SenderEndpoint);
                        }
                        else
                        {
                            LoginVerificationPacket packet = new LoginVerificationPacket();
                            packet.message = "verified";
                            SendAsOneTimeMessage(packet, msg.SenderEndpoint);

                            gameManager.PlayerSaves.Add(unkwPacket.username, save);
                            gameManager.RUIDUsernames.Add(unkwPacket.username, msg.SenderConnection.RemoteUniqueIdentifier);
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

        public void SendAsOneTimeMessage(iPacket packet, IPEndPoint receiver)
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg = packet.Pack(sendMsg);
            server.SendUnconnectedMessage(sendMsg, receiver);
        }
    }
}