using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.util
{
    [Serializable()]
    public class PlayerSave
    {
        public string Username { get; set; }

        public PlayerSave(string username)
        {
            Username = username;
        }
    }
}
