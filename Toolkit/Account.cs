﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class Account
    {
        /// <summary>
        /// 
        /// </summary>
        public string sid { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 企业邮箱
        /// </summary>
        public string orgemail { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string jobnumber { get; set; }
        /// <summary>
        /// 微信绑定Id
        /// </summary>
        public string wxopenid { get; set; }
        /// <summary>
        /// 钉钉绑定Id
        /// </summary>
        public string dinguserid { get; set; }

        /// <summary>
        /// 获取一个账号信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="key"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Account Get(xCommon com, String key, String by = "sid")
        {
            if (string.IsNullOrEmpty(key) || com == null)
            {
                return null;
            }
            var rlt = com.Get<Account>("app", "getaccount", new KeyValuePair<string, string>(by, key));
            if (rlt.success)
            {
                return rlt.data;
            }
            return null;
        }
        /// <summary>
        /// 根据微信OpenId获取
        /// </summary>
        /// <param name="com"></param>
        /// <param name="wxopenid"></param>
        /// <returns></returns>
        public static Account GetByOpenId(xCommon com, String wxopenid)
        {
            return Get(com, wxopenid, "wxopenid");
        }
        /// <summary>
        /// 获取一个账号信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static String GetName(xCommon com, String sid)
        {
            if (string.IsNullOrEmpty(sid) || com == null)
            {
                return "";
            }
            var val = Wlniao.Cache.Get("sidname-" + com.Host + "-" + sid);
            if (string.IsNullOrEmpty(val))
            {
                var rlt = com.Get<Account>("app", "getaccount", new KeyValuePair<string, string>("sid", sid));
                if (rlt.success && !string.IsNullOrEmpty(rlt.data.name))
                {
                    val = rlt.data.name;
                    Wlniao.Cache.Set("sidname-" + com.Host + "-" + sid, val, 3600);
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