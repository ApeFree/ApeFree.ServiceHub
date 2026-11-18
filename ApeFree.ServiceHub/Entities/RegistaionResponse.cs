using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 基础响应
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
