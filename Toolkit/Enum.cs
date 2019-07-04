using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class Enum
    {
        /// <summary>
        /// 值
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 字段说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 获取一个枚举类型
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Enum Get(xCommon icommon, String key)
        {
            if (string.IsNullOrEmpty(key) || icommon == null)
            {
                return null;
            }
            var rlt = icommon.Get<Enum>("app", "getenum", new KeyValuePair<string, string>("key", key));
            if (rlt.success)
            {
                return rlt.data;
            }
            return null;
        }
        /// <summary>
        /// 获取一个枚举类型
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetName(xCommon icommon, String key)
        {
            if (string.IsNullOrEmpty(key) || icommon == null)
            {
                return "";
            }
            var val = Wlniao.Cache.Get("enumname-" + icommon.Host + "-" + key);
            if (string.IsNullOrEmpty(val))
            {
                var rlt = icommon.Get<Enum>("app", "getenum", new KeyValuePair<string, string>("key", key));
                if (rlt.success && !string.IsNullOrEmpty(rlt.data.label))
                {
                    val = rlt.data.label;
                    Wlniao.Cache.Set("enumname-" + icommon.Host + "-" + key, val, 3600);
                }
                else
                {
                    val = key;
                }
            }
            return val;
        }
        /// <summary>
        /// 根据类型获取枚举列表
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<Enum> GetList(xCommon icommon, String parent)
        {
            var rlt = icommon.Get<List<Enum>>("app", "getenumlist", new KeyValuePair<string, string>("parent", parent));
            if (rlt.success && rlt.data != null)
            {
                return rlt.data;
            }
            return new List<Enum>();
        }
    }
}