using System.Collections.Generic;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 发现响应
    /// </summary>
    public class DiscoveryResponse : BaseResponse
    {
        /// <summary>
        /// 发现到的服务信息集合
        /// </summary>
        public List<ServiceInfo> Services { get; set; }
    }
}
