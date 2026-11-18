using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 服务注册响应
    /// </summary>
    public class RegisterResponse : BaseResponse
    {
        /// <summary>
        /// 注册标识字典
        /// </summary>
        public Dictionary<string, string> Signs { get; set; }
    }
}
