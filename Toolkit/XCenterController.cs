using System;
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
        /// 当前登录的用户Id
        /// </summary>
        protected string _sid = "";
        /// <summary>
        /// 客户统一数字编号
        /// </summary>
        protected string _wkey = "";
        /// <summary>
        /// Wlniao.i接口访问工具
        /// </summary>
        protected xCommon xcommon = null;
        /// <summary>
        /// 页面加载前事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 解析请求Session
            var xhost = GetRequestNoSecurity("xhost");
            if (string.IsNullOrEmpty(xhost))
            {
                xhost = string.IsNullOrEmpty(GetCookies("xhost")) ? xCommon.xHost : GetCookies("xhost");
            }
            else
            {
                Response.Cookies.Append("xhost", xhost);
            }
            xcommon = xCommon.Create(xhost);
            var encrypt = string.IsNullOrEmpty(GetRequest("session")) ? GetCookies("session") : GetRequest("session");
            var session = string.IsNullOrEmpty(encrypt) ? "" : Encryptor.AesDecrypt(encrypt, string.IsNullOrEmpty(xcommon.Token) ? xCommon.xToken : xcommon.Token);
            if (String.IsNullOrEmpty(session) && (xcommon.Register < DateTime.Now || string.IsNullOrEmpty(xcommon.wKey)))
            {
                errorMsg = xcommon.Message;
                var errorPage = new ContentResult();
                errorPage.ContentType = "text/html;charset=utf-8";
                errorPage.Content = errorHtml.Replace("{{errorMsg}}", errorMsg).Replace("{{errorTitle}}", errorTitle).Replace("{{errorIcon}}", errorIcon);
                filterContext.Result = errorPage;
            }
            else
            {
                errorMsg = "您的访问已失效，请重新登录";
                var ht = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Hashtable>(session);
                if (ht != null && ht.ContainsKey("sid") && ht.ContainsKey("wkey"))
                {
                    _sid = ht["sid"].ToString();
                    _wkey = ht["wkey"].ToString();
                    Response.Cookies.Append("session", encrypt);
                    if (!string.IsNullOrEmpty(_sid) && !string.IsNullOrEmpty(_wkey))
                    {
                        errorMsg = "";
                        ViewBag.iHost = string.IsNullOrEmpty(xcommon.Host) ? "" : (xcommon.Host.IndexOf("://") > 0 ? xcommon.Host : "//" + xcommon.Host);
                        base.OnActionExecuting(filterContext);
                    }
                }
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
                var rlt = xcommon.Get<Boolean>("app", "permission"
                    , new KeyValuePair<string, string>("sid", string.IsNullOrEmpty(Sid) ? _sid : Sid)
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
                var rlt = xcommon.Get<Boolean>("app", "permissionorgan"
                    , new KeyValuePair<string, string>("sid", string.IsNullOrEmpty(Sid) ? _sid : Sid)
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