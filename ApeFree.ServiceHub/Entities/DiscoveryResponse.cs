using System.Collections.Generic;

namespace ApeFree.ServiceHub.Entities
{
    public class DiscoveryResponse:BaseResponse
    {
        public List<ServiceInfo> Services { get; set; }
    }
}
