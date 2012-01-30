using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

namespace Util.util
{
    public interface iPacket
    {
        PacketType packetType { get; }
        NetOutgoingMessage Pack(NetOutgoingMessage msg);
        void Unpack(NetIncomingMessage msg);
    }

    public enum PacketType
    {
        UsernameKeywordCombo,
        LoginVerification,
        ChatMessage,
        PlayerPosition
    }

    public class UsernameKeywordComboPacket : iPacket
    {
        public PacketType packetType { get { return PacketType.UsernameKeywordCombo; } }
        public string username { get; set; }
        public string keyword { get; set; }

        public NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            msg.Write((byte)packetType);
            msg.Write(username);
            msg.Write(keyword);
            return msg;
        }

        public void Unpack(NetIncomingMessage msg)
        {
            username = msg.ReadString();
            keyword = msg.ReadString();
        }
    }

    public class LoginVerificationPacket : iPacket
    {

        public PacketType packetType { get { return PacketType.LoginVerification; } }
        public string message { get; set; }

        public NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {   
            msg.Write((byte)packetType);
            msg.Write(message);
            return msg;
        }

        public void Unpack(NetIncomingMessage msg)
        {
            message = msg.ReadString();
        }
    }

    public class ChatMessagePacket : iPacket
    {

        public PacketType packetType { get { return PacketType.ChatMessage; } }
        public string message { get; set; }

        public NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            msg.Write((byte)packetType);
            msg.Write(message);
            return msg;
        }

        public void Unpack(NetIncomingMessage msg)
        {
            message = msg.ReadString();
        }
    }

    public class PlayerPositionPacket : iPacket
    {
        public PacketType packetType { get { return PacketType.PlayerPosition; } }
        public string username { get; set; }
        public Vector2 position { get; set; }

        public NetOutgoingMessage Pack(NetOutgoingMessage msg)
        {
            msg.Write((byte)packetType);
            msg.Write(username);
            msg.Write(position);
            return msg;
        }

        public void Unpack(NetIncomingMessage msg)
        {
            username = msg.ReadString();
            position = msg.ReadVector2();
        }
    }
}
