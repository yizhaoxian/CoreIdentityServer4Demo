using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
//using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreApi
{
    public class HttpHeaderOperation : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }  
            var actionAttrs = context.ApiDescription.CustomAttributes();
            var isAuthorized = actionAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));

            ////提供action都没有权限特性标记，检查控制器有没有
            //if (isAuthorized == false)
            //{
            //    var controllerAttrs = context.ApiDescription.ControllerAttributes();

            //    isAuthorized = controllerAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));
            //}

            var isAllowAnonymous = actionAttrs.Any(a => a.GetType() == typeof(AllowAnonymousAttribute));

            if (isAuthorized && isAllowAnonymous == false)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Authorization",  //添加Authorization头部参数 
                    Required = false,
                    In = ParameterLocation.Header
                });
            }
        }
    }
}
