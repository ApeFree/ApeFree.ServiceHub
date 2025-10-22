using ApeFree.ServiceHub.Entities;
using ApeFree.ServiceHub.RouteHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ApeFree.ServiceHub
{
    /// <summary>
    /// 发现服务
    /// </summary>
    public class DiscoveryService
    {
        private RequestDispatcher httpServer;
        private UdpHeartbeatListener udpServer;
        private Dictionary<string, ServiceInfo> serviceInfoList;
        private object writeReadLock = new object();

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 心跳超时时间，单位毫秒
        /// </summary>
        public int HeartbeatTime { get; set; } = 10000;

        public DiscoveryService(string address, int httpPort = 4555, int udpPort = 4556)
        {
            Address = address;
            serviceInfoList = new Dictionary<string, ServiceInfo>();

            // 初始化UDP服务器
            udpServer = new UdpHeartbeatListener(udpPort);
            udpServer.HeartbeatHandler = OnHeartbeatHandler;

            // 初始化HTTP服务器
            httpServer = new RequestDispatcher(this.Address, httpPort);
            httpServer.RegisterRequestHandler = OnRegisterRequestHandler;
            httpServer.DiscoveryRequestHandler = OnDiscoveryRequestHandler;
            httpServer.AddPrefix("Discovery", new DiscoveryHandler());
            httpServer.AddPrefix("Register", new RegisterHandler());
        }

        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bytes"></param>
        private void OnHeartbeatHandler(object sender, byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            var request = JsonConvert.DeserializeObject<HeartbeatRequest>(json);

            foreach (var serviceId in request.ServiceInfoIds)
            {
                if (serviceInfoList.TryGetValue(serviceId, out var serviceInfo) && serviceInfo.Address == request.IPAddress)
                {
                    serviceInfo.LastActiveTimestamp = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private Dictionary<string, string> OnRegisterRequestHandler(RegisterRequest request)
        {
            // 注册服务列表内部是否有重复
            if (request.ServiceInfoList.GroupBy(p => p.Name).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("注册列表中有重复项，请检查后重新注册");
            }

            lock (writeReadLock)
            {
                // 检查是否有重复名称的服务
                var duplicateServices = request.ServiceInfoList.Where(newService => serviceInfoList.Values.Any(existingService => existingService.Name == newService.Name)).ToList();

                if (duplicateServices.Any())
                {
                    throw new ArgumentException($"已存在同样名称的服务: {string.Join(", ", duplicateServices.Select(s => s.Name))}");
                }

                foreach (var info in request.ServiceInfoList)
                {
                    info.Id = Guid.NewGuid().ToString();
                    info.LastActiveTimestamp = DateTime.Now;
                    serviceInfoList.Add(info.Id, info);
                }

                return serviceInfoList.ToDictionary(x => x.Key, x => x.Value.Name);
            }
        }

        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private List<ServiceInfo> OnDiscoveryRequestHandler(DiscoveryRequest request)
        {
            lock (writeReadLock)
            {
                switch (request.DiscoveryType)
                {
                    case DiscoveryType.Id:
                        return serviceInfoList.Where(x => x.Key == request.Sign && (DateTime.Now - x.Value.LastActiveTimestamp).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    case DiscoveryType.Name:
                        return serviceInfoList.Where(x => x.Value.Name == request.Sign && (DateTime.Now - x.Value.LastActiveTimestamp).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    case DiscoveryType.Keywords:
                        return serviceInfoList.Where(x => x.Value.Keywords != null && x.Value.Keywords.Contains(request.Sign) && (DateTime.Now - x.Value.LastActiveTimestamp).TotalMilliseconds < HeartbeatTime).Select(x => x.Value).ToList();
                    default:
                        throw new InvalidOperationException("无法以未知的方式筛选服务");
                }
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            httpServer.Start();
            udpServer.Start();
        }

        /// <summary>
        /// 添加监听的路由和处理器
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        public void AddPrefix(string route, IRouteHandler handler)
        {
            httpServer.AddPrefix(route, handler);
        }
    }
}
