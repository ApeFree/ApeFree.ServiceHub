using ApeFree.ServiceHub.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ApeFree.ServiceHub
{
    /// <summary>
    /// 服务发现客户端
    /// </summary>
    public class ServiceDiscoveryClient : IDisposable
    {
        private ClientInfo Info;
        private UdpClient udpClient;
        private HttpClient httpClient;
        private Timer heartbeatTimer;
        private Dictionary<string, string> ServicesInfoList;
        private IPEndPoint serviceIPAddress;

        public ServiceDiscoveryClient(string ipAddress, int httpPort = 4555, int udpPort = 4556)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"http://{ipAddress}:{httpPort}");

            Info = new ClientInfo()
            {
                ClientId = Guid.NewGuid().ToString(),
            };

            ServicesInfoList = new Dictionary<string, string>();

            serviceIPAddress = new IPEndPoint(IPAddress.Parse(ipAddress), udpPort);

            udpClient = new UdpClient();

            heartbeatTimer = new Timer();
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ServicesInfoList.Count <= 0)
            {
                return;
            }

            var bytes = new HeartbeatRequest(Info, ServicesInfoList.Keys.ToArray()).GetBytes();
            udpClient.Send(bytes, bytes.Length, serviceIPAddress);
        }

        public async Task<RegisterResponse> Register(ServiceInfo[] servicesInfoList)
        {
            var req = new RegisterRequest()
            {
                ServiceInfoList = servicesInfoList,
                ClientInfo = Info
            };

            var resp = await httpClient.PostAsync<RegisterResponse>("Register", req);

            if (resp.Success)
            {
                resp.Signs.ForEach(x => ServicesInfoList.Add(x.Key, x.Value));
            }
            return resp;
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="nodeId"></param>
        public void Unregister(string nodeId)
        {
            ServicesInfoList.Remove(nodeId);
        }

        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<DiscoveryResponse> Discovery(string sign, DiscoveryType type)
        {
            var request = new DiscoveryRequest()
            {
                Sign = sign,
                DiscoveryType = type,
            };

            return await httpClient.PostAsync<DiscoveryResponse>("Discovery", request);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var nodeId in ServicesInfoList.Keys.ToArray())
            {
                Unregister(nodeId);
            }

            (udpClient as IDisposable).Dispose();
            httpClient.Dispose();

            heartbeatTimer.Elapsed -= HeartbeatTimer_Elapsed;
            heartbeatTimer.Stop();
            heartbeatTimer.Dispose();
        }
    }
}
