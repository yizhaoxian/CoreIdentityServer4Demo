using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreApi.Models
{
    public class ResultData<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
    public enum ResultCodeEnum
    {
        [Description("成功")]
        SUCCESS = 200,
        [Description("找不到所需数据")]
        DATA_NULL = 204,
        [Description("失败")]
        FAIL = 400,
        [Description("没有权限")]
        UNAUTHORIZED = 403,
        [Description("访问的接口不存在")]
        NOT_FOUND = 404,
        [Description("服务器内部错误")]
        INTERNAL_SERVER_ERROR = 500
    }
}
