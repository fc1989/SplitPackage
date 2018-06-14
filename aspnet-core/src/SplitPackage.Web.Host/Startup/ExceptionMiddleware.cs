using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Runtime.Validation;
using Abp.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SplitPackage.Authentication.BasicAuth;
using SplitPackage.Split.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Web.Host.Startup
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var isWebserviceWay = false;
            if (context.Request.Headers.ContainsKey("requestWay") || context.Request.Headers["requestWay"].ToString() != "webapi")
            {
                isWebserviceWay = true;
            }
            if (isWebserviceWay)
            {
                try
                {
                    await _next.Invoke(context);
                    if (context.Response.StatusCode == 404)
                    {
                        var result = new ResultMessage<object>(ResultCode.NoFind, string.Empty, null);
                        context.Response.ContentType = "application/json;charset=utf-8";
                        context.Response.StatusCode = (int)result.Code;
                        result.Message = "no find";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(result,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }));
                    }
                    else if (context.Response.StatusCode == 415)
                    {
                        var result = new ResultMessage<object>(ResultCode.UnsupportedMediaType, string.Empty, null);
                        context.Response.ContentType = "application/json;charset=utf-8";
                        context.Response.StatusCode = (int)result.Code;
                        result.Message = "Unsupported media type";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(result,
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }));
                    }
                    return;
                }
                catch (Exception ex)
                {
                    var code = ResultCode.SytemError;
                    string message = ex.Message;
                    if (ex is AbpValidationException)
                    {
                        code = ResultCode.BadRequest;
                        var detailBuilder = new StringBuilder();
                        foreach (var validationResult in ((AbpValidationException)ex).ValidationErrors)
                        {
                            detailBuilder.AppendFormat("{0} - {1}", string.Join(",", validationResult.MemberNames), validationResult.ErrorMessage);
                            detailBuilder.AppendLine();
                        }
                        message = $"{ex.Message}{detailBuilder.ToString()}";
                    }
                    else if (ex is UserFriendlyException)
                    {
                        code = (ResultCode)((UserFriendlyException)ex).Code;
                        message = ex.Message;
                    }
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json;charset=utf-8";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResultMessage<object>(code, message, null),
                            Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }));
                    return;
                }
            }
            await _next.Invoke(context);
        }
    }
}
