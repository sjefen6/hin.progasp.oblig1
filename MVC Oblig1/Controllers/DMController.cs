using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Oblig1.Models;

namespace MVC_Oblig1.Controllers
{
    public class DMController : Controller
    {
        private ChannelRepository chRep;

        public DMController()
        {
            chRep = new ChannelRepository();
        }

        //
        // GET: /DM/
        [Authorize]
        public ActionResult Index()
        {
            var channels = chRep.showAllDMChannels(User.Identity.Name).ToList();
            return View(channels);
        }

        //
        // GET: /Channel/Display/5
        [Authorize]
        public ActionResult Display(string id)
        {
            DisplayModel display = new DisplayModel();
            display.ChannelName = id;
            display.Messages = new List<MessageFormViewModel>();
            foreach (Message m in chRep.getAllMessages(id, User.Identity.Name).ToList())
            {
                display.Messages.Add(new MessageFormViewModel(m));
            }
            display.isOp = false;
            display.isJoined = false;
            display.isClosed = false;
            return View(display);
        }

        //
        // GET: /Channel/SendMessage/5
        [Authorize]
        public ActionResult SendMessage(string id)
        {
            Message m = new Message();
            if (id != null)
                m.Receiver = chRep.userDB.getUser(id).UserId;
            MessageSendDM dm = new MessageSendDM(m);
            return View(dm);
        }

        //
        // POST: /Channel/SendMessage/5
        [Authorize]
        [HttpPost]
        public ActionResult SendMessage(string id, FormCollection collection)
        {
            // Whitelist
            List<string> wl = new List<string>();
            wl.Add("Content");

            // Message
            MessageSendDM dm = new MessageSendDM(new Message());

            // If id is set, use that userid, if not let it be updated with the model
            if (id != null)
                dm.Message.Receiver = chRep.userDB.getUser(id).UserId;
            else
                wl.Add("Receiver");

            try
            {
                UpdateModel(dm.Message, "Message", wl.ToArray());
                if (dm.Message.Receiver == null)
                    throw new Exception();
                chRep.sendMessage(dm.Message, User.Identity.Name);

                return RedirectToAction("Display", new { id = chRep.userDB.getUser((Guid) dm.Message.Receiver).UserName });
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }
    }
}
