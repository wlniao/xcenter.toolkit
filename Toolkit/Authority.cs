using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 权限点
    /// </summary>
    public class Authority
    {
        /// <summary>
        /// 权限类型 0、系统权限；1、App权限；2、机构权限
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 权限点
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 权限说明
        /// </summary>
        public string description { get; set; }
    }
}