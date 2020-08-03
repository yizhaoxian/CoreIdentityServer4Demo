using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreApi
{
    public class JwtSettings
    {
        /// <summary>
        /// 证书颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 允许使用的角色
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 加密字符串
        /// </summary>
        public string SecretKey { get; set; }
    }


    public class JwtPayload
    {
        /// <summary>
        /// Id
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string role { get; set; }
    }
}
