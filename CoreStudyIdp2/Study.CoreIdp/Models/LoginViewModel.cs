using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreIdp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必填")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "必填")]
        public string PassWord { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// 记住密码
        /// </summary>
        public bool RememberLogin { get; set; }
    }
}
