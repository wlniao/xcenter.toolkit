﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XCenter
{
    /// <summary>
    /// 钉钉业务功能
    /// </summary>
    public class Dingtalk
    {
        /// <summary>
        /// 表单信息
        /// </summary>
        public class FormItemVo
        {
            /// <summary>
            /// 表单标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 表单内容
            /// </summary>
            public string content { get; set; }
        }


        /// <summary>
        /// 获取钉钉AccessToken
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static Wlniao.ApiResult<String> GetToken(xCommon com)
        {
            if (com == null)
            {
                return null;
            }
            return com.Get<String>("app", "dingtalk_token");
        }
        /// <summary>
        /// 钉钉待办任务推送
        /// </summary>
        /// <param name="com"></param>
        /// <param name="to">Sid或手机号</param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="exdata"></param>
        /// <returns></returns>
        public static Wlniao.ApiResult<String> WorkrecordAdd(xCommon com, String to, String title, String url, List<FormItemVo> exdata)
        {
            if (string.IsNullOrEmpty(to) || com == null)
            {
                return null;
            }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                by = Wlniao.strUtil.IsMobile(to) ? "mobile" : "sid",
                exdata = Newtonsoft.Json.JsonConvert.SerializeObject(exdata),
                mobile = to,
                sid = to,
                title,
                url
            });
            return com.Post<String>("app", "dingtalk_workrecord_add", json);
        }
        /// <summary>
        /// 钉钉待办任务更新
        /// </summary>
        /// <param name="com"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public static Wlniao.ApiResult<String> WorkrecordUpdate(xCommon com, String record)
        {
            if (string.IsNullOrEmpty(record) || com == null)
            {
                return null;
            }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { record });
            return com.Post<String>("app", "dingtalk_workrecord_update", json);
        }
        /// <summary>
        /// 钉钉待办任务更新
        /// </summary>
        /// <param name="com"></param>
        /// <param name="userid"></param>
        /// <param name="record_id"></param>
        /// <returns></returns>
        public static Wlniao.ApiResult<String> WorkrecordUpdate(xCommon com, String userid, String record_id)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(record_id) || com == null)
            {
                return null;
            }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { userid, record_id });
            return com.Post<String>("app", "dingtalk_workrecord_update", json);
        }

    }
}