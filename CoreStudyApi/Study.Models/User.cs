using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Models
{
    /// <summary>
    /// 商户
    /// </summary>
    public class Merchant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public DateTime CreateAt { get; set; }
        public int Status { get; set; } 
    }
}
