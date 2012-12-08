using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Oblig1.Models;

namespace MVC_Oblig1.Controllers
{
    public class ChannelController : Controller
    {
        private ChannelRepository chRep;

        public ChannelController()
        {
            chRep = new ChannelRepository();
        }

        //
        // GET: /Channel/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var channels = chRep.showAllJoinedChannels(User.Identity.Name).ToList();
                return View(channels);
            }
            else
            {
                return RedirectToAction("All");
            }
        }

        //
        // GET: /Channel/All

        public ActionResult All()
        {
            var channels = chRep.showAllChannels().ToList();
            return View(channels);
        }

        //
        // GET: /Channel/Join

        [Authorize]
        public ActionResult Join(string id)
        {
            Channel c = new Channel();
            c.Name = id;
            return View(c);
        } 

        //
        // POST: /Channel/Join

        [Authorize]
        [HttpPost]
        public ActionResult Join(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Channel joinChannel = new Channel();
                String[] wl = { "Name" };
                UpdateModel(joinChannel, wl);
                try
                {
                    chRep.join(joinChannel, User.Identity.Name);
                }
                catch (NotInvitedException e)
                {
                    return View("NotInvited");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Error");
            }
        }

        //
        // GET: /Channel/Invite/5

        [Authorize]
        public ActionResult Invite(string id)
        {
            if (chRep.isOp(id, User.Identity.Name))
            {
                aspnet_User u = new aspnet_User();
                return View(u);
            }
            else
            {
                return View("Error");
            }
        }

        //
        // POST: /Channel/Join

        [Authorize]
        [HttpPost]
        public ActionResult Invite(string id, FormCollection collection)
        {
            if (chRep.isOp(id, User.Identity.Name)) try
            {
                // TODO: Add insert logic here
                chRep.invite(id, collection.Get("UserName"));

                return RedirectToAction("Index");
            }
            catch
            { }
            return View("Error");
        }

        //
        // GET: /Channel/Permissions/5

        public ActionResult Permissions(string id)
        {
            if (chRep.isOp(id, User.Identity.Name))
            {
                PermissionsModel p = new PermissionsModel();
                p.Permissions = chRep.getAllPermissions(id).ToList();
                p.ChannelName = id;
                return View(p);
            }
            return View("Error");
        }

        //
        // GET: /Channel/Display/5

        public ActionResult Display(string id)
        {
            DisplayModel display = new DisplayModel();
            display.ChannelName = id;
            display.Messages = new List<MessageFormViewModel>();
            foreach(Message m in chRep.getAllMessages(id).ToList())
            {
                display.Messages.Add(new MessageFormViewModel(m));
            }
            display.isOp = chRep.isOp(id, User.Identity.Name);
            display.isJoined = chRep.isJoined(id, User.Identity.Name);
            display.isClosed = chRep.isClosed(id);
            return View(display);
        }

        //
        // GET: /Channel/ChangeLevel/5
        [Authorize]
        public ActionResult ChangeLevel(string id, string user)
        {
            if (chRep.isOp(id, User.Identity.Name))
            {
                Permission p = chRep.getPermission(id, user);
                if (p != null)
                    return View(p);
            }
            return View("Error");
        }

        //
        // POST: /Channel/ChangeLevel/5
        [Authorize]
        [HttpPost]
        public ActionResult ChangeLevel(string id, string user, FormCollection collection)
        {
            if (chRep.isOp(id, User.Identity.Name))
            {
                try
                {
                    // TODO: Add insert logic here
                    Permission p = chRep.getPermission(id, user);
                    String[] wl = { "Level" };
                    UpdateModel(p, wl);
                    chRep.updatePermission(p);
                    return RedirectToAction("Permissions", new { id = id });
                }
                catch { }
            }
            return View("Error");
        }

        //
        // GET: /Channel/SendMessage/5
        
        [Authorize]
        public ActionResult SendMessage(string id)
        {
            Message m = new Message();
            m.ChannelName = id;
            return View(m);
        }

        //
        // POST: /Channel/SendMessage/5
        [Authorize]
        [HttpPost]
        public ActionResult SendMessage(string id, FormCollection collection)
        {
            // TODO: Add insert logic here
            Message newMessage = new Message();
            newMessage.ChannelName = id;
            try
            {
                String[] wl = { "Content" };
                UpdateModel(newMessage, wl);
                chRep.sendMessage(newMessage, User.Identity.Name);

                return RedirectToAction("Display", new { id = id });
            }
            catch
            {
                return View("Error");
            }
        }
        
        //
        // GET: /Channel/Settings/5

        public ActionResult Settings(int id)
        {
            return View();
        }

        //
        // POST: /Channel/Settings/5

        [HttpPost]
        public ActionResult Settings(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Channel/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Channel/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
