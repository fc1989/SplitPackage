using Abp.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using SplitPackage.Authentication.BasicAuth;
using Abp.Dependency;
using Castle.Core.Logging;

namespace SplitPackage.Web.Host.Startup
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization") && 
                context.Request.Headers["Authorization"].ToString().StartsWith(BasicAuthenticationDefaults.AuthenticationScheme, true,System.Globalization.CultureInfo.CurrentCulture))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine($"请求url:{context.Request.Path}");
                sb.AppendLine("请求头信息:");
                foreach (var item in context.Request.Headers)
                {
                    sb.Append($"key:{item.Key},value:{item.Value.ToString()};");
                }
                sb.AppendLine();
                sb.AppendLine($"内容体:");
                var injectedRequestStream = new MemoryStream();
                try
                {
                    using (var bodyReader = new System.IO.StreamReader(context.Request.Body))
                    {
                        var body = bodyReader.ReadToEnd();
                        var bytesToWrite = Encoding.UTF8.GetBytes(body);
                        injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                        injectedRequestStream.Seek(0, SeekOrigin.Begin);
                        sb.Append(body);
                        IocManager.Instance.Resolve<ILoggerFactory>().Create("OpenApi").Debug(sb.ToString());
                        context.Request.Body = injectedRequestStream;
                    }
                    await _next.Invoke(context);
                    return;
                }
                finally
                {
                    injectedRequestStream.Dispose();
                }
            }
            await _next.Invoke(context);
        }
    }
}
