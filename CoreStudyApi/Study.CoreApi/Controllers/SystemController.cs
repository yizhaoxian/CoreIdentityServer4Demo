using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study.CoreApi.Models;
using Study.IRepository;
using Study.Models;

namespace Study.CoreApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class SystemController : Controller
    {
        [HttpGet]
        public string Get()
        {
            var roles = User.Claims.Where(l => l.Type == ClaimTypes.Role);
            return "访问成功，当前用户角色 " + string.Join(',', roles.Select(l => l.Value));
        }
    }
}
