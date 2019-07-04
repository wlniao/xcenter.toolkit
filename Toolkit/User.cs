using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string sid { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 用户名（手机号码）
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 微信绑定Id
        /// </summary>
        public string wxopenid { get; set; }
        /// <summary>
        /// 企业微信Id
        /// </summary>
        public string wxuserid { get; set; }
        /// <summary>
        /// 钉钉绑定Id
        /// </summary>
        public string dinguserid { get; set; }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="icommon"></param>
        /// <param name="mobile"></param>
        /// <param name="pwdcode"></param>
        /// <param name="wxopenid"></param>
        /// <returns></returns>
        public static Wlniao.ApiResult<String> Login(xCommon icommon, String mobile, String pwdcode, String wxopenid = "")
        {
            return icommon.Get<String>("app", "login"
                , new KeyValuePair<string, string>("mobile", mobile)
                , new KeyValuePair<string, string>("pwdcode", pwdcode)
                , new KeyValuePair<string, string>("wxopenid", wxopenid));
        }
        /// <summary>
        /// 获取一个用户信息
        /// </summary>
        /// <param name="xcommon"></param>
        /// <param name="key"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static User Get(xCommon xcommon, String key, String by = "sid")
        {
            if (string.IsNullOrEmpty(key) || xcommon == null)
            {
                return null;
            }
            var rlt = xcommon.Get<User>("app", "getaccount", new KeyValuePair<string, string>(by, key));
            if (rlt.success)
            {
                return rlt.data;
            }
            return null;
        }
        /// <summary>
        /// 根据手机号获取
        /// </summary>
        /// <param name="xcommon"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static User GetByMobile(xCommon xcommon, String mobile)
        {
            return Get(xcommon, mobile, "mobile");
        }
        /// <summary>
        /// 根据微信OpenId获取
        /// </summary>
        /// <param name="xcommon"></param>
        /// <param name="wxopenid"></param>
        /// <returns></returns>
        public static User GetByOpenId(xCommon xcommon, String wxopenid)
        {
            return Get(xcommon, wxopenid, "wxopenid");
        }
        /// <summary>
        /// 获取用户账号
        /// </summary>
        /// <param name="xcommon"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static String GetUid(xCommon xcommon, String sid)
        {
            if (string.IsNullOrEmpty(sid) || xcommon == null)
            {
                return "";
            }
            var user = Get(xcommon, sid, "sid");
            return user == null ? "" : user.account;
        }
        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <param name="xcommon"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static String GetName(xCommon xcommon, String sid)
        {
            if (string.IsNullOrEmpty(sid) || xcommon == null)
            {
                return "";
            }
            var val = Wlniao.Cache.Get("username-" + xcommon.Host + "-" + sid);
            if (string.IsNullOrEmpty(val))
            {
                var user = Get(xcommon, sid, "sid");
                if (user != null && !string.IsNullOrEmpty(user.name))
                {
                    val = user.name;
                    Wlniao.Cache.Set("username-" + xcommon.Host + "-" + sid, val, 3600);
                }
                else
                {
                    val = "";
                }
            }
            return val;
        }

    }
}