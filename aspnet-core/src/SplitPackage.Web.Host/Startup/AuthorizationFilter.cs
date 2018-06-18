using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SplitPackage.Split.Dto;

namespace SplitPackage.Web.Host.Startup
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IEventBus _eventBus;

        public AuthorizationFilter(
            IAuthorizationHelper authorizationHelper,
            IErrorInfoBuilder errorInfoBuilder,
            IEventBus eventBus)
        {
            _authorizationHelper = authorizationHelper;
            _errorInfoBuilder = errorInfoBuilder;
            _eventBus = eventBus;
            Logger = NullLogger.Instance;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var isWebserviceWay = false;
            if (context.HttpContext.Request.Headers.ContainsKey("requestWay") || context.HttpContext.Request.Headers["requestWay"].ToString() != "webapi")
            {
                isWebserviceWay = true;
            }

            //TODO: Avoid using try/catch, use conditional checking
            try
            {
                await _authorizationHelper.AuthorizeAsync(
                    context.ActionDescriptor.GetMethodInfo(),
                    context.ActionDescriptor.GetMethodInfo().DeclaringType
                );
            }
            catch (AbpAuthorizationException ex)
            {
                Logger.Warn(ex.ToString(), ex);

                _eventBus.Trigger(this, new AbpHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    if (isWebserviceWay)
                    {
                        context.Result = new ObjectResult(new ResultMessage<object>(ResultCode.Auth_Error, "authorization failure"));
                    }
                    else
                    {
                        context.Result = new ObjectResult(new AjaxResponse(_errorInfoBuilder.BuildForException(ex), true))
                        {
                            StatusCode = context.HttpContext.User.Identity.IsAuthenticated? (int)System.Net.HttpStatusCode.Forbidden:(int)System.Net.HttpStatusCode.Unauthorized
                        };
                    }
                }
                else
                {
                    if (isWebserviceWay)
                    {
                        context.Result = new ObjectResult(new ResultMessage<object>(ResultCode.Auth_Error, "authorization failure"));
                    }
                    else
                    {
                        context.Result = new ChallengeResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);

                _eventBus.Trigger(this, new AbpHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    if (isWebserviceWay)
                    {
                        context.Result = new ObjectResult(new ResultMessage<object>(ResultCode.SytemError, "Unknown exception"));
                    }
                    else
                    {
                        context.Result = new ObjectResult(new AjaxResponse(_errorInfoBuilder.BuildForException(ex)))
                        {
                            StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                        };
                    }
                }
                else
                {
                    if (isWebserviceWay)
                    {
                        context.Result = new ObjectResult(new ResultMessage<object>(ResultCode.SytemError, "Unknown exception"));
                    }
                    else
                    {
                        //TODO: How to return Error page?
                        context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }
        }
    }
}
