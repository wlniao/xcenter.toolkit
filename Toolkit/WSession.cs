using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 统一会话状态
    /// </summary>
    public class WSession
    {
        /// <summary>
        /// 
        /// </summary>
        public string sid = "";
        /// <summary>
        /// 
        /// </summary>
        public string key = "";
        /// <summary>
        /// 
        /// </summary>
        public string wkey = "";
        /// <summary>
        /// 
        /// </summary>
        public string name = "";
        /// <summary>
        /// 
        /// </summary>
        public string account = "";
        /// <summary>
        /// 
        /// </summary>
        public string platform = "";
        /// <summary>
        /// 
        /// </summary>
        public string platformId = "";
        /// <summary>
        /// 
        /// </summary>
        public bool login = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="com"></param>
        /// <param name="key"></param>
        public WSession(xCommon com, String key)
        {
            this.key = key;
            if (com != null && !string.IsNullOrEmpty(key))
            {
                var jsonStr = com.Get("app", "wsession", new KeyValuePair<string, string>("key", key));
                var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonStr);
                if (jsonObj.success.ToString().ToLower() == "true")
                {
                    this.sid = jsonObj.sid;
                    this.wkey = jsonObj.wkey;
                    this.name = jsonObj.name;
                    this.account = jsonObj.account;
                    this.platform = jsonObj.platform;
                    this.platformId = jsonObj.platformId;
                    this.login = true;
                }
            }
        }
    }
}