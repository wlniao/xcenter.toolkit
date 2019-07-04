using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Wlniao;

namespace XCenter
{
    /// <summary>
    /// 交互状态
    /// </summary>
    public class xCommon
    {
        /// <summary>
        /// App标识
        /// </summary>
        public static string xApp = Config.GetSetting("xApp");
        /// <summary>
        /// XCenter域名
        /// </summary>
        public static string xHost = Config.GetSetting("xHost");
        /// <summary>
        /// XCenter通讯令牌
        /// </summary>
        public static string xToken = Config.GetSetting("xToken");
        private static Dictionary<String, xCommon> cache = new Dictionary<String, xCommon>();
        private static Dictionary<String, String> hostToken = new Dictionary<String, String>();

        /// <summary>
        /// 根据host生成一个iCommon对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="fromcache"></param>
        /// <returns></returns>
        public static xCommon Create(String host, Boolean fromcache = true)
        {
            if (string.IsNullOrEmpty(host))
            {
                return new xCommon() { Message = "当前访问已失效，请重新打开页面" };
            }
            else
            {
                var https = host.StartsWith("https://");
                host = host.IndexOf("://") > 0 ? host.Substring(host.IndexOf("://") + 3) : host;
                if (host.IndexOf('/') > 0)
                {
                    host = host.Substring(0, host.IndexOf('/'));
                }
                lock (cache)
                {
                    if (!cache.ContainsKey(host))
                    {
                        try
                        {
                            var token = hostToken.ContainsKey(host) ? hostToken[host] : "";
                            cache.Add(host, new xCommon() { Https = https, Host = host, Token = token, Message = "", Register = DateTime.MinValue });
                        }
                        catch { }
                    }
                }
                if (!fromcache || cache[host].Register < DateTime.Now)
                {
                    #region 同步服务器链接
                    if (string.IsNullOrEmpty(xCommon.xApp))
                    {
                        cache[host].Message = "参数xApp未配置，请配置";
                    }
                    else
                    {
                        var rlt = cache[host].Get<String>("app", "check", new KeyValuePair<string, string>("app", xCommon.xApp));
                        if (rlt.success)
                        {
                            cache[host].wKey = rlt.data;
                            cache[host].Message = rlt.message;
                            if (rlt.message == "install")
                            {
                                cache[host].Install = true;
                                cache[host].Register = DateTime.Now.AddMinutes(5);
                                cache[host].Token = rlt.code;
                                SetToken(new KeyValuePair<string, string>(host, rlt.code));
                            }
                            else
                            {
                                cache[host].Install = false;
                                cache[host].Register = DateTime.MinValue;
                                cache[host].Message = "模块未安装，请先安装";
                                SetToken(new KeyValuePair<string, string>(host, cache[host].Token));
                            }
                        }
                        else
                        {
                            if (rlt.message == "token not config")
                            {
                                cache[host].Message = "Token参数未配置，请先配置或注册";
                            }
                            else if (rlt.message == "token error")
                            {
                                cache[host].Message = "Token参数配置错误，请重新配置或注册";
                            }
                            else if (rlt.message == "request is expired")
                            {
                                cache[host].Message = "请求超时，请检查服务器时间是否同步";
                            }
                            else if (rlt.message == "request exception")
                            {
                                cache[host].Message = "XCenter服务器链接失败，请确保服务器已启动并检查您填写的地址是否正确!";
                            }
                            else if (!string.IsNullOrEmpty(rlt.message))
                            {
                                cache[host].Message = rlt.message;
                            }
                            else
                            {
                                cache[host].Message = "XCenter服务器链接失败!";
                            }
                        }
                    }
                    #endregion
                }
                return cache[host];
            }
        }
        /// <summary>
        /// 根据host生成一个iCommon对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static xCommon Create(String host, String token)
        {
            if (string.IsNullOrEmpty(host))
            {
                return null;
            }
            else if (string.IsNullOrEmpty(token))
            {
                return Create(host, false);
            }
            else
            {
                var https = host.StartsWith("https://");
                host = host.IndexOf("://") > 0 ? host.Substring(host.IndexOf("://") + 3) : host;
                if (host.IndexOf('/') > 0)
                {
                    host = host.Substring(0, host.IndexOf('/'));
                }
                if (cache.ContainsKey(host))
                {
                    cache[host].Token = token;
                }
                else
                {
                    cache.Add(host, new xCommon() { Https = https, Host = host, Token = token, Message = "", Register = DateTime.MinValue });
                }
                #region 同步服务器链接
                if (string.IsNullOrEmpty(xCommon.xApp))
                {
                    cache[host].Message = "参数xApp未配置，请配置";
                }
                else
                {
                    var rlt = cache[host].Get<String>("app", "check", new KeyValuePair<string, string>("app", xCommon.xApp));
                    if (rlt.success)
                    {
                        cache[host].wKey = rlt.data;
                        cache[host].Message = rlt.message;
                        if (https)
                        {
                            cache[host].Https = https;
                        }
                        if (rlt.message == "install")
                        {
                            cache[host].Install = true;
                            cache[host].Register = DateTime.Now.AddMinutes(5);
                            cache[host].Token = rlt.code;
                            SetToken(rlt.data, rlt.code);
                        }
                        else
                        {
                            cache[host].Install = false;
                            cache[host].Register = DateTime.MinValue;
                            cache[host].Message = "模块未安装，请先安装";
                            SetToken(rlt.data, token);
                        }
                    }
                    else
                    {
                        if (rlt.message == "request is expired")
                        {
                            cache[host].Message = "请求超时，请检查服务器时间是否同步";
                        }
                        else if (rlt.message == "token not config")
                        {
                            cache[host].Message = "Token参数未配置，请先配置或注册";
                        }
                        else if (rlt.message == "token error")
                        {
                            cache[host].Message = "参数Token配置错误，请重新配置或注册";
                        }
                        else if (rlt.message == "request exception")
                        {
                            cache[host].Message = "XCenter服务器链接失败，请确保服务器已启动并检查您填写的地址是否正确!";
                        }
                        else if (!string.IsNullOrEmpty(rlt.message))
                        {
                            cache[host].Message = rlt.message;
                        }
                        else
                        {
                            cache[host].Message = "XCenter服务器链接失败!";
                        }
                    }
                }
                #endregion
                return cache[host];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void SetToken()
        {
            if (!string.IsNullOrEmpty(xHost) && !string.IsNullOrEmpty(xToken))
            {
                if (hostToken.ContainsKey(xHost))
                {
                    hostToken[xHost] = xToken;
                }
                else
                {
                    hostToken.Add(xHost, xToken);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="token"></param>
        public static void SetToken(string host, string token)
        {
            if (hostToken.ContainsKey(host))
            {
                hostToken[host] = token;
            }
            else
            {
                hostToken.Add(host, token);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host_token"></param>
        public static void SetToken(params KeyValuePair<string, string>[] host_token)
        {
            foreach (var kv in host_token)
            {
                if (hostToken.ContainsKey(kv.Key))
                {
                    hostToken[kv.Key] = kv.Value;
                }
                else
                {
                    hostToken.Add(kv.Key, kv.Value);
                }
            }
        }
        /// <summary>
        /// HTTPS证书验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate
            , System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private static String RequestGet(String url)
        {
            var str = "";
            try
            {
                if (url.StartsWith("https://"))
                {
                    #region HTTPS请求
                    var uri = new Uri(url);
                    var hostSocket = Wlniao.Net.WlnSocket.GetSocket(uri.Host, uri.Port);
                    var reqStr = "";
                    reqStr += "GET " + uri.PathAndQuery + " HTTP/1.1";
                    reqStr += "\r\nHost: " + uri.Host;
                    reqStr += "\r\nDate: " + DateTools.ConvertToGMT(DateTools.GetUnix());
                    reqStr += "\r\nAccept: application/json";
                    reqStr += "\r\n";
                    reqStr += "\r\n";
                    var request = System.Text.Encoding.UTF8.GetBytes(reqStr);
                    using (var ssl = new System.Net.Security.SslStream(new System.Net.Sockets.NetworkStream(hostSocket, true), false, new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null))
                    {
                        ssl.AuthenticateAsClientAsync(uri.Host).ContinueWith((_rlt) =>
                        {
                            if (ssl.IsAuthenticated)
                            {
                                ssl.Write(request);
                                ssl.Flush();
                                var length = 0;
                                var end = false;
                                var start = false;
                                var chunked = false;
                                while (true)
                                {
                                    var rev = new byte[65535];
                                    var index = ssl.Read(rev, 0, rev.Length);
                                    if (index == 0)
                                    {
                                        break;
                                    }
                                    var beffur = new byte[index];
                                    Buffer.BlockCopy(rev, 0, beffur, 0, index);
                                    var tempstr = strUtil.GetUTF8String(beffur);
                                    var lines = tempstr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                                    index = 0;
                                    #region Headers处理
                                    if (!start && lines[0].StartsWith("HTTP"))
                                    {
                                        var ts = lines[0].Split(' ');
                                        if (ts[1] == "200")
                                        {
                                            for (index = 1; index < lines.Length; index++)
                                            {
                                                if (lines[index].ToLower().StartsWith("content-length"))
                                                {
                                                    ts = lines[index].Split(' ');
                                                    length = cvt.ToInt(ts[1]);
                                                }
                                                else if (lines[index].ToLower().StartsWith("transfer-encoding"))
                                                {
                                                    chunked = lines[index].EndsWith("chunked");
                                                }
                                                if (string.IsNullOrEmpty(lines[index]))
                                                {
                                                    index++;
                                                    start = true;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            index = lines.Length;
                                            break;
                                        }
                                    }
                                    #endregion
                                    #region 取文本内容
                                    for (; index < lines.Length; index++)
                                    {
                                        var line = lines[index];
                                        if (chunked)
                                        {
                                            index++;
                                            if (index < lines.Length)
                                            {
                                                var tempLength = cvt.DeHex(line, "0123456789abcdef");
                                                if (tempLength > 0)
                                                {
                                                    length += (int)tempLength;
                                                    line = lines[index];
                                                }
                                                else if (lines.Length == index + 2 && string.IsNullOrEmpty(lines[index + 1]))
                                                {
                                                    end = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        if (index == 0 || (chunked && index == 1) || str.Length == 0)
                                        {
                                            str += line;
                                        }
                                        else
                                        {
                                            str += "\r\n" + line;
                                        }
                                        if (!chunked && System.Text.Encoding.UTF8.GetBytes(str).Length >= length)
                                        {
                                            end = true;
                                        }
                                    }
                                    if (end)
                                    {
                                        break;
                                    }
                                    #endregion
                                }
                            }
                        }).Wait();
                    }
                    hostSocket.Using = false;
                    #endregion
                }
                else
                {
                    #region HTTP请求
                    var response = new System.Net.Http.HttpClient().GetAsync(url).GetAwaiter().GetResult();
                    str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    #endregion
                }
                if (string.IsNullOrEmpty(str))
                {
                    str = "{\"success\":false,\"message\":\"empty response\",\"data\":\"\"}";
                }
                else
                {
                    log.Info(str);
                }
            }
            catch (Exception ex)
            {
                str = "{\"success\":false,\"message\":\"request exception\",\"data\":\"" + ex.Message + "\"}";
            }
            return str;
        }
        private static String RequestPost(String url,String data)
        {
            var str = "";
            try
            {
                str = Wlniao.XServer.Common.PostResponseString(url, data);
                if (string.IsNullOrEmpty(str))
                {
                    str = "{\"success\":false,\"message\":\"empty response\",\"data\":\"\"}";
                }
                else
                {
                    log.Info(str);
                }
            }
            catch (Exception ex)
            {
                str = "{\"success\":false,\"message\":\"request exception\",\"data\":\"" + ex.Message + "\"}";
            }
            return str;
        }
        private static String CreateUrl(bool https, string host, string token, string controller, string action, List<KeyValuePair<String, String>> kvList)
        {
            var url = (https ? "https" : "http") + "://" + host + "/" + controller + "/" + action;
            #region 处理接口基本参数及签名
            if (!string.IsNullOrEmpty(token))
            {
                kvList.Add(new KeyValuePair<String, String>("timespan", DateTools.GetUnix().ToString()));
                kvList = kvList.OrderBy(o => o.Key).ToList();
                var values = new System.Text.StringBuilder();
                foreach (var kv in kvList)
                {
                    if (!string.IsNullOrEmpty(kv.Key))
                    {
                        values.Append(kv.Value);
                    }
                }
                values.Append(token);
                kvList.Add(new KeyValuePair<String, String>("sig", Wlniao.Encryptor.Md5Encryptor32(values.ToString())));
            }
            #endregion
            #region 拼接请求参数
            foreach (var kv in kvList)
            {
                url += url.IndexOf('?') > 0 ? "&" : "?";
                url += kv.Key + "=" + kv.Value;
            }
            #endregion
            log.Debug(url);
            return url;
        }


        /// <summary>
        /// Get请求XCenter服务器
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="kvs"></param>
        /// <returns>服务器返回的泛型实例</returns>
        public String Get(string controller, string action, params KeyValuePair<string, string>[] kvs)
        {
            var rlt = new ApiResult<Object>();
            if (string.IsNullOrEmpty(Token))
            {
                if (!string.IsNullOrEmpty(Host) && hostToken.ContainsKey(Host))
                {
                    Token = hostToken[Host];
                }
                else if (!string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
                {
                    var _rlt = Wlniao.XServer.Common.Get<String>("xcenter", "xserver", "token"
                        , new KeyValuePair<string, string>("wkey", wKey)
                        , new KeyValuePair<string, string>("host", Host));
                    if (_rlt.success)
                    {
                        Token = _rlt.data;
                        SetToken(new KeyValuePair<string, string>(Host, _rlt.data));
                    }
                    else
                    {
                        rlt.message = "token not config";
                        return Newtonsoft.Json.JsonConvert.SerializeObject(rlt);
                    }
                }
            }
            var hasApp = false;
            var list = new List<KeyValuePair<string, string>>();
            foreach (var kv in kvs)
            {
                if (kv.Key == "app")
                {
                    hasApp = true;
                }
                list.Add(kv);
            }
            if (!hasApp)
            {
                list.Add(new KeyValuePair<string, string>("app", xApp));
            }
            if (string.IsNullOrEmpty(Token))
            {
                rlt.message = "token not config";
            }
            else
            {
                var json = RequestGet(CreateUrl(Https, Host, Token, controller, action, list));
                rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<Object>>(json);
                if (rlt.message != "token error")
                {
                    return json;
                }
            }
            if (!string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
            {
                //重新获取最新的ApiToken
                var _rlt = Wlniao.XServer.Common.Get<String>("icenter", "xserver", "token"
                    , new KeyValuePair<string, string>("wkey", wKey)
                    , new KeyValuePair<string, string>("host", Host));
                if (_rlt.success && _rlt.data != Token)
                {
                    Token = _rlt.data;
                    SetToken(Host, Token);
                    return RequestGet(CreateUrl(Https, Host, Token, controller, action, list));
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(rlt);
        }
        /// <summary>
        /// Get请求Wlniao.i服务器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="kvs"></param>
        /// <returns>服务器返回的泛型实例</returns>
        public ApiResult<T> Get<T>(string controller, string action, params KeyValuePair<string, string>[] kvs)
        {
            var rlt = new ApiResult<T>();
            if (string.IsNullOrEmpty(Token))
            {
                if (!string.IsNullOrEmpty(Host) && hostToken.ContainsKey(Host))
                {
                    Token = hostToken[Host];
                }
                else if (!string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
                {
                    var _rlt = Wlniao.XServer.Common.Get<String>("xcenter", "xserver", "token"
                    , new KeyValuePair<string, string>("wkey", wKey)
                    , new KeyValuePair<string, string>("host", Host));
                    if (_rlt.success)
                    {
                        Token = _rlt.data;
                        SetToken(new KeyValuePair<string, string>(Host, _rlt.data));
                    }
                    else
                    {
                        rlt.message = "token not config";
                        return rlt;
                    }
                }
            }
            var hasApp = false;
            var list = new List<KeyValuePair<string, string>>();
            foreach (var kv in kvs)
            {
                if (kv.Key == "app")
                {
                    hasApp = true;
                }
                list.Add(kv);
            }
            if (!hasApp)
            {
                list.Add(new KeyValuePair<string, string>("app", xApp));
            }
            if (string.IsNullOrEmpty(Token))
            {
                rlt.message = "token not config";
            }
            else
            {

                try
                {
                    var json = RequestGet(CreateUrl(Https, Host, Token, controller, action, list));
                    rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                }
                catch
                {
                    return new ApiResult<T>() { success = false, message = "返回的数据格式错误", data = default(T) };
                }
            }
            if (rlt.message == "token error" && !string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
            {
                //重新获取最新的ApiToken
                var _rlt = Wlniao.XServer.Common.Get<String>("xcenter", "xserver", "token"
                    , new KeyValuePair<string, string>("wkey", wKey)
                    , new KeyValuePair<string, string>("host", Host));
                if (_rlt.success && _rlt.data != Token)
                {
                    Token = _rlt.data;
                    SetToken(Host, Token);
                    try
                    {
                        var json = RequestGet(CreateUrl(Https, Host, Token, controller, action, list));
                        rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                    }
                    catch
                    {
                        rlt = new ApiResult<T>() { success = false, message = "返回的数据格式错误", data = default(T) };
                    }
                }
            }
            return rlt;
        }

        /// <summary>
        /// Get请求Wlniao.i服务器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="postdata"></param>
        /// <param name="kvs"></param>
        /// <returns>服务器返回的泛型实例</returns>
        public ApiResult<T> Post<T>(string controller, string action, string postdata, params KeyValuePair<string, string>[] kvs)
        {
            var rlt = new ApiResult<T>();
            if (string.IsNullOrEmpty(Token))
            {
                if (!string.IsNullOrEmpty(Host) && hostToken.ContainsKey(Host))
                {
                    Token = hostToken[Host];
                }
                else if (!string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
                {
                    var _rlt = Wlniao.XServer.Common.Get<String>("xcenter", "xserver", "token"
                    , new KeyValuePair<string, string>("wkey", wKey)
                    , new KeyValuePair<string, string>("host", Host));
                    if (_rlt.success)
                    {
                        Token = _rlt.data;
                        SetToken(Host, Token);
                    }
                    else
                    {
                        rlt.message = "token not config";
                        return rlt;
                    }
                }
            }
            var hasApp = false;
            var list = new List<KeyValuePair<string, string>>();
            foreach (var kv in kvs)
            {
                if (kv.Key == "app")
                {
                    hasApp = true;
                }
                list.Add(kv);
            }
            if (!hasApp)
            {
                list.Add(new KeyValuePair<string, string>("app", xApp));
            }
            if (string.IsNullOrEmpty(Token))
            {
                rlt.message = "token not config";
            }
            else
            {
                try
                {
                    var json = RequestPost(CreateUrl(Https, Host, Token, controller, action, list), postdata);
                    rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                }
                catch
                {
                    return new ApiResult<T>() { success = false, message = "返回的数据格式错误", data = default(T) };
                }
            }
            if (rlt.message == "token error" && !string.IsNullOrEmpty(Wlniao.XServer.Common.AppId))
            {
                //重新获取最新的ApiToken
                var _rlt = Wlniao.XServer.Common.Get<String>("xcenter", "xserver", "token"
                    , new KeyValuePair<string, string>("wkey", wKey)
                    , new KeyValuePair<string, string>("host", Host));
                if (_rlt.success && _rlt.data != Token)
                {
                    Token = _rlt.data;
                    SetToken(new KeyValuePair<string, string>(Host, _rlt.data));
                    try
                    {
                        var json = RequestPost(CreateUrl(Https, Host, Token, controller, action, list), postdata);
                        rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                    }
                    catch
                    {
                        rlt = new ApiResult<T>() { success = false, message = "返回的数据格式错误", data = default(T) };
                    }
                }
            }
            return rlt;
        }


        /// <summary>
        /// 获取App入口地址（已包含ihost）
        /// </summary>
        /// <param name="appcode">Key</param>
        /// <returns></returns>
        public string AppHost(String appcode)
        {
            if (string.IsNullOrEmpty(appcode))
            {
                return "";
            }
            var val = Cache.Get("apphost-" + Host + "-" + appcode);
            if (string.IsNullOrEmpty(val))
            {
                var rlt = Get<String>("app", "apphost", new KeyValuePair<string, string>("appcode", appcode));
                if (rlt.success && !string.IsNullOrEmpty(rlt.data))
                {
                    val = rlt.data;
                }
            }
            return val;
        }
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public string Setting(String key, String value = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return "";
            }
            var val = Cache.Get("setting-" + Host + "-" + key);
            if (string.IsNullOrEmpty(val))
            {
                var rlt = Get<String>("app", "setting", new KeyValuePair<string, string>("key", key), new KeyValuePair<string, string>("value", value));
                if (rlt.success && !string.IsNullOrEmpty(rlt.data))
                {
                    val = rlt.data;
                    Cache.Set("setting-" + Host + "-" + key, val, 180);
                }
                else if (string.IsNullOrEmpty(value))
                {
                    val = "";
                }
                else
                {
                    val = value;
                }
            }
            return val;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">默认值</param>
        /// <returns></returns>
        public ApiResult<String> GetSetting(String key, String value = "")
        {
            return Get<String>("app", "setting", new KeyValuePair<string, string>("key", key), new KeyValuePair<string, string>("value", value));
        }
        /// <summary>
        /// 获取Label值
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="label">默认值</param>
        /// <returns></returns>
        public string GetLabel(String key, String label = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return "";
            }
            var val = Cache.Get("label-" + Host + "-" + key);
            if (string.IsNullOrEmpty(val))
            {
                var rlt = Get<String>("app", "getlabel", new KeyValuePair<string, string>("key", key), new KeyValuePair<string, string>("label", label));
                if (rlt.success && !string.IsNullOrEmpty(rlt.data))
                {
                    val = rlt.data;
                }
                else if (string.IsNullOrEmpty(label))
                {
                    val = key;
                }
                else
                {
                    val = label;
                }
                Cache.Set("label-" + Host + "-" + key, val, 3600);
            }
            return val;
        }

        /// <summary>
        /// 是否支持Https
        /// </summary>
        public bool Https { get; set; }
        /// <summary>
        /// i主机地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 企业用户标识
        /// </summary>
        public string wKey { get; set; }
        /// <summary>
        /// 交互密钥（每次启动时同步）
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// XCenter端消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 下次注册时间
        /// </summary>
        public DateTime Register { get; set; }
        /// <summary>
        /// 是否已经注册
        /// </summary>
        public Boolean Install { get; set; }
    }
}
