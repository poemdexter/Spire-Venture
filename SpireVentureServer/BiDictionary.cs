using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SpireVentureServer
{
    class BiDictionary
    {
        Dictionary<IPEndPoint, string> IPtoPlayerDict;
        Dictionary<string, IPEndPoint> PlayertoIPDict;

        public BiDictionary()
        {
            IPtoPlayerDict = new Dictionary<IPEndPoint, string>();
            PlayertoIPDict = new Dictionary<string, IPEndPoint>();
        }

        public void Add(string username, IPEndPoint endPoint)
        {
            IPtoPlayerDict.Add(endPoint, username);
            PlayertoIPDict.Add(username, endPoint);
        }

        public void Remove(string username)
        {
            IPEndPoint endPoint = PlayertoIPDict[username];
            IPtoPlayerDict.Remove(endPoint);
            PlayertoIPDict.Remove(username);
        }

        public void Remove(IPEndPoint endPoint)
        {
            string username = IPtoPlayerDict[endPoint];
            IPtoPlayerDict.Remove(endPoint);
            PlayertoIPDict.Remove(username);
        }

        public IPEndPoint GetValue(string usernameKey)
        {
            return PlayertoIPDict[usernameKey];
        }

        public string GetValue(IPEndPoint endPointKey)
        {
            return IPtoPlayerDict[endPointKey];
        }

        public bool UserExists(string username)
        {
            return PlayertoIPDict.ContainsKey(username);
        }
    }
}
