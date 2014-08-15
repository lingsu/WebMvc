using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core;

namespace Lxs.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IWorkContext _workContext;
        public IWorkContext WorkContext
        {
            get
            {
                return _workContext;
            }
        }
        public override void InitHelpers()
        {
            base.InitHelpers();

            
            //_workContext = EngineContext.Current.Resolve<IWorkContext>();
        }
    }
    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
