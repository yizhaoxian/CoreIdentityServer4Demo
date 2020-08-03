using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study.CoreApi.Models;
using Study.IRepository;
using Study.Models;

namespace Study.CoreApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class SuiBianController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var user = User;
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
