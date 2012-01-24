using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SpireVentureServer
{
    class BiDictionary
    {
        Dictionary<long, string> RUIDtoPlayerDict;
        Dictionary<string, long> PlayertoRUIDDict;

        public BiDictionary()
        {
            RUIDtoPlayerDict = new Dictionary<long, string>();
            PlayertoRUIDDict = new Dictionary<string, long>();
        }

        public void Add(string username, long RUID)
        {
            RUIDtoPlayerDict.Add(RUID, username);
            PlayertoRUIDDict.Add(username, RUID);
        }

        public void Remove(string username)
        {
            long RUID = PlayertoRUIDDict[username];
            RUIDtoPlayerDict.Remove(RUID);
            PlayertoRUIDDict.Remove(username);
        }

        public void Remove(long RUID)
        {
            string username = RUIDtoPlayerDict[RUID];
            RUIDtoPlayerDict.Remove(RUID);
            PlayertoRUIDDict.Remove(username);
        }

        public long GetValue(string usernameKey)
        {
            return PlayertoRUIDDict[usernameKey];
        }

        public string GetValue(long endPointKey)
        {
            return RUIDtoPlayerDict[endPointKey];
        }

        public bool UserExists(string username)
        {
            return PlayertoRUIDDict.ContainsKey(username);
        }
    }
}
