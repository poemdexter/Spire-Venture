using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using SpireVentureServer.managers;
using System.Threading;
using Util.util;
using System.Net;
using Entities.framework;
using Entities.components;

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

        private Queue<ChatMessage> ChatMessageQueue;

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
            ChatMessageQueue = new Queue<ChatMessage>();
        }

        public void Stop()
        {
            this.running = false;
            ServerConsoleMessage("Stopping Server");
            ServerConsoleMessage("Saving Player Data");
            gameManager.SaveAllPlayerData(isLocalGame);
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
                            ServerConsoleMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.DebugMessage:
                            ServerConsoleMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            ServerConsoleMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                            ServerConsoleMessage(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            switch ((NetConnectionStatus)msg.ReadByte())
                            {
                                case NetConnectionStatus.Connected:
                                    ServerConsoleMessage("connected");
                                    break;
                                case NetConnectionStatus.Disconnecting:
                                    ServerConsoleMessage("disconnecting");
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    string user = gameManager.RUIDUsernames.GetValue(msg.SenderConnection.RemoteUniqueIdentifier);
                                    ServerConsoleMessage(user + " disconnected");
                                    gameManager.HandleDisconnect(isLocalGame, user);
                                    if (!isLocalGame)
                                    {
                                        ChatMessageQueue.Enqueue(new ChatMessage("SERVER", user + " has disconnected."));
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
                    List<ChatMessage> chats = getChatMessageList();

                    // for each connected client
                    foreach (NetConnection connection in server.Connections)
                    {
                        // send all chat messages in queue
                        if (chats.Count > 0)
                        {
                            foreach (ChatMessage chatmsg in chats)
                            {
                                ChatMessagePacket chatpacket = new ChatMessagePacket();
                                chatpacket.message = chatmsg.getChatString();
                                SendReliableData(chatpacket, connection);
                            }
                        }

                        // update all player locations
                        if (gameManager.PlayerEntities.Count > 0)
                        {
                            List<Entity> players = gameManager.PlayerEntities.Values.ToList();
                            foreach (Entity entity in players)
                            {
                                PlayerPositionPacket positionPacket = new PlayerPositionPacket();
                                positionPacket.username = (entity.GetComponent("Username") as Username).UserNm;
                                positionPacket.position = (entity.GetComponent("Position") as Position).Vector2Pos;
                                SendUnreliableData(positionPacket, connection);
                            }
                        }
                    }

                    nextUpdate += (1.0 / ticksPerSecond);
                }

                Thread.Sleep(1);
            }
            ServerConsoleMessage("Stopping Server...");
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
                        ServerConsoleMessage(unkwPacket.username + " login error: Already logged in.");
                        SendUnconnectedMessage(packet, msg.SenderEndpoint);
                    }
                    else
                    {
                        PlayerSave save = FileGrabber.getPlayerSave(isLocalGame, unkwPacket.username, unkwPacket.keyword);
                        if (!unkwPacket.keyword.Equals(save.Keyword))
                        {
                            LoginVerificationPacket packet = new LoginVerificationPacket();
                            packet.message = "Keyword does not match.";
                            ServerConsoleMessage(unkwPacket.username + " login error: Bad keyword.");
                            SendUnconnectedMessage(packet, msg.SenderEndpoint);
                        }
                        else
                        {
                            LoginVerificationPacket packet = new LoginVerificationPacket();
                            packet.message = "verified";
                            gameManager.PlayerSaves.Add(unkwPacket.username, save);
                            gameManager.createPlayerEntityFromSave(unkwPacket.username);
                            gameManager.RUIDUsernames.Add(unkwPacket.username, msg.SenderConnection.RemoteUniqueIdentifier);
                            SendUnconnectedMessage(packet, msg.SenderEndpoint);
                            ServerConsoleMessage(unkwPacket.username + " has logged into the game.");
                            if (!isLocalGame)
                            {
                                ChatMessageQueue.Enqueue(new ChatMessage("SERVER", unkwPacket.username + " has connected."));
                            }
                        }
                    }
                    break;

                case PacketType.ChatMessage:
                    ChatMessagePacket chatPacket = new ChatMessagePacket();
                    chatPacket.Unpack(msg);
                    string username = gameManager.RUIDUsernames.GetValue(msg.SenderConnection.RemoteUniqueIdentifier);
                    ChatMessage cmsg = new ChatMessage(username,chatPacket.message);
                    ChatMessageQueue.Enqueue(cmsg);
                    ServerConsoleMessage(cmsg.getChatString());
                    break;

                case PacketType.InputsPacket:
                    InputsPacket inputsPacket = new InputsPacket();
                    inputsPacket.Unpack(msg);
                    string un = gameManager.RUIDUsernames.GetValue(msg.SenderConnection.RemoteUniqueIdentifier);
                    gameManager.HandlePlayerMoving(gameManager.RUIDUsernames.GetValue(msg.SenderConnection.RemoteUniqueIdentifier), inputsPacket.inputs);
                    break;
            }
        }

        private void ServerConsoleMessage(string message)
        {
            if (!isLocalGame) Console.WriteLine(message);
        }

        public void SendUnconnectedMessage(iPacket packet, IPEndPoint receiver)
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg = packet.Pack(sendMsg);
            server.SendUnconnectedMessage(sendMsg, receiver);
        }

        public void SendUnreliableData(iPacket packet, NetConnection recip )
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg = packet.Pack(sendMsg);
            server.SendMessage(sendMsg, recip, NetDeliveryMethod.Unreliable);
        }

        public void SendReliableData(iPacket packet, NetConnection recip)
        {
            NetOutgoingMessage sendMsg = server.CreateMessage();
            sendMsg = packet.Pack(sendMsg);
            server.SendMessage(sendMsg, recip, NetDeliveryMethod.ReliableUnordered);
        }

        public List<ChatMessage> getChatMessageList()
        {
            List<ChatMessage> temp = ChatMessageQueue.ToList();
            ChatMessageQueue.Clear();
            return temp;
        }
    }
}