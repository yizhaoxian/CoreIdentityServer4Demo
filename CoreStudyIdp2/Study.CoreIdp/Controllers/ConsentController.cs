using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Study.CoreIdp.Models;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace Study.CoreIdp.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IEventService _events;
        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            IEventService events
            )
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _events = events;
        }
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await BuildModelAsync(returnUrl);
            return View(model);
        }

        private async Task<ConsentViewModel> BuildModelAsync(string returnUrl)
        {
            var model = new ConsentViewModel
            {
                ReturnUrl = returnUrl
            };

            //获取授权上下文
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
            {
                model.Error = "请求授权上下文错误";
                return model;
            }
            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            if (client == null)
            {
                model.Error = "请求授权客户端识别错误";
                return model;
            }
            model.ClientName = client.ClientName ?? client.ClientId;
            model.AllowedScopes = client.AllowedScopes;
            var resource = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            if (resource == null)
            {
                model.Error = "没有匹配的 Scopes";
                return model;
            }
            if (!resource.IdentityResources.Any() && !resource.ApiResources.Any())
            {
                model.Error = "客户端请求授权资源不可为空";
                return model;
            }


            if (resource.OfflineAccess)
            {
                resource.ApiResources.Add(new ApiResource()
                {
                    Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                    DisplayName = "Offline Access",
                    Description = "Offline Access 离线访问",
                });
            }
            model.apiResources = resource.ApiResources;
            model.identityResources = resource.IdentityResources;
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentModel model)
        {
            //获取授权上下文，验证返回 URL 是否有效
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
            {
                model.Error = "请求授权上下文失效";
                return RedirectToAction("error");
            }
            //同意-响应
            ConsentResponse grantedConsent = null;
            // 是否同意授权
            if (!model.ConsentBtn)
            {
                //拒绝
                grantedConsent = ConsentResponse.Denied;
                // 触发拒绝事件
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.ClientId, request.ScopesRequested));
                //return Redirect(model.ReturnUrl);
            }
            else
            {
                grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent
                };

                if (model.apis.Any())
                {
                    grantedConsent.ScopesConsented = model.identitys.Concat(model.apis);
                }
                else
                {
                    grantedConsent.ScopesConsented = model.identitys;
                }
                //grantedConsent.ScopesConsented = grantedConsent.ScopesConsented.Where(l => l != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);

                var subjectId = User.GetSubjectId();
                // 触发授权事件
                await _events.RaiseAsync(new ConsentGrantedEvent(subjectId, request.ClientId, request.ScopesRequested, grantedConsent.ScopesConsented, grantedConsent.RememberConsent));

            }

            // 将授权的结果传达回IdentityServer
            await _interaction.GrantConsentAsync(request, grantedConsent);


            if (!string.IsNullOrWhiteSpace(request.ClientId))
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
                //指定基于授权码的令牌请求是否需要证明密钥（默认为false）。
                if (client?.RequirePkce == true)
                {
                    model.Error = "请求授权上下文失效";
                    return RedirectToAction("error");
                } 
            } 
            return Redirect(model.ReturnUrl);
        }
    }
}
