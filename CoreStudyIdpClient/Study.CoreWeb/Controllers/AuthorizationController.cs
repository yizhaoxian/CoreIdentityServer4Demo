using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Study.CoreWeb.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult NoPermission()
        { 
            return Content("抱歉，您没有权限");
        }
    }
}
