using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 下属机构
    /// </summary>
    public class Organ
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 全称
        /// </summary>
        public string fullname { get; set; }
        /// <summary>
        /// 机构类型 OrganType
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// 获取一个枚举类型
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Organ Get(xCommon icommon, String id)
        {
            if (string.IsNullOrEmpty(id) || icommon == null)
            {
                return null;
            }
            var rlt = icommon.Get<Organ>("app", "getorgan", new KeyValuePair<string, string>("id", id));
            if (rlt.success)
            {
                return rlt.data;
            }
            return null;
        }
        /// <summary>
        /// 获取一个机构信息
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static String GetName(xCommon icommon, String id)
        {
            if (string.IsNullOrEmpty(id) || icommon == null)
            {
                return "";
            }
            var rlt = icommon.Get<Organ>("app", "getorgan", new KeyValuePair<string, string>("id", id));
            if (rlt.success)
            {
                return rlt.data.name;
            }
            return "";
        }
        /// <summary>
        /// 获取一个机构信息
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static String GetFullName(xCommon icommon, String id)
        {
            if (string.IsNullOrEmpty(id) || icommon == null)
            {
                return "";
            }
            var rlt = icommon.Get<Organ>("app", "getorgan", new KeyValuePair<string, string>("id", id));
            if (rlt.success)
            {
                if (string.IsNullOrEmpty(rlt.data.fullname))
                {
                    return rlt.data.name;
                }
                return rlt.data.fullname;
            }
            return "";
        }
        /// <summary>
        /// 获取机构列表
        /// </summary>
        /// <param name="icommon"></param>
        /// <returns></returns>
        public static List<Organ> GetList(xCommon icommon)
        {
            var rlt = icommon.Get<List<Organ>>("app", "getorganlist");
            if (rlt.success && rlt.data != null)
            {
                return rlt.data;
            }
            return new List<Organ>();
        }
        /// <summary>
        /// 根据用户权限获取机构列表
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static List<Organ> GetList(xCommon icommon,String sid)
        {
            var rlt = icommon.Get<List<Organ>>("app", "getorganlist", new KeyValuePair<string, string>("sid", sid));
            if (rlt.success && rlt.data != null)
            {
                return rlt.data;
            }
            return new List<Organ>();
        }
    }
}