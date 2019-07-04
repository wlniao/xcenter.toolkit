using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class AppMenu
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 菜单链接（支持相对目录）
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// 链接指向的应用
        /// </summary>
        public string appto { get; set; }
        /// <summary>
        /// 上级菜单编码
        /// </summary>
        public string parent { get; set; }
        /// <summary>
        /// 需要验证的权限点
        /// </summary>
        public string authority { get; set; }
        /// <summary>
        /// 菜单排序
        /// </summary>
        public int sort { get; set; }
    }
}