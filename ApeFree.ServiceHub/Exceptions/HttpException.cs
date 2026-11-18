using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// HTTP 扩展工具类
    /// </summary>
    public static class HttpException
    {
        /// <summary>
        /// 异步发送 POST JSON 请求并解析响应
        /// </summary>
        /// <typeparam name="T">反序列化目标类型</typeparam>
        /// <param name="httpClient">HttpClient实例</param>
        /// <param name="url">请求地址</param>
        /// <param name="request">请求负载</param>
        /// <returns>反序列化后的响应对象</returns>
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string url, object request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            var bytes = await response.Content.ReadAsByteArrayAsync();

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}
