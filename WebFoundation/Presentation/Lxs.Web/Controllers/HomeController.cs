using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Lxs.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Debug.WriteLine("1");
            //Debug.WriteLine(HttpRuntime.BinDirectory);
            //Debug.WriteLine("2");
            //Debug.WriteLine(AppDomain.CurrentDomain);



            return View();
        }

        public ActionResult About()
        {
           // ViewBag.Message = "Your application description page.";
           
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}