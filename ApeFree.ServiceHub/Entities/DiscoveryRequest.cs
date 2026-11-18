using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 发现请求
    /// </summary>
    public class DiscoveryRequest
    {
        /// <summary>
        /// 发现标识
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 检索类型
        /// </summary>
        public DiscoveryType DiscoveryType { get; set; }

        /// <summary>
        /// 请求方IP地址
        /// </summary>
        public string IPAddress { get; set; }
    }
}
