using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lxs.Core
{
    public interface IWebHelper
    {
        /// <summary>
        /// 获得 URL 访问来源
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// 获得 IP 地址
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// 获得当前页链接
        /// </summary>
        /// <param name="includeQueryString">是否包含查询字符串</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString);

        /// <summary>
        /// 获得当前页链接
        /// </summary>
        /// <param name="includeQueryString">是否包含查询字符串</param>
        /// <param name="useSsl">是否需要 SSL 保护页</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// 获取一个值，该值指示当前的连接是否安全的
        /// </summary>
        /// <returns>true - 安全, false - 不安全</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// 按名称获取服务器变量
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        string ServerVariables(string name);

        /// <summary>
        /// 获取店铺主机位置
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Store host location</returns>
        string GetStoreHost(bool useSsl);

        /// <summary>
        /// 获取店铺位置
        /// </summary>
        /// <returns>Store location</returns>
        string GetStoreLocation();

        /// <summary>
        /// 获取店铺位置
        /// </summary>
        /// <param name="useSsl">使用 SSL</param>
        /// <returns>Store location</returns>
        string GetStoreLocation(bool useSsl);

        /// <summary>
        /// 如果所请求的资源是一种不必由 cms 引擎处理的典型资源返回 true。
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        /// <remarks>
        /// These are the file extensions considered to be static resources:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        bool IsStaticResource(HttpRequest request);

        /// <summary>
        /// 将虚拟路径映射到物理磁盘的路径。
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);


        /// <summary>
        /// 修改查询字符串
        /// </summary>
        /// <param name="url">需要修改的Url</param>
        /// <param name="queryStringModification">查询字符串修改</param>
        /// <param name="anchor">锚点</param>
        /// <returns>New url</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// 删除 url 中的查询字符串
        /// </summary>
        /// <param name="url">需要修改的Url</param>
        /// <param name="queryString">要移除的查询字符串</param>
        /// <returns>New url</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// 获取名称获取查询字符串值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">参数名</param>
        /// <returns>Query string value</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// 重新启动应用程序域
        /// </summary>
        /// <param name="makeRedirect">指示是否应该作出重定向后重新启动</param>
        /// <param name="redirectUrl">重定向 URL ；空字符串 （如果您想要将重定向到当前页面的 URL）</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");

        /// <summary>
        /// 获取一个值，该值指示客户端是否重定向到新的位置
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        bool IsPostBeingDone { get; set; }
    }
}
