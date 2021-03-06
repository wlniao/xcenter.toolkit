﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wlniao;
using Wlniao.XServer;

namespace XCenter
{
    /// <summary>
    /// Wlniao.i基础Controller
    /// </summary>
    public partial class XCenterController : XCoreController
    {
        /// <summary>
        /// 主接口访问工具
        /// </summary>
        protected xCommon com = null;
        /// <summary>
        /// 主平台登录会话状态
        /// </summary>
        protected WSession wsession = null;
        /// <summary>
        /// 页面加载前事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 解析请求Session
            var key = "";
            var xhost = GetCookies("xhost");
            if (Request.Query.Keys.Contains("xhost"))
            {
                xhost = GetRequestNoSecurity("xhost");
                Response.Cookies.Append("xhost", xhost);
            }
            com = xCommon.Create(xhost);
            if (Request.Query.Keys.Contains("wsession"))
            {
                key = GetRequestNoSecurity("wsession");
                Response.Cookies.Append("wsession", key);
            }
            else
            {
                key = GetCookies("wsession");
            }
            wsession = new WSession(com, key);
            if (wsession.login)
            {
                ViewBag.iHost = string.IsNullOrEmpty(xhost) ? "" : (xhost.IndexOf("://") > 0 ? xhost : "//" + xhost);
                base.OnActionExecuting(filterContext);
            }
            else
            {
                errorMsg = "您的访问已失效，请重新登录";
                if (com != null && !string.IsNullOrEmpty(com.Message))
                {
                    errorMsg = com.Message;
                }
                var errorPage = new ContentResult();
                errorPage.ContentType = "text/html;charset=utf-8";
                errorPage.Content = errorHtml.Replace("{{errorMsg}}", errorMsg).Replace("{{errorTitle}}", errorTitle).Replace("{{errorIcon}}", errorIcon);
                filterContext.Result = errorPage;
            }
            #endregion
        }
        /// <summary>
        /// 检查用户系统权限权限
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Sid"></param>
        /// <returns></returns>
        [NonAction]
        public bool Permission(String Code, String Sid = "")
        {
            if (!string.IsNullOrEmpty(Code))
            {
                var rlt = com.Get<Boolean>("app", "permission"
                    , new KeyValuePair<string, string>("sid", string.IsNullOrEmpty(Sid) ? wsession.sid : Sid)
                    , new KeyValuePair<string, string>("code", Code));
                if (rlt.data)
                {
                    return rlt.data;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查用户系统权限权限
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Sid"></param>
        /// <returns></returns>
        [NonAction]
        public bool CheckPermission(String Code, String Sid = "")
        {
            if (Permission(Code, Sid))
            {
                return true;
            }
            errorMsg = "您暂无执行当前操作的权限";
            return false;
        }
        /// <summary>
        /// 检查机构数据查看权限
        /// </summary>
        /// <param name="Organ"></param>
        /// <param name="Code"></param>
        /// <param name="Sid"></param>
        /// <returns></returns>
        [NonAction]
        public bool PermissionOrgan(String Organ, String Code, String Sid = "")
        {
            if (!string.IsNullOrEmpty(Code))
            {
                var rlt = com.Get<Boolean>("app", "permissionorgan"
                    , new KeyValuePair<string, string>("sid", string.IsNullOrEmpty(Sid) ? wsession.sid : Sid)
                    , new KeyValuePair<string, string>("code", Code)
                    , new KeyValuePair<string, string>("organ", Organ));
                if (rlt.data)
                {
                    return rlt.data;
                }
                errorMsg = "您暂无执行当前操作的权限";
            }
            return false;
        }
        /// <summary>
        /// 返回无权限提示
        /// </summary>
        /// <param name="ajax"></param>
        /// <returns></returns>
        public ActionResult NoPermission(Boolean ajax = false)
        {
            if (ajax || !string.IsNullOrEmpty(method))
            {
                errorMsg = "";
                return Json(new { success = false, message = "您暂无执行当前操作的权限" });
            }
            else
            {
                var errorPage = new ContentResult();
                errorPage.ContentType = "text/html;charset=utf-8";
                errorPage.Content = errorHtml.Replace("{{errorMsg}}", errorMsg).Replace("{{errorTitle}}", errorTitle).Replace("{{errorIcon}}", errorIcon);
                return errorPage;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult enums()
        //{
        //    var enums = XCenter.Enum.GetList(xcommon, method);
        //    return Json(enums.Select(e => new
        //    {
        //        value = e.key,
        //        label = e.label
        //    }).ToList());
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult organ()
        //{
        //    if (method == "list")
        //    {
        //        var data = Organ.GetList(xcommon, _sid);
        //        if (data.Count == 1)
        //        {
        //            _organ = data[0].id;
        //        }
        //        else if (data.Count == 0)
        //        {
        //            _organ = "";
        //        }
        //        return Json(new { data = data, select = string.IsNullOrEmpty(_organ) ? "" : _organ });
        //    }
        //    else
        //    {
        //        //保存当前所选定的要管理的校园
        //        Response.Cookies.Append("select_organ", method);
        //        return JsonStr("");
        //    }
        //}
        ///// <summary>
        ///// 权限检查 参数 organ:所属范围 code:权限代码
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult auth()
        //{
        //    var code = GetRequest("code");
        //    var organ = GetRequest("organ");
        //    if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(organ))
        //    {
        //        return Json(new { success = false, message = "未指定权限范围" });
        //    }
        //    else
        //    {
        //        var rlt = false;
        //        if (string.IsNullOrEmpty(organ))
        //        {
        //            rlt = Permission(code, _sid);
        //        }
        //        else
        //        {
        //            rlt = PermissionOrgan(organ, code, _sid);
        //        }
        //        if (!rlt)
        //        {
        //            errorMsg = "";
        //        }
        //        return Json(new { success = rlt });
        //    }
        //}
    }
}