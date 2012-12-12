using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using MVC_Oblig1.Models;

namespace MVC_Oblig1.Controllers
{
    public class PswinController : Controller
    {
        private PswinRepository psRep;

        public PswinController()
        {
            psRep = new PswinRepository();
        }

        //
        // GET: /pswin/

        public JsonResult Index()
        {
            psRep.lookupUserMessages();
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
