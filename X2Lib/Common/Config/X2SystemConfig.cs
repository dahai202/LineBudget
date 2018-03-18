using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using X2Lib.X2Sys;

namespace X2Lib.Common.Config
{
    [DataContract]
    public class X2SystemConfig
    {
        [DataContract]
        public class X2CfgModel
        {
            #region 字段
            private string _key;
            private string _value;
            #endregion

            #region 属性
            [DataMember]
            public string key
            {
                get { return _key; }
                set { _key = value; }
            }

            [DataMember]
            public string value
            {
                get { return _value; }
                set { _value = value; }
            }
            #endregion

            #region 公共方法
            public static List<X2CfgModel> ConvertToModel(Dictionary<string, string> keyValues)
            {
                List<X2CfgModel> cfgModels = new List<X2CfgModel>();
                if (ConfigurationManager.AppSettings.Count > 0)
                {
                    foreach (var item in keyValues)
                    {
                        if (item.Key == "FullTextSearchConnectionString")
                        {
                            continue;
                        }
                        cfgModels.Add(new X2CfgModel() { key = item.Key, value = item.Value });
                    }
                }
                return cfgModels;
            }

            public static Dictionary<string, string> ConvertToKeyValues(List<X2CfgModel> cfgModels)
            {
                Dictionary<string, string> dics = new Dictionary<string, string>();

                if (cfgModels != null && cfgModels.Count > 0)
                {
                    foreach (var item in cfgModels)
                    {
                        if (dics.ContainsKey(item.key)) continue;
                        if (item.key == "FullTextSearchConnectionString")
                        {
                            continue;
                        }
                        dics.Add(item.key, item.value);
                    }
                }
                return dics;

            }
            #endregion
        }

        #region 公共方法

        /// <summary>
        /// 初始化旧的配置文件为新的配置模式
        /// </summary>
        public static void InitOldConfig()
        {
            string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "settings.config");

            if (X2Lib.IO.X2File.IsExist(configPath))
            {
                return;
            }
            else
            {
                try
                {
                    string xmlContent = @"<appSettings></appSettings>";
                    X2Lib.IO.X2File.SaveFieContent(configPath, xmlContent);

                    Dictionary<string, string> defalultKeyValues = new Dictionary<string, string>();
                    defalultKeyValues.Add("X2PlatformVersion", GetAppSetting("X2PlatformVersion") ?? "2.8.2.1");
                    defalultKeyValues.Add("X2LANGUAGE", GetAppSetting("X2PlatformVersion") ?? "en");
                    defalultKeyValues.Add("X2DefaultLayout", GetAppSetting("X2DefaultLayout") ?? "autoId");
                    defalultKeyValues.Add("AppTitle", GetAppSetting("AppTitle") ?? "DRAKEX2_HZDEV");
                    defalultKeyValues.Add("X2VistorAccount", GetAppSetting("X2VistorAccount") ?? "test2;admin");
                    defalultKeyValues.Add("UserTag", GetAppSetting("UserTag") ?? "12345");
                    defalultKeyValues.Add("X2SearchSet", GetAppSetting("X2SearchSet") ?? "true;true");
                    defalultKeyValues.Add("SMSTemplate", GetAppSetting("SMSTemplate") ?? "{0}您好,有{1}条{2},请您处理");
                    defalultKeyValues.Add("X2HOSTPASS", GetAppSetting("X2HOSTPASS") ?? "123");
                    defalultKeyValues.Add("VisitMode", GetAppSetting("VisitMode") ?? "VISITMODE;test2");

                    string todoConfig = GetAppSetting("SendSmsType");
                    string[] accountList = todoConfig.Split(';');

                    if (!string.IsNullOrEmpty(todoConfig) && accountList.Length == 3)
                    {
                        defalultKeyValues.Add("ToDoSmsIsOpen", accountList[0]);
                        defalultKeyValues.Add("ToDoMailIsOpen", accountList[1]);
                        defalultKeyValues.Add("ToDoRemindFrequency", accountList[2]);
                    }
                    else
                    {
                        defalultKeyValues.Add("ToDoSmsIsOpen", GetAppSetting("ToDoSmsIsOpen") ?? "false");
                        defalultKeyValues.Add("ToDoMailIsOpen", GetAppSetting("ToDoMailIsOpen") ?? "false");
                        defalultKeyValues.Add("ToDoRemindFrequency", GetAppSetting("ToDoRemindFrequency") ?? "60");
                    }

                    defalultKeyValues.Add("X2SystemAccount", GetAppSetting("X2SystemAccount") ?? "");
                    defalultKeyValues.Add("X2SystemSmtpHost", GetAppSetting("X2SystemSmtpHost") ?? "");
                    defalultKeyValues.Add("SmsAccount", GetAppSetting("SmsAccount") ?? "");
                    defalultKeyValues.Add("SmsGatewayUrl", GetAppSetting("SmsGatewayUrl") ?? "");
                    defalultKeyValues.Add("SmsServerName", GetAppSetting("SmsServerName") ?? "");

                    #region 删除
                    //foreach (string item in ConfigurationManager.AppSettings)
                    //{
                    //    if (item == "FullTextSearchConnectionString")
                    //    {
                    //        continue;
                    //    }

                    //    switch (item)
                    //    {
                    //        case "X2PlatformVersion":
                    //            if (defalultKeyValues.ContainsKey("X2PlatformVersion"))
                    //            {
                    //                defalultKeyValues["X2PlatformVersion"] = ConfigurationManager.AppSettings[item];
                    //            }
                    //            else
                    //            {
                    //                defalultKeyValues.Add("X2PlatformVersion", ConfigurationManager.AppSettings[item]);
                    //            }
                    //            break;

                    //        case "X2LANGUAGE":
                    //            defalultKeyValues.Add("X2LANGUAGE", ConfigurationManager.AppSettings[item]);
                    //            break;

                    //        case "X2DefaultLayout":
                    //            defalultKeyValues.Add("X2DefaultLayout", ConfigurationManager.AppSettings[item]);
                    //            break;

                    //        default:
                    //            break;
                    //    }
                    //} 
                    #endregion

                    SetValues(defalultKeyValues);
                    UpdateConfig(defalultKeyValues);
                }
                catch (Exception ex)
                {
                    X2Error.Log("Error__InitOldConfig", ex, "InitOldConfig");
                }
            }

        }

