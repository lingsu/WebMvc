using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Lxs.Core;
using Lxs.Core.Infrastructure;

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

            var webhellper = EngineContext.Current.Resolve<IWebHelper>();
            Debug.WriteLine(webhellper.GetUrlReferrer());

            string url = "abc?aaa=2";

            Debug.WriteLine(webhellper.ModifyQueryString(url,"bb=3",null));

            ConstantExpression exp1 = Expression.Constant(1);
            ConstantExpression exp2 = Expression.Constant(2);

            BinaryExpression exp12 = Expression.Add(exp1, exp2);

            ConstantExpression exp3 = Expression.Constant(3);

            BinaryExpression exp123 = Expression.Add(exp12, exp3);


            ParameterExpression expA = Expression.Parameter(typeof(double), "a"); //参数a
            MethodCallExpression expCall = Expression.Call(null,
                typeof(Math).GetMethod("Sin", BindingFlags.Static | BindingFlags.Public),
                expA); //Math.Sin(a)

            LambdaExpression exp = Expression.Lambda(expCall, expA); // a => Math.Sin(a)
            var cc = exp.Compile();
            Debug.WriteLine(cc);
            return View();
        }

        public ActionResult About()
        {
           // ViewBag.Message = "Your application description page.";
            var webhellper = EngineContext.Current.Resolve<IWebHelper>();
            Debug.WriteLine(webhellper.GetUrlReferrer());
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            var webhellper = EngineContext.Current.Resolve<IWebHelper>();
            Debug.WriteLine(webhellper.GetUrlReferrer());
            return View();
        }
    }
}