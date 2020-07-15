using System;

namespace Script.Common
{
    [System.Serializable]
    public class ConfigVo
    {
        public String name { get; set; }//项目名称
        public String describe { get; set; }//项目详情
        public String author{ get; set; }//作者
        public String blogs{ get; set; }//博客
        public String github{ get; set; }//GitHub
        public String gitee{ get; set; }//gitee
        public String QQqun{ get; set; }//QQ交流群
        public String serviceUrl{ get; set; }//服务器地址，如果数据需要从后端服务获取
    }
}