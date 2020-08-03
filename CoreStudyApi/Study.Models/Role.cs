using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Study.Models
{
    /// <summary>
    /// 商户权限 (Api权限分组)
    /// </summary>
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public DateTime CreateAt { get; set; }
        public int Status { get; set; }
    }
}
