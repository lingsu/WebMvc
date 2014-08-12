using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lxs.Plugin.Payments.Alipay.Controllers
{
    public class AbcController : Controller
    {
        // GET: Abc
        public ActionResult Index()
        {
            return View("~/Plugins/Lxs.Plugin.Payments.Alipay/Views/Abc/Index.cshtml");
        }
    }
}