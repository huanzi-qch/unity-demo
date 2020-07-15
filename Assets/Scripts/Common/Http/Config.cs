using System;
using System.Runtime.CompilerServices;
using BestHTTP;
using LitJson;
using UnityEngine;

namespace Script.Common
{
    /// <summary>
    ///  读取配置文件，把配置文件的信息初始化到类中
    /// </summary>
    public class Config
    {
        public static string serviceUrl; //服务器URL

        public delegate void EventCallBack(ConfigVo configVo);//声明回调事件委托

        public static void InitConfig(EventCallBack callBack = null)
        {
            //PS：如果是打包成WebGL，由于浏览器的同源策略，会存在跨域问题，这一点需要注意
            HTTPRequest httpRequest = new HTTPRequest(
                new Uri(Application.streamingAssetsPath + "/Config.txt"),
                (req, resp, responseheader) =>
                {
                    //Json字符串转对象
//                    ConfigVo configVo = JsonUtility.FromJson<ConfigVo>(resp.DataAsText);
                    ConfigVo configVo = JsonMapper.ToObject<ConfigVo>(resp.DataAsText);
                    Config.serviceUrl = configVo.serviceUrl;
                    Debug.Log("初始化配置成功！");

                    callBack(configVo);
                }
            );
            httpRequest.MethodType=HTTPMethods.Get;
            
            //post请求，如需设置请求参数
//            httpRequest.MethodType=HTTPMethods.Post;
//            httpRequest.AddField("key","value");

            httpRequest.Send();
        }
    }
}