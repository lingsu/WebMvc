using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;


namespace Lxs.Core
{
    public class WebHelper : IWebHelper
    {
        #region Fields

        private readonly HttpContextBase _httpContext;

        #endregion
        #region Utilities

        protected virtual Boolean IsRequestAvailable(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;

            try
            {
                if (httpContext.Request == null)
                    return false;
            }
            catch (HttpException ex)
            {
                return false;
            }

            return true;
        }
        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected virtual bool TryWriteGlobalAsax()
        {
            try
            {
                //When a new plugin is dropped in the Plugins folder and is installed into nopCommerce, 
                //even if the plugin has registered routes for its controllers, 
                //these routes will not be working as the MVC framework couldn't 
                //find the new controller types and couldn't instantiate the requested controller. 
                //That's why you get these nasty errors 
                //i.e "Controller does not implement IController".
                //The issue is described here: http://www.nopcommerce.com/boards/t/10969/nop-20-plugin.aspx?p=4#51318
                //The solution is to touch global.asax file
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

      
        public WebHelper(HttpContextBase httpContext)
        {
            this._httpContext = httpContext;
        }

        public string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            if (IsRequestAvailable(_httpContext) && _httpContext.Request.UrlReferrer !=null)
            {
                referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;
            }


            return referrerUrl;
        }

        public string GetCurrentIpAddress()
        {
            var result = string.Empty;

            if (!IsRequestAvailable(_httpContext))
            {
                return result;
            }

            if (_httpContext.Request.Headers != null)
            {
                //The X-Forwarded-For (XFF) HTTP header field is a de facto standard
                //for identifying the originating IP address of a client
                //connecting to a web server through an HTTP proxy or load balancer.
                var forwardedHttpHeader = "X-FORWARDED-FOR";

                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ForwardedHTTPheader"]))
                {
                    //but in some cases server use other HTTP header
                    //in these cases an administrator can specify a custom Forwarded HTTP header
                    forwardedHttpHeader = ConfigurationManager.AppSettings["ForwardedHTTPheader"];
                }

                //it's used for identifying the originating IP address of a client connecting to a web server
                //through an HTTP proxy or load balancer. 
                string xff = _httpContext.Request.Headers.AllKeys
                    .Where(x => forwardedHttpHeader.Equals(x, StringComparison.InvariantCultureIgnoreCase))
                    .Select(k => _httpContext.Request.Headers[k])
                    .FirstOrDefault();

                //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                if (!String.IsNullOrEmpty(xff))
                {
                    string lastIp = xff.Split(new char[] { ',' }).FirstOrDefault();
                    result = lastIp;
                }
            }

            if (String.IsNullOrEmpty(result) && _httpContext.Request.UserHostAddress != null)
            {
                result = _httpContext.Request.UserHostAddress;
            }

            //some validation
            if (result == "::1")
                result = "127.0.0.1";
            //remove port
            if (!String.IsNullOrEmpty(result))
            {
                int index = result.IndexOf(":", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                    result = result.Substring(0, index);
            }

            return result;
        }

        public string GetThisPageUrl(bool includeQueryString)
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        public string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            string url = string.Empty;
            if (!IsRequestAvailable(_httpContext))
                return url;

            if (includeQueryString)
            {
                string storeHost = GetStoreHost(useSsl);
                if (storeHost.EndsWith("/"))
                    storeHost = storeHost.Substring(0, storeHost.Length - 1);
                url = storeHost + _httpContext.Request.RawUrl;
            }
            else
            {
                if (_httpContext.Request.Url != null)
                {
                    url = _httpContext.Request.Url.GetLeftPart(UriPartial.Path);
                }
            }
            url = url.ToLowerInvariant();
            return url;
        }

        public bool IsCurrentConnectionSecured()
        {
            bool useSsl = false;
            if (IsRequestAvailable(_httpContext))
            {
                useSsl = _httpContext.Request.IsSecureConnection;
                //when your hosting uses a load balancer on their server then the Request.IsSecureConnection is never got set to true, use the statement below
                //just uncomment it
                //useSSL = _httpContext.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }

            return useSsl;
        }

        public string ServerVariables(string name)
        {
            string result = string.Empty;

            if (!IsRequestAvailable(_httpContext))
                return result;

            try
            {
                if (_httpContext.Request.ServerVariables[name] != null)
                {
                    result = _httpContext.Request.ServerVariables[name];
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public string GetStoreHost(bool useSsl)
        {
            throw new NotImplementedException();
        }

        public string GetStoreLocation()
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetStoreLocation(useSsl);
        }

        public string GetStoreLocation(bool useSsl)
        {
            string result = GetStoreHost(useSsl);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            if (IsRequestAvailable(_httpContext))
                result = result + _httpContext.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }

        public bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".htm":
                case ".html":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 将虚拟路径映射到物理磁盘的路径。
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }
      
        public string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            string result = string.Empty;

            if (String.IsNullOrWhiteSpace(url))
            {
                return result;
            }
            url = url.ToLowerInvariant();
            queryStringModification = queryStringModification.ToLowerInvariant();

            string str = string.Empty;
            string str2 = string.Empty;


            int ind = url.IndexOf('#');

            if (ind > -1)
            {
                str2 = url.Substring(ind + 1);
                url = url.Substring(0, ind);
            }


            ind = url.IndexOf('?');
            if (ind > -1)
            {
                str = url.Substring(ind+1);
                url = url.Substring(0, ind);
            }
            

            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();

                    foreach (var s in str.Split('&'))
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            var p = s.Split('=');
                            if (p.Length == 2)
                            {
                                if (!dictionary.ContainsKey(p[0]))
                                {
                                    dictionary[p[0]] = p[1];
                                }
                            }
                        }
                    }

                    foreach (var s in queryStringModification.Split('&'))
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            var p = s.Split('=');
                            if (p.Length == 2)
                            {
                                dictionary[p[0]] = p[1];
                            }
                            else
                            {
                                dictionary[s] = null;
                            }
                        }
                        
                    }

