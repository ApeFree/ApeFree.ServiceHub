using ApeFree.ServiceHub.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ApeFree.ServiceHub.RouteHandler
{
    /// <summary>
    /// 路由处理器接口
    /// </summary>
    public interface IRouteHandler
    {
        /// <summary>
        /// 处理分发的HTTP请求
        /// </summary>
        /// <param name="service">请求分发服务</param>
        /// <param name="context">HTTP上下文</param>
        /// <returns>响应结果</returns>
        BaseResponse RequestHandler(RequestDispatcher service, HttpListenerContext context);
    }
}
