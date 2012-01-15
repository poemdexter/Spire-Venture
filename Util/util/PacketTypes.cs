using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace SpireVenture.util
{
    public interface iPacket
    {
        PacketType packetType { get; }
        NetOutgoingMessage Pack(NetOutgoingMessage msg);
        void Unpack(NetIncomingMessage msg);
    }

    public enum PacketType
    {
        UsernameKeywordCombo
    }

    public class UsernameKeywordComboPacket : iPacket
    {
        public PacketType packetType { get { return PacketType.UsernameKeywordCombo; } }
        public String username { get; set; }
        public String keyword { get; set; }

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
}
