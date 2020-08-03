using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Study.CoreWeb
{
    public class BobAuthorizationHandler : AuthorizationHandler<BobRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BobRequirement requirement)
        {
            var content = context.Resource as AuthorizationFilterContext;
            if (content == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var givenname = context.User.Claims.FirstOrDefault(l => l.Type == JwtClaimTypes.GivenName)?.Value;
            if (givenname == "Bob")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