        /// <summary>
        /// 更新webconfig配置文件信息
        /// </summary>
        /// <param name="delKeys">值</param>
        private static void UpdateConfig(Dictionary<string, string> delKeys)
        {
            string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "web.config");
            string newConfig = string.Format("{0}\\{1}__{2}", System.AppDomain.CurrentDomain.BaseDirectory, "web.config", DateTime.Now.Ticks);
            if (!X2Lib.IO.X2File.IsExist(newConfig))
            {
                X2Lib.IO.X2File.CopyFile(newConfig, configPath);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(newConfig);
            XmlNode node = doc.SelectSingleNode(@"//configuration/appSettings");
            XmlElement ele = (XmlElement)node;
            ele.SetAttribute("file", "settings.config");

            for (int i = 0; i < node.ChildNodes.Count; i++)
			{
                if (node.ChildNodes[i] == null
                    || node.ChildNodes[i].Attributes == null
                    || node.ChildNodes[i].Attributes.Count == 0
                    || node.ChildNodes[i].Attributes["key"] == null
                    || string.IsNullOrEmpty(node.ChildNodes[i].Attributes["key"].Value))
                {
                    continue;
                }
                if (delKeys.ContainsKey(node.ChildNodes[i].Attributes["key"].Value.Trim())
                    || node.ChildNodes[i].Attributes["key"].Value.Trim() == "SendSmsType"
                    )
                {
                    node.RemoveChild(node.ChildNodes[i]);
                    continue;
                }
            }

            doc.Save(newConfig);

            X2Lib.IO.X2File.SaveFieContent(configPath, X2Lib.IO.X2File.ReadFile(newConfig));
            X2Lib.IO.X2File.DeleteFile(newConfig);

        }

