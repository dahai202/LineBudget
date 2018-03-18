/**********************************************************
*名称:		     	异常处理模块
*功能:		        异常处理模块
*作者:		        wangjunhai
*创建时间:	     	2012/1/1
*最新修改:	     	2012/6/7
*修改描述:	     	
*			     	2012/6/7:
*			     		1.新增异常处理模块
*				    2012/6/29
*				        1./*单号:00009_00040, 当前处理人:wangjunhai[王君海], 处理时间：2012/6/29
***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X2Lib.IO;
using System.IO;
using System.Web;
using System.Runtime.Serialization;
using System.Xml;

namespace X2Lib.X2Sys
{
    public class X2Error
    {
        private static string rootPath = "";

        private static object errorLock = new object();

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="Mode">模块名称</param>
        public static void Log(string errorMessage, Exception ex, string moduleName)
        {
            ///写入日志
            if (string.IsNullOrEmpty(rootPath))
            {
                try
                {
                    if (System.Web.HttpContext.Current == null)
                        return;
                    else
                        rootPath = System.Web.HttpContext.Current.Request.MapPath(@"\Documentation\SYSLOG\");
                }
                catch
                {
                    return;
                }
                /*end  /*单号:00009_00040*/
            }

            try
            {
                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);
                string ErroLogFileName = GetTodayFileName();

                if (!X2File.IsExist(ErroLogFileName))
                {
                    X2File.AppendFile(ErroLogFileName, "<meta http-equiv=content-type content='charset=utf8'/>");
                }

                ///here should save this file into readable code
                string fileContent = GetErrorContent(errorMessage, ex, moduleName);
                lock (errorLock)
                {
                    X2File.AppendFile(ErroLogFileName, fileContent);
                }
            }
            catch (Exception exthis)
            {
                return;
            }
        }

        /// <summary>
        /// 记录日志 (该方法把日志存为XML文件,方法以年月建文件夹,天为文件名) 单号:00010_00021 郭飞 2012-6-11 00007_00295 2012-8-21 14:12:11
        /// </summary>
        /// <param name="userName" >用户名</param>
        /// <param name="logCategory">日志分类,通用对象传入对象名,其他日志传入自定义分类名</param>
        /// <param name="logType">日志类型</param>
        /// <param name="nvList">日志节点下自定义节点</param>
        /// <param name="content">日志内容</param>
        public static void Log(string userName, string logCategory, string logType, List<NameAndValue> nvList, string content)
        {
            DateTime time = DateTime.Now;
            string dicPath = System.Web.HttpContext.Current.Request.MapPath(string.Format(@"\Documentation\X2OBJLOG\{0}\", time.ToString("yyyyMM")));
            string filePath = dicPath + time.ToString("dd");
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
            if (!File.Exists(filePath + ".xml"))
            {
                using (var xmlSw = File.CreateText(filePath + ".xml"))
                {
                    xmlSw.WriteLine(string.Format("<?xml version=\"1.0\"?><!DOCTYPE logfile [<!ENTITY log  SYSTEM \"{0}.txt\">]><root>&log;</root>", time.ToString("dd")));
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<Log Category=\"{0}\" UserName=\"{1}\" Type=\"{2}\" Time=\"{3}\">", logCategory, userName, logType, DateTime.Now.Ticks);
            if (nvList != null && nvList.Count > 0)
            {
                foreach (var item in nvList)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", item.Name, item.Value);
                }
            }
            if (!string.IsNullOrEmpty(content))
            {
                sb.AppendFormat("<LogContent>{0}</LogContent>", content);
            }
            sb.Append("</Log>");
            lock (errorLock)
            {
                var sw = File.AppendText(filePath + ".txt");
                using (sw)
                {
                    sw.WriteLine(sb);
                }
            }
        }

        /// <summary>
        /// 获取日志节点集 00007_00295 郭飞 2012-8-21 14:12:49
        /// </summary>
        /// <param name="logCategory" >日志分类</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="logType">日志类型</param>
        /// <param name="xpath">查询XML日志的XPATH</param>
        /// <param name="errorMsg" >错误消息</param>
        /// <returns></returns>
        public static XmlDocument GetLog(string userName, string logCategory, DateTime startTime, DateTime endTime, string logType, string xpath, ref string errorMsg)
        {
            XmlDocument doc = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            XmlDocument tmpDoc = new XmlDocument();
            string dicPath = System.Web.HttpContext.Current.Request.MapPath(@"\Documentation\X2OBJLOG\");
            string filePath;
            for (; startTime <= endTime; startTime = startTime.AddDays(1))//遍历获取并解析时间段内的XML数据文件
            {
                filePath = string.Format(@"{0}{1}\{2}.xml", dicPath, startTime.ToString("yyyyMM"), startTime.ToString("dd"));
                if (!File.Exists(filePath)) continue;
                try
                {
                    XmlValidatingReader vr = new XmlValidatingReader(new XmlTextReader(filePath));
                    vr.ValidationType = ValidationType.None;
                    vr.EntityHandling = EntityHandling.ExpandEntities;
                    tmpDoc.Load(vr);
                    XmlNodeList list;
                    if (!string.IsNullOrEmpty(xpath))//若有xpath参数,则查找日志时以该参数为准
                    {
                        list = tmpDoc.SelectNodes(xpath + "/ancestor-or-self::Log");
                    }
                    else//否则用UI输入的参数
                    {
                        StringBuilder sbPath = new StringBuilder();
                        if (!string.IsNullOrEmpty(userName))
                        {
                            sbPath.AppendFormat("@UserName='{0}' and ", userName);
                        }
                        if (!string.IsNullOrEmpty(logCategory))
                        {
                            sbPath.AppendFormat("@Category='{0}' and ", logCategory);
                        }
                        if (!string.IsNullOrEmpty(logType))
                        {
                            sbPath.AppendFormat("@Type='{0}' and ", logType);
                        }
                        list = tmpDoc.SelectNodes(sbPath.Length < 1 ? "//Log" : string.Format("//Log[{0}]", sbPath.Remove(sbPath.Length - 5, 5)));
                    }
                    foreach (XmlNode item in list)//拼接查找出来的XML数据
                    {
                        sb.Append(item.OuterXml);
                    }
                    vr.Close();
                }
                catch (Exception ex)
                {
                    errorMsg += string.Concat("检索", filePath, "文件异常:", ex.Message);
                    break;
                }
            }
            doc.LoadXml(string.Format("<root>{0}</root>", sb));
            return doc;
        }

        /// <summary>
        /// 获取错误日志信息
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        private static string GetErrorContent(string ErrorMessage, Exception ex, string ModuleName)
        {

            StringBuilder sb = new StringBuilder();
            string exMsg = "";
            if (ex != null)
            {
                exMsg = ex.Message + "</br>" + ex.StackTrace;
            }

            sb.AppendFormat("\n<br/>{0}:{1}:{2}:[{3}]", ErrorMessage, exMsg, ModuleName, DateTime.Now.ToLongTimeString());
            return sb.ToString();
            /*start 单号:00009_00106, 当前处理人:wangjunhai[王君海]  */
        }

        private static string GetTodayFileName()
        {
            string dateFolder = rootPath + "//" + GetDateFolderName(DateTime.Now);
            if (!Directory.Exists(dateFolder))
            {
                Directory.CreateDirectory(dateFolder);
            }

            return dateFolder + "//Error_" + DateTime.Now.Hour.ToString() + ".htm";
        }

        public static void SetErrorLogFilePath(string path)
        {
            rootPath = path;
        }

        public static List<string> GetFileListByDay(DateTime date)
        {
            string dateFolder = rootPath + "//" + GetDateFolderName(date);

            if (!Directory.Exists(dateFolder))
            {
                return null;
            }
            else
            {
                return X2File.GetFileNames(dateFolder);
            }
        }

        public static string GetDateFolderName(DateTime date)
        {
            return "E_" + date.Year.ToString() + "_" + date.Month.ToString() + "_" + date.Day.ToString();
        }

    }

    #region 服务器返回错误信息集合
    /// <summary>
    /// 异常处理模块（返回给前台的可视化提示）
    /// </summary>
    [DataContract]
    public enum ReturnExceptionHandle
    {
        /// <summary>
        /// 前台参数不能成功解析
        /// </summary>
        [EnumMember(Value = "PARAMISNOTAVALIABLE")]//param is not avaliable
        PARAMISNOTAVALIABLE,
        /// <summary>
        /// 系统不存在该对象
        /// </summary>
        [EnumMember(Value = "NOSUCHOBJECT")]//no such object
        NOSUCHOBJECT,
        /// <summary>
        ///获取结果失败
        /// </summary>
        [EnumMember(Value = "COULDNOTGETTHERESULT")]//couldn't get the result
        COULDNOTGETTHERESULT,
        /// <summary>
        /// 不能成功保存元素
        /// </summary>
        [EnumMember(Value = "COULDNOTSAVEELEMENT")]//couldn't save element
        COULDNOTSAVEELEMENT,
        /// <summary>
        /// 不能获取对象的属性
        /// </summary>
        [EnumMember(Value = "COULDNOTGETPROPERTIES")]//couldn't get properties
        COULDNOTGETPROPERTIES,
        /// <summary>
        /// 返回对象结果为空
        /// </summary>
        [EnumMember(Value = "RETURNISEMPTY")]//return is empty
        RETURNISEMPTY,
        /// <summary>
        /// 群组对象子群组不能包含父群组
        /// </summary>
        [EnumMember(Value = "X2GROUPSUBNODE")]//X2Group Sub node should not contain the parent node
        X2GROUPSUBNODE,
        /// <summary>
        /// 系统上传文件失败
        /// </summary>
        [EnumMember(Value = "COULDNOTUPLOADFILE")]//Could not upload file
        COULDNOTUPLOADFILE,
        /// <summary>
        /// 系统配置未设置上传最大限制
        /// </summary>
        [EnumMember(Value = "NOMAXREQUESTLENGTH")]//NO httpRuntimeSection.MaxRequestLength
        NOMAXREQUESTLENGTH,
        /// <summary>
        /// 该账号在别处登录
        /// </summary>
        [EnumMember(Value = "REPLACEDBYOTHERS")]//been replaced by others
        REPLACEDBYOTHERS,
        /// <summary>
        /// 该账号在别处登录或者已经超时
        /// </summary>
        [EnumMember(Value = "REPLACEDBYOTHERSORSESSIONTIMEOUT")]//been replaced by others OR session timeout
        REPLACEDBYOTHERSORSESSIONTIMEOUT,
        /// <summary>
        /// 网络错误，无法连接到服务器！
        /// </summary>
        [EnumMember(Value = "NETWORKERROR")]//网络错误，无法连接到服务器！
        NETWORKERROR,
        /// <summary>
        /// 不能成功更新在线用户！
        /// </summary>
        [EnumMember(Value = "UPDATEONLINEUSERFAILURE")]//Update onlineuser failure 
        UPDATEONLINEUSERFAILURE,
        /// <summary>
        /// 读取用户的dll失败
        /// </summary>
        [EnumMember(Value = "READCLIENTDLLERROR")]//读取用户的dll失败
        READCLIENTDLLERROR,
        /// <summary>
        /// 发送短信失败
        /// </summary>
        [EnumMember(Value = "NOTSENDINGMSG")]//发送短信失败
        NOTSENDINGMSG,
        /// <summary>
        /// 电话号码不正确
        /// </summary>
        [EnumMember(Value = "NORIGHTFORMMOBILE")]//NO RIGHT FORM MOBILE 
        NORIGHTFORMMOBILE,
        /// <summary>
        /// 不是excel文件
        /// </summary>
        [EnumMember(Value = "NOTEXCELFILE")]//Not excel file
        NOTEXCELFILE,
        /// <summary>
        /// excel文件无数据
        /// </summary>
        [EnumMember(Value = "NOTEXCELDATA")]//Not excel Data
        NOTEXCELDATA,
        /// <summary>
        ///邮件主题或内容为空
        /// </summary>
        [EnumMember(Value = "MAILSUBJECTEMPTY")]// Mail subject or content is empty
        MAILSUBJECTEMPTY,
        /// <summary>
        ///发送邮件失败
        /// </summary>
        [EnumMember(Value = "SENDMAILFAIL")]// SENDMAILFAIL
        SENDMAILFAIL,
        /// <summary>
        /// 订单不存在
        /// </summary>
        [EnumMember(Value = "ORDERNOTEXIST")]
        ORDERNOTEXIST,
        /// <summary>
        /// 订单已支付
        /// </summary>
        [EnumMember(Value = "ORDERHASPAY")]
        ORDERHASPAY,
        /// <summary>
        /// 订单支付金额错误
        /// </summary>
        [EnumMember(Value = "ORDERPAYAMOUNTERROR")]
        ORDERPAYAMOUNTERROR
    }


    /// <summary>
    /// 元素异常处理模块（返回给前台的可视化提示）
    /// </summary>
    [DataContract]
    public enum ReturnElementException
    {
        /// <summary>
        /// 已存在该应用
        /// </summary>
        [EnumMember(Value = "EXITEDSAMEAPP")]//0x004
        EXITEDSAMEAPP,

        /// <summary>
        /// 新建应用为空
        /// </summary>
        [EnumMember(Value = "NEWAPPERROR")]//0x002
        NEWAPPERROR,

        /// <summary>
        /// 应用列表未成功加载，为NULL
        /// </summary>
        [EnumMember(Value = "APPLISTNULL")]//param is not avaliable
        APPLISTNULL,

        /// <summary>
        /// 元素信息文件异常
        /// </summary>
        [EnumMember(Value = "ELEMENTFILEERROR")]//Error:0x20000011
        ELEMENTFILEERROR,


        /// <summary>
        /// 文件不存在
        /// </summary>
        [EnumMember(Value = "FILEISNOTEXSITED")]//File is not exsited
        FILEISNOTEXSITED,

        /* 单号:00062_00522, 当前处理人:wangjunhai[王君海]*/
        /// <summary>
        /// 应用配置文件不存在
        /// </summary>
        [EnumMember(Value = "SETTINGFILEISNOTEXSITED")]
        SETTINGFILEISNOTEXSITED,
        /*end 单号:00062_00522, 当前处理人:wangjunhai[王君海]*/

    }


    /*单号:00009_00044, 当前处理人:wangjunhai[王君海],短信和邮件发送失败，前端给予友好提示*/
    /// <summary>
    /// 消息中心异常处理模块（返回给前台的可视化提示）
    /// </summary>
    [DataContract]
    public enum ReturnMsgException
    {
        /// <summary>
        /// 短信发送成功
        /// </summary>
        [EnumMember(Value = "SMSSENDSUCCESS")]//SMS send success! Transmission target:
        SMSSENDSUCCESS,

        /// <summary>
        /// 短信发送失败：
        /// </summary>
        [EnumMember(Value = "SMSSENDFAIL")]//SMS send failure! Transmission target:
        SMSSENDFAIL,

        /// <summary>
        /// 系统错误
        /// </summary>
        [EnumMember(Value = "CMDERROR")]//CMDERROR
        CMDERROR,

        /// <summary>
        /// 邮件发送失败，目标为：
        /// </summary>
        [EnumMember(Value = "MAILSENDFAIL")]// Mail send fail! Transmission target:
        MAILSENDFAIL,

        /// <summary>
        /// 邮件发送成功，目标为：
        /// </summary>
        [EnumMember(Value = "MAILSENDSUCCESS")]// Mail send success! Transmission target:
        MAILSENDSUCCESS,

        /// <summary>
        /// 手机号码未注册
        /// </summary>
        [EnumMember(Value = "PHONENOTREGISTERED")]//Mobile phone account is not registered!
        PHONENOTREGISTERED,

        /// <summary>
        /// 手机号码不正确
        /// </summary>
        [EnumMember(Value = "PHONENOTRIGHT")] // "Phone number is incorrect!"
        PHONENOTRIGHT,



        /// <summary>
        /// 邮箱账号不能为空
        /// </summary>
        [EnumMember(Value = "EmailAccountNotRight")]
        EMAILACCOUNTNOTRIGHT,


        /// <summary>
        /// 未配置 短信开关
        /// </summary>
        [EnumMember(Value = "NOSMSSWITCH")] // no sms switch
        NOSMSSWITCH,

        /// <summary>
        /// 未打开 短信开关
        /// </summary>
        [EnumMember(Value = "NOTOPENSMSSWITCH")] // not open  sms switch
        NOTOPENSMSSWITCH,

        /// <summary>
        /// 未配置 邮件开关
        /// </summary>
        [EnumMember(Value = "NOEMAILSWITCH")] // no email switch
        NOEMAILSWITCH,

        /// <summary>
        /// 未打开 邮件开关
        /// </summary>
        [EnumMember(Value = "NOTOPENSMSSWITCH")] // not open  emial switch
        NOTOPENEMAILSWITCH,

        /// <summary>
        /// 新建或者回复消息 收件人为空
        /// </summary>
        [EnumMember(Value = "NORECEIVEUSERS")] // there is no receive users
        NORECEIVEUSERS,


        /// <summary>
        /// 邮箱地址不存在
        /// </summary>
        [EnumMember(Value = "X2M_EMAILADDRESSDOESNOTEXIST")] 
        X2M_EMAILADDRESSDOESNOTEXIST,
    }
    /*end 单号:00009_00044*/


    /// <summary>
    /// 元素更新异常处理模块（返回给前台的可视化提示）
    /// </summary>
    [DataContract]
    public enum ReturnLOADElementException
    {
        /// <summary>
        /// 全部新增的
        /// </summary>
        [EnumMember(Value = "ADDNEW")]
        ADDNEW,

        /// <summary>
        /// 更新的
        /// </summary>
        [EnumMember(Value = "ADDNEW")]
        UPDATE,

        /// <summary>
        /// 已经存在相同的版本
        /// </summary>
        [EnumMember(Value = "SAMEVERSION")]
        SAMEVERSION,
    }

    #endregion

    #region 待办事宜 异常列表

    /// <summary>
    /// 待办事宜异常列表
    /// </summary>
    [DataContract]
    public enum ToDoListException
    {
        /// <summary>
        /// 参数不能解析
        /// </summary>
        [EnumMember(Value = "X2TPARAMISNOTAVALIABLE")]
        X2TPARAMISNOTAVALIABLE,

        /// <summary>
        /// 服务器 运行异常
        /// </summary>
        [EnumMember(Value = "SERVERRUNNINGANOMALY")]
        SERVERRUNNINGANOMALY

    }
    #endregion

    #region 系统出错信息集合
    /// <summary>
    /// 系统出错信息集合
    /// </summary>
    [DataContract]
    public enum SystemErrInfo
    {
        /// <summary>
        /// 参数不能解析
        /// </summary>
        [EnumMember(Value = "PARAMISNOTAVALIABLE")]
        PARAMISNOTAVALIABLE,

        /// <summary>
        /// 服务器运行异常
        /// </summary>
        [EnumMember(Value = "SERVERRUNNINGANOMALY")]
        SERVERRUNNINGANOMALY

    }

    #endregion

    #region 消息中心异常短编码列表


    /// <summary>
    /// 发送和回复消息中心异常
    /// </summary>
    [DataContract]
    public enum X2SendMsgException
    {
        /// <summary>
        /// 参数不能解析
        /// </summary>
        [EnumMember(Value = "X2M_X2TPARAMISNOTAVALIABLE")]
        X2M_X2TPARAMISNOTAVALIABLE,

        /// <summary>
        /// 服务器运行异常
        /// </summary>
        [EnumMember(Value = "X2M_SERVERRUNNINGANOMALY")]
        X2M_SERVERRUNNINGANOMALY,

        /// <summary>
        /// 没有收件人
        /// </summary>
        [EnumMember(Value = "X2M_NORECEIVEUSER")]
        X2M_NORECEIVEUSER,

        /// <summary>
        /// 没有消息数据
        /// </summary>
        [EnumMember(Value = "X2M_NOMSGDATA")]
        X2M_NOMSGDATA,

        /// <summary>
        /// 邮件标题包含了敏感字符
        /// </summary>
        [EnumMember(Value = "X2M_MAILSUBJCTCONTAINSSENSITIVECHARACTER")]
        X2M_MAILSUBJCTCONTAINSSENSITIVECHARACTER,

        /// <summary>
        /// 用户个人信息不包含个人邮件服务器配置信息
        /// </summary>
        [EnumMember(Value = "X2M_MAILSERVERCONFIGISNOTCORRECT")]
        X2M_MAILSERVERCONFIGISNOTCORRECT

    }


    #endregion

    #region x2系统统一异常的信息

    /// <summary>
    /// x2系统统一异常的信息
    /// </summary>
    [DataContract]
    public enum X2BLCommonException
    {
        /// <summary>
        /// BL传入参数不能解析
        /// </summary>
        [EnumMember(Value = "X2BL_PARAMISNOTAVALIABLE")]
        X2BL_PARAMISNOTAVALIABLE,

        /// <summary>
        /// BL系统运行异常
        /// </summary>
        [EnumMember(Value = "X2BL_SYSTEMERROR")]
        X2BL_SYSTEMERROR,

    }


    #endregion

    #region 系统功能运行异常

    /// <summary>
    /// 系统功能运行异常
    /// </summary>
    [DataContract]
    public enum X2BLSystemException
    {
        /// <summary>
        /// 表达式不能被执行
        /// </summary>
        [EnumMember(Value = "X2S_EXPRESSIONCANNOTBECARRYOUT")]
        X2S_EXPRESSIONCANNOTBECARRYOUT,

        /// <summary>
        /// 服务器不是中央服务器
        /// </summary>
        [EnumMember(Value = "X2S_THESERVERISNOTTHECENTRALSERVER")]
        X2S_THESERVERISNOTTHECENTRALSERVER,

        /// <summary>
        /// 多命令批处理的个数过多
        /// </summary>
        [EnumMember(Value = "X2S_CMDNUMBERTOOMUCH")]
        X2S_CMDNUMBERTOOMUCH,

        /// <summary>
        /// 命令的请求参数不能被替换
        /// </summary>
        [EnumMember(Value = "X2S_CMDPARAMCANNOTBEREPLACED")]
        X2S_CMDPARAMCANNOTBEREPLACED,

        /// <summary>
        /// 群组中不存在该用户
        /// </summary>
        [EnumMember(Value = "X2S_GROUPOFTHEUSERDOESNOTEXIST")]
        X2S_GROUPOFTHEUSERDOESNOTEXIST


    }
    #endregion

    /// <summary>
    /// 系统应用异常
    /// </summary>
    [DataContract]
    public enum X2BLDeskInfoException
    {
        /// <summary>
        /// 系统应用不存在
        /// </summary>
        [EnumMember(Value = "X2E_APPNOTEXISTED")]
        X2E_APPNOTEXISTED,

        /// <summary>
        /// 系统应用已经存在
        /// </summary>
        [EnumMember(Value = "X2E_APPHAVEEXISTED")]
        X2E_APPHAVEEXISTED,

        /// <summary>
        /// 运行权限的标题不存在
        /// </summary>
        [EnumMember(Value = "X2E_PERMISSIONRUNTITLEISNOTEXISTED")]
        X2E_PERMISSIONRUNTITLEISNOTEXISTED,
    }


    /// <summary>
    /// 应用包和补丁包异常
    /// </summary>
    [DataContract]
    public enum X2BLServerAppAndPatchException
    {
        #region 应用和元素的安装升级
        /// <summary>
        /// 系统应用包不存在
        /// </summary>
        [EnumMember(Value = "X2AS_APPPACKAGENOTEXISTED")]
        X2AS_APPPACKAGENOTEXISTED,


        /// <summary>
        /// 系统补丁包不存在
        /// </summary>
        [EnumMember(Value = "X2AS_PATCHNOTEXISTED")]
        X2AS_PATCHNOTEXISTED,

        /// <summary>
        /// 系统元素不存在
        /// </summary>
        [EnumMember(Value = "X2SS_SYSTEMELEMENTDOESNOTEXIST")]
        X2SS_SYSTEMELEMENTDOESNOTEXIST,

        /// <summary>
        /// 系统应用配置不存在
        /// </summary>
        [EnumMember(Value = "X2AS_APPPACKAGESETTINGNOTEXISTED")]
        X2AS_APPPACKAGESETTINGNOTEXISTED,

        /// <summary>
        /// 系统应用配置不正确
        /// </summary>
        [EnumMember(Value = "X2AS_APPPACKAGESETTINGWRONG")]
        X2AS_APPPACKAGESETTINGWRONG,

        /// <summary>
        /// 中央服务器不存在该应用相关信息
        /// </summary>
        [EnumMember(Value = "X2SS_THECENTRALSERVERDOESNOTEXISTTHEAPPLICATION")]
        X2SS_THECENTRALSERVERDOESNOTEXISTTHEAPPLICATION,

        /// <summary>
        /// 用户的唯一标识不能为空
        /// </summary>
        [EnumMember(Value = "X2SS_THEUSERUNIQUEIDENTIFIERCANNOTBEEMPTY")]
        X2SS_THEUSERUNIQUEIDENTIFIERCANNOTBEEMPTY,

        /// <summary>
        /// 新添加数据失败
        /// </summary>
        [EnumMember(Value = "X2SS_THENEWLYADDEDDATAFAILED")]
        X2SS_THENEWLYADDEDDATAFAILED,

        /// <summary>
        /// 请求服务器发生了异常
        /// </summary>
        [EnumMember(Value = "X2SS_REQUESTTOTHESERVEREXCEPTIONOCCURRED")]
        X2SS_REQUESTTOTHESERVEREXCEPTIONOCCURRED,
        #endregion

        #region 应用的一键上传同步

        /// <summary>
        /// 通用对象中元素或者应用的附件信息不存在
        /// </summary>
        [EnumMember(Value = "X2SS_THEATTACHMENTINFORMATIONDOESNOTEXIST")]
        X2SS_ATTACHMENTINFORMATIONDOESNOTEXIST,

        /// <summary>
        /// 通用对象中元素或者应用的附件信息不正确
        /// </summary>
        [EnumMember(Value = "X2SS_ATTACHMENTINFORMATIONISNOTCORRECT")]
        X2SS_ATTACHMENTINFORMATIONISNOTCORRECT,

        /// <summary>
        /// 指定元素id的元素在中央服务器的通用对象中不存在
        /// </summary>
        [EnumMember(Value = "X2SS_SPECIFYTHEIDOFTHEELEMENTDOESNOTEXIST")]
        X2SS_SPECIFYTHEIDOFTHEELEMENTDOESNOTEXIST,

        /// <summary>
        /// 指定id的应用在中央服务器的通用对象中不存在
        /// </summary>
        [EnumMember(Value = "X2SS_SPECIFYTHEIDOFTHEAPPDOESNOTEXIST")]
        X2SS_SPECIFYTHEIDOFTHEAPPDOESNOTEXIST,

        /// <summary>
        /// 请求者不是一个元素开发人员
        /// </summary>
        [EnumMember(Value = "X2SS_THEREQUESTERISNOTADEVELOPER")]
        X2SS_THEREQUESTERISNOTADEVELOPER,

        /// <summary>
        /// 应用或者元素已经被其他人锁定,请求者不能解锁元素或者应用
        /// </summary>
        [EnumMember(Value = "X2SS_THEREQUESTERCANN0TUNLOCK")]
        X2SS_THEREQUESTERCANN0TUNLOCK,

        /// <summary>
        /// mds安装文件不存在
        /// </summary>
        [EnumMember(Value = "X2SS_MDSINSTALLDOESNOTEXIST")]
        X2SS_MDSINSTALLDOESNOTEXIST,

        /// <summary>
        /// 应用包中的元素文件不存在
        /// </summary>
        [EnumMember(Value = "X2SS_ELEMENTINTHEAPPLICATIONPACKAGEFILEDOESNOTEXIST")]
        X2SS_ELEMENTINTHEAPPLICATIONPACKAGEFILEDOESNOTEXIST,

        /// <summary>
        /// 本地并未存储任何应用信息
        /// </summary>
        [EnumMember(Value = "X2SS_LOCALAPPLICATIONDOESNOTSTOREANYINFORMATION")]
        X2SS_LOCALAPPLICATIONDOESNOTSTOREANYINFORMATION,

        /// <summary>
        /// 中央服务器构建应用包失败
        /// </summary>
        [EnumMember(Value = "X2SS_APPLICATIONOFPACKAGESBUILDFAILURES")]
        X2SS_APPLICATIONOFPACKAGESBUILDFAILURES,

        /// <summary>
        /// 创建新版本时原版本的元素包信息不正确
        /// </summary>
        [EnumMember(Value = "X2SS_ELEMENTSTOCREATEANEWVERSION")]
        X2SS_ELEMENTSTOCREATEANEWVERSION,

        /// <summary>
        /// 中央服务器中该元素已经存在,但是指定版本不存在
        /// </summary>
        [EnumMember(Value = "X2SS_CENTRALSERVERWHICHALREADYEXISTS")]
        X2SS_CENTRALSERVERWHICHALREADYEXISTS,


        /// <summary>
        /// 该应用在中央服务器已经为测试发布或者正式发布状态，不允许更新
        /// </summary>
        [EnumMember(Value = "X2SS_THECENTRALSERVERHASBEENRELEASEDFORTHEAPPLICATION")]
        X2SS_THECENTRALSERVERHASBEENRELEASEDFORTHEAPPLICATION,

        #endregion

        #region 平台包的安装升级
        /// <summary>
        /// 中央服务器不存在指定的平台信息
        /// </summary>
        [EnumMember(Value = "X2SS_THECENTRALSERVERHASNOPLATFORMPACKAGE")]
        X2SS_THECENTRALSERVERHASNOPLATFORM,


        /// <summary>
        /// 中央服务器不存在指定的平台安装包
        /// </summary>
        [EnumMember(Value = "X2SS_THECENTRALSERVERHASNOPLATFORMPACKAGE")]
        X2SS_THECENTRALSERVERHASNOPLATFORMPACKAGE,

        /// <summary>
        /// 中央服务器平台安装包信息错误
        /// </summary>
        [EnumMember(Value = "X2SS_THECENTRALSERVERHASNOPLATFORMPACKAGE")]
        X2SS_THECENTRALSERVERHPLATFORMPACKAGEERROR,

        /// <summary>
        /// 中央服务器不存在客户端上传的包文件
        /// </summary>
        [EnumMember(Value = "X2SS_THECLIENTUPLOADEDFILEDOESNOTEXIST")]
        X2SS_THECLIENTUPLOADEDFILEDOESNOTEXIST,


        /// <summary>
        /// 中央服务器不存在指定的版本信息
        /// </summary>
        [EnumMember(Value = "X2SS_VERSIONINFORMATIONISNOTEXIST")]
        X2SS_VERSIONINFORMATIONISNOTEXIST,

        /// <summary>
        /// 中央服务器指定的版本信息错误
        /// </summary>
        [EnumMember(Value = "X2SS_VERSIONINFORMATIONISNOTCORRECT")]
        X2SS_VERSIONINFORMATIONISNOTCORRECT,


        /// <summary>
        /// 开发状态必须为开发中
        /// </summary>
        [EnumMember(Value = "X2SS_DEVELOPMENTSTATUSMUSTBEDEVELOPED")]
        X2SS_DEVELOPMENTSTATUSMUSTBEDEVELOPED,

        /// <summary>
        /// 客户服务器安装包路径错误
        /// </summary>
        [EnumMember(Value = "X2AS_SERVERPACKAGEPATHISEMPTY")]
        X2AS_SERVERPACKAGEPATHISEMPTY,

        /// <summary>
        /// 不是标准的平台版本号
        /// </summary>
        [EnumMember(Value = "X2AS_ISNOTASTANDARDPLATFORMVERSIONNUMBER")]
        X2AS_ISNOTASTANDARDPLATFORMVERSIONNUMBER,


        /// <summary>
        /// 平台包不正确
        /// </summary>
        [EnumMember(Value = "X2AS_PLATFORMPACKAGEISNOTCORRECT")]
        X2AS_PLATFORMPACKAGEISNOTCORRECT,

        #endregion
    }


    /// <summary>
    /// 一个键值对
    /// </summary>
    public class NameAndValue
    {
        /// <summary>
        /// 以一对键值构建该对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NameAndValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
