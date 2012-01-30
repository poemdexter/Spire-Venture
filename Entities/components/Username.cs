using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.framework;

namespace Entities.components
{
    public class Username : Component
    {
        public string UserNm { get; set; }

        public Username(string user)
        {
            this.Name = "Username";
            this.UserNm = user;
        }
    }
}