        /// <summary>
        /// 修改webconfig
        /// </summary>
        /// <param name="delKeys"></param>
        /// <returns></returns>
        private static bool InitWebconfig(Dictionary<string, string> delKeys)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
            foreach (var item in delKeys)
            {
                if (ExistsKey(item.Key))
                {
                    config.AppSettings.Settings.Remove(item.Key);
                }
            }
            config.AppSettings.SectionInformation.ForceSave = true;//强制保存
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");//刷新AppSettings
            return true;
        }

        /// <summary>
        /// 修改系统级配置
        /// </summary>
        /// <param name="configs"></param>
        public static void AddOrUpdateConfig(Dictionary<string, string> configs)
        {
            if (configs != null && configs.Count > 0)
            {
                UpdateAppSettings(configs);
            }
        }

        /// <summary>
        /// 获取AppSetting项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null || value == string.Empty)
                return null;

            return value;
        }

        /// <summary>
        /// 获取AppSetting项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<X2CfgModel> GetAppSettingByKeys(Dictionary<string, string> keys)
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            if (ConfigurationManager.AppSettings.Count > 0)
            {
                foreach (string item in ConfigurationManager.AppSettings)
                {
                    if (item == "FullTextSearchConnectionString" || !keys.ContainsKey(item))
                    {
                        continue;
                    }
                    dics.Add(item, ConfigurationManager.AppSettings[item]);
                }
            }
            return X2CfgModel.ConvertToModel(dics);
        }

        /// <summary>
        /// 获取系统级配置
        /// </summary>
        /// <returns></returns>
        public static List<X2CfgModel> GetAllConfigs()
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            if (ConfigurationManager.AppSettings.Count > 0)
            {
                foreach (string item in ConfigurationManager.AppSettings)
                {
                    if (item == "FullTextSearchConnectionString")
                    {
                        continue;
                    }
                    dics.Add(item, ConfigurationManager.AppSettings[item]);
                }
            }
            return X2CfgModel.ConvertToModel(dics);
        }

        #endregion

        #region 配置文件修改
        /// <summary>
        /// 更新配置文件信息
        /// </summary>
        /// <param name="name">配置文件字段名称</param>
        /// <param name="value">值</param>
        private static void UpdateConfig(string name, string value)
        {
            XmlDocument doc = new XmlDocument();
            string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "settings.config");
            doc.Load(configPath);
            XmlNode node = doc.SelectSingleNode(@"//add[@key='" + name + "']");
            XmlElement ele = (XmlElement)node;
            ele.SetAttribute("value", value);
            doc.Save(configPath);
        }

        ///<summary>  
        ///向.config文件的appKey结写入信息AppValue保存设置  
        ///</summary>  
        ///<param name="appKey">节点名</param>  
        ///<param name="appValue">值</param>
        private static void SetValue(string appKey, string appValue)
        {
            XmlDocument xDoc = new XmlDocument();
            string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "settings.config");

            xDoc.Load(configPath);
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
            if (xElem1 != null)
                xElem1.SetAttribute("value", appValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", appKey);
                xElem2.SetAttribute("value", appValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(configPath);
        }

        ///<summary>  
        ///向.config文件的appKey结写入信息AppValue保存设置  
        ///</summary>  
        ///<param name="keyValues">值</param>
        private static void SetValues(Dictionary<string, string> keyValues)
        {
            XmlDocument xDoc = new XmlDocument();
            string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "settings.config");
            xDoc.Load(configPath);
            XmlNode xNode = xDoc.SelectSingleNode("//appSettings");

            foreach (var item in keyValues)
            {
                XmlElement xElem1;
                XmlElement xElem2;

                xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + item.Key + "']");
                if (xElem1 != null)
                    xElem1.SetAttribute("value", item.Value);
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("key", item.Key);
                    xElem2.SetAttribute("value", item.Value);
                    xNode.AppendChild(xElem2);
                }
            }
            xDoc.Save(configPath);
        }

        /// <summary>
        /// 更新AppSetting项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateAppSettings(string key, string value)
        {
            //如果已经有此key存在，更新
            if (ExistsKey(key))
            {
                ConfigurationManager.AppSettings[key] = value;
            }
            else
            {
                ConfigurationManager.AppSettings.Add(key, value);
            }

            SetValue(key, value);
            ConfigurationManager.RefreshSection("appSettings");//刷新AppSettings

            return true;
        }

        /// <summary>
        /// 更新AppSetting项
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static bool UpdateAppSettings(Dictionary<string, string> keyValues)
        {
            foreach (var item in keyValues)
            {
                if (item.Key == "FullTextSearchConnectionString")
                {
                    continue;
                }

                //如果已经有此key存在，更新
                if (ExistsKey(item.Key))
                {
                    ConfigurationManager.AppSettings[item.Key] = item.Value;
                }
                else
                {
                    ConfigurationManager.AppSettings.Add(item.Key, item.Value);
                }
            }

            SetValues(keyValues);
            ConfigurationManager.RefreshSection("appSettings");//刷新AppSettings

            return true;
        }

        /// <summary>
        /// 获取一个值，改值指示配置文件的AppSettings配置节中是否含有和参数“key”同名的键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool ExistsKey(string key)
        {
            bool exist = false;
            for (int i = 0; i < ConfigurationManager.AppSettings.Keys.Count; i++)
            {
                if (ConfigurationManager.AppSettings.Keys[i] == key)
                {
                    exist = true;
                }
            }
            return exist;
        }
        #endregion

        #region 暂时删除

        #region 字段
        private string _defaultLanguage;
        private string _defaultLayout;

        private string _websiteTitle;
        private string _todoSmsTemplate;
        private string _todoRemindFrequency;
        private string _platformVersion;
        #endregion

        #region 属性

        /// <summary>
        /// 默认语言
        /// </summary>
        [DataMember]
        public string defaultLanguage
        {
            get { return _defaultLanguage; }
            set { _defaultLanguage = value; }
        }

        /// <summary>
        /// 默认布局
        /// </summary>
        public string defaultLayout
        {
            get { return _defaultLayout; }
            set { _defaultLayout = value; }
        }

        /// <summary>
        /// 网站标题
        /// </summary>
        public string websiteTitle
        {
            get { return _websiteTitle; }
            set { _websiteTitle = value; }
        }

        /// <summary>
        /// 代办短信模板
        /// </summary>
        public string todoSmsTemplate
        {
            get { return _todoSmsTemplate; }
            set { _todoSmsTemplate = value; }
        }

        /// <summary>
        /// 代办提醒频率
        /// </summary>
        public string todoRemindFrequency
        {
            get { return _todoRemindFrequency; }
            set { _todoRemindFrequency = value; }
        }

        /// <summary>
        /// 平台版本
        /// </summary>
        public string platformVersion
        {
            get { return _platformVersion; }
            set { _platformVersion = value; }
        }

        #endregion

        /////<summary>
        /////在＊.exe.config文件中appSettings配置节增加一对键、值对
        /////</summary>
        /////<param name="newKey"></param>
        /////<param name="newValue"></param>
        //private static void UpdateAppConfig(string key, string newValue, X2Env currentUser)
        //{
        //    string configPath = string.Format("{0}{1}", currentUser.CurrentUser.workingServerMapPath, "settings.config");

        //    if (ConfigurationManager.AppSettings[key] != null)
        //    {
        //        ConfigurationManager.AppSettings[key] = newValue;
        //    }
        //    else
        //    {
        //        ConfigurationManager.AppSettings.Add(key, newValue);
        //    }

        //    //重新加载setting配置
        //    ConfigurationManager.RefreshSection("appSettings");
        //}

        ///<summary>
        ///返回＊.exe.config文件中appSettings配置节的value项
        ///</summary>
        ///<param name="strKey"></param>
        ///<returns></returns>
        private static string GetAppConfig(string strKey)
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == strKey)
                {
                    return ConfigurationManager.AppSettings[strKey];
                }
            }
            return null;
        }

        /// <summary>
        /// 配置通用方法
        /// </summary>
        private class CfgCommon
        {
            #region PublicMethod

            private static string configPath = string.Format("{0}\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, "settings.config");
            //获取AppSetting项
            public static string GetAppSetting(string key)
            {
                string value = ConfigurationManager.AppSettings[key];
                if (value == null || value == string.Empty)
                    return null;

                return value;
            }

            //添加AppSetting项
            public static bool AddAppSettings(string key, string value)
            {
                if (!ExistsKey(key))
                {
                    // Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    Configuration config = WebConfigurationManager.OpenWebConfiguration(configPath);
                    config.AppSettings.Settings.Add(key, value);
                    SaveAppSettings(config);
                    return true;
                }
                return false;
            }

            //更新AppSetting项
            public static bool UpdateAppSettings(string key, string value)
            {
                //如果已经有此key存在，更新
                if (ExistsKey(key))
                {
                    //  Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    Configuration config = WebConfigurationManager.OpenWebConfiguration(configPath);

                    config.AppSettings.Settings[key].Value = value;
                    SaveAppSettings(config);
                }
                return false;
            }

            /// 获取一个值，改值指示配置文件的AppSettings配置节中是否含有和参数“key”同名的键         
            public static bool ExistsKey(string key)
            {
                bool exist = false;
                for (int i = 0; i < ConfigurationManager.AppSettings.Keys.Count; i++)
                {
                    if (ConfigurationManager.AppSettings.Keys[i] == key)
                    {
                        exist = true;
                    }
                }
                return exist;
            }

            //保存当前appSettings
            public static void SaveAppSettings(Configuration cfg)
            {
                cfg.AppSettings.SectionInformation.ForceSave = true;//强制保存
                cfg.Save();
                ConfigurationManager.RefreshSection("appSettings");//刷新AppSettings
            }


            #endregion
        }
        #endregion

    }
}
