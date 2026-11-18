using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 服务注册请求
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 要注册的服务信息集合
        /// </summary>
        public ServiceInfo[] ServiceInfoList { get; set; }

        /// <summary>
        /// 客户端信息
        /// </summary>
        public ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string IPAddress { get; set; }
    }
}