                    var sb = new StringBuilder();
                    foreach (var dict in dictionary)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("&");
                        }
                        sb.Append(dict.Key);
                        if (dict.Value != null)
                        {
                            sb.Append("=");
                            sb.Append(dict.Value);
                        }
                    }
                    str = sb.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }


            if (anchor !=null)
            {
                str2 = anchor;
            }

            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        public string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();


            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        public T QueryString<T>(string name)
        {
            string queryParam = null;
            if (IsRequestAvailable(_httpContext) && _httpContext.Request.QueryString[name] != null)
                queryParam = _httpContext.Request.QueryString[name];

            //if (!String.IsNullOrEmpty(queryParam))
            //    return CommonHelper.To<T>(queryParam);

            return default(T);
        }

        public void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "")
        {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();

                TryWriteGlobalAsax();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    throw new Exception("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'web.config' file.");
                }

                success = TryWriteGlobalAsax();
                if (!success)
                {
                    throw new Exception("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                        "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                        "- run the application in a full trust environment, or" + Environment.NewLine +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (_httpContext != null && makeRedirect)
            {
                if (String.IsNullOrEmpty(redirectUrl))
                    redirectUrl = GetThisPageUrl(true);
                _httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }

        public bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContext.Response;
                return response.IsRequestBeingRedirected;
            }
            
        }

        public bool IsPostBeingDone
        {
            get
            {
                if (_httpContext.Items["lxs.IsPOSTBeingDone"] == null)
                    return false;
                return Convert.ToBoolean(_httpContext.Items["lxs.IsPOSTBeingDone"]);
            }
            set
            {
                _httpContext.Items["lxs.IsPOSTBeingDone"] = value;
            }
        }
    }
}
