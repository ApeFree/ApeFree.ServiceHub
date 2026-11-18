using ApeFree.ServiceHub.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ApeFree.ServiceHub.RouteHandler
{
    /// <summary>
    /// 服务注册路由处理器
    /// </summary>
    public class RegisterHandler : IRouteHandler
    {
        /// <inheritdoc/>
        public BaseResponse RequestHandler(RequestDispatcher service, HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var result = new RegisterResponse();
            result.Success = true;
            try
            {
                //接收客户端传过来的数据并转成字符串类型
                var bodyBytes = RequestHandlerHelper.ReadRequestBody(request);
                var d = Encoding.UTF8.GetString(bodyBytes);
                //处理请求
                var registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(Encoding.UTF8.GetString(bodyBytes));
                if (registerRequest != null)
                {
                    registerRequest.IPAddress = request.Url.ToString();
                    result.Signs = service.RegisterRequestHandler?.Invoke(registerRequest);
                }
                else
                {
                }
                return result;
            }
            catch (System.Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
    }
}
