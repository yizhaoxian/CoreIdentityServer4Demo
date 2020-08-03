using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreWeb.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必填")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "必填")]
        public string PassWord { get; set; }

        public string ReturnUrl { get; set; }
    }
}
