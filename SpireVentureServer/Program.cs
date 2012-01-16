using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace SpireVentureServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-= Spire Server 0.1 =-");
            Server server = new Server(false);
            server.Start();
        }
    }
}
