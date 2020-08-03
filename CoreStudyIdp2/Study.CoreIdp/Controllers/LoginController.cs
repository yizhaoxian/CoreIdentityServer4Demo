using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study.CoreIdp.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Stores;

namespace Study.CoreIdp.Controllers
{
    public class LoginController : Controller
    {
        private readonly TestUserStore _users;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;

        public LoginController(
            IIdentityServerInteractionService interaction,
            IEventService events,
            IClientStore clientStore,
            TestUserStore users = null)
        {
            _events = events;
            _users = users ?? new TestUserStore(TestUsers.Users);
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpGet]
        public IActionResult Index(string returnurl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(returnurl ?? "/Home");
            }
            ViewBag.ReturnUrl = returnurl ?? "/Home";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {

            //IdentityServer4.Validation.ScopeValidator


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "账号密码必填");
                return View(model);
            }

            if (!_users.ValidateCredentials(model.UserName, model.PassWord))
            {
                ModelState.AddModelError(string.Empty, "账号密码错误");
                return View(model);
            }

            //校验用户名、密码
            if (_users.ValidateCredentials(model.UserName, model.PassWord))
            {
                //获取用户实例
                var user = _users.FindByUsername(model.UserName);
                //触发登陆成功事件
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                AuthenticationProperties props = null;

                if (model.RememberLogin) //记住密码
                {
                    props = new AuthenticationProperties
                    {
                        IssuedUtc = DateTime.Now,
                        ExpiresUtc = DateTime.Now.AddDays(1),//设置自动过期时间
                        AllowRefresh = false,
                        IsPersistent = false,
                    };
                }
                // 登陆用户 保存到 cookie
                await HttpContext.SignInAsync(user.SubjectId, user.Username, props);

                // 获取授权上下文信息
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

                if (context != null)
                {
                    if (!string.IsNullOrWhiteSpace(context.ClientId))
                    {
                        //查找登陆客户端
                        var _clientTemp = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                        if (_clientTemp?.RequirePkce == true)
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new { RedirectUrl = model.ReturnUrl });
                        }
                        // model.ReturnUrl 验证上下文成功 直接跳转
                        return Redirect(model.ReturnUrl);
                    }
                }
            }

            return Redirect(model.ReturnUrl ?? "/Home");
        }
         
        [Authorize]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var logout = await _interaction.GetLogoutContextAsync(logoutId);
            await HttpContext.SignOutAsync(IdentityServerConstants.SignoutScheme);
            //先查看配置的注销跳转url
            if (!string.IsNullOrEmpty(logout.PostLogoutRedirectUri))
            {
                return Redirect(logout.PostLogoutRedirectUri);
            }
            // 如果没有配置就跳转回注销页面
            var refererUrl = Request.Headers["Referer"].ToString();
            return Redirect(refererUrl);
        }
    }
}
