using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ApeFree.ServiceHub
{
    /// <summary>
    /// UDP心跳监听器
    /// </summary>
    public class UdpHeartbeatListener
    {
        /// <summary>
        /// UDP客户端
        /// </summary>
        private UdpClient udpClient;

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 心跳处理程序
        /// </summary>
        public EventHandler<byte[]> HeartbeatHandler;

        public UdpHeartbeatListener(int port)
        {
            Port = port;
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            udpClient = new UdpClient(RemoteIpEndPoint);
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        public void Start()
        {
            udpClient.BeginReceive(EndReceive, udpClient);

            void EndReceive(IAsyncResult ar)
            {
                var client = ar.AsyncState as UdpClient;
                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedBytes = client.EndReceive(ar, ref remoteEP); // 获取接收到的数据

                HeartbeatHandler?.Invoke(this, receivedBytes);
                client.BeginReceive(EndReceive, client);
            }
        }
    }
}
