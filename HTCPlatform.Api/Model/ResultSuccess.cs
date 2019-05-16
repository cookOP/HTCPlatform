using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HTCPlatform.Api.Model
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ResultSuccess
    {

        /// <summary>
        /// 状态码 0：成功 1：失败
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 对象
        /// </summary>

        public object Data { get; set; } = null;
    }
    /// <summary>
    /// 枚举
    /// </summary>

    public enum Status
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 1,
        /// <summary>
        /// 其他状态
        /// </summary>
        Oterh=101
    }
}
