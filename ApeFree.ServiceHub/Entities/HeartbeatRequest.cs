using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 心跳请求数据包
    /// </summary>
    public class HeartbeatRequest
    {
        //包格式
        //固定头（1byte）+版本号（1byte）+包长度（4byte）+数据（x byte）
        /// <summary>
        /// 保持心跳的服务ID集合
        /// </summary>
        public string[] ServiceInfoIds { get; set; }

        /// <summary>
        /// 客户端信息
        /// </summary>
        public ClientInfo ClientInfo { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        [JsonIgnore]
        public string IPAddress { get; set; }

        /// <summary>
        /// 构造心跳请求
        /// </summary>
        public HeartbeatRequest()
        {

        }

        /// <summary>
        /// 从原始字节反序列化构造心跳请求
        /// </summary>
        /// <param name="bytes">原始二进制数据</param>
        public HeartbeatRequest(byte[] bytes)
        {
            var jsonString = Encoding.UTF8.GetString(bytes);
            var request = JsonConvert.DeserializeObject<HeartbeatRequest>(jsonString);

            this.ServiceInfoIds = request.ServiceInfoIds;
            this.ClientInfo = request.ClientInfo;
        }

        /// <summary>
        /// 构造心跳请求
        /// </summary>
        /// <param name="info">客户端信息</param>
        /// <param name="nodeIds">服务ID集合</param>
        public HeartbeatRequest(ClientInfo info, string[] nodeIds)
        {
            ServiceInfoIds = nodeIds;
            ClientInfo = info;
        }

        /// <summary>
        /// 序列化为网络传输二进制数据
        /// </summary>
        /// <returns>字节数组</returns>
        public byte[] GetBytes()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var byteArr = Encoding.UTF8.GetBytes(jsonString);

            var bytes = new List<byte>();
            //    bytes.AddRange(BitConverter.GetBytes(byteArr.Length + 4));
            bytes.AddRange(byteArr);
            return bytes.ToArray();
        }
    }
}
