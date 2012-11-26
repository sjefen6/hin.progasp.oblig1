using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Oblig1.Models
{
    public class DisplayModel
    {
        public string ChannelName;
        public List<Message> Messages;
        public Boolean isOp;
        public Boolean isJoined;
        public Boolean isClosed;
    }
}
