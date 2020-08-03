using IdentityServer4.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Study.CoreIdp.Models
{
    public class ConsentViewModel
    {
        //public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        //public IEnumerable<ScopeViewModel> ResourceScopes { get; set; }
        /// <summary>
        /// 请求授权的客户端
        /// </summary>
        public string ClientName { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }

        public IEnumerable<IdentityResource> identityResources { get; set; }
        public IEnumerable<ApiResource> apiResources { get; set; }

        public string ReturnUrl { get; set; }
        public string Error { get; set; }
    }

    public class ConsentModel
    {  
        public IEnumerable<string> identitys { get; set; }
        public IEnumerable<string> apis { get; set; } 
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 是否同意授权
        /// </summary>
        public bool ConsentBtn { get; set; }
        /// <summary>
        /// 是否记住授权
        /// </summary>
        public bool RememberConsent { get; set; }
        public string Error { get; set; }
    }
}
