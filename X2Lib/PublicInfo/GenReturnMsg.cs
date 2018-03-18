using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace X2Lib.PublicInfo
{

    [DataContract]
    public class RuningFootPrint
    {
        [DataMember]
        public string OccurTime;

        [DataMember]
        public string Title;

        public RuningFootPrint()
        {
            OccurTime = DateTime.UtcNow.Ticks.ToString();
            Title = "default";
        }

        public RuningFootPrint(string _title)
        {
            OccurTime = DateTime.UtcNow.Ticks.ToString();
            Title = _title;
        }

    }

    [DataContract]
    public class ReturnMsg<T>
    {
        public bool result;

        public string msg;

        public List<RuningFootPrint> FootPrints;

        public T resultObj;

        public void AddFootPrint(string title)
        {
            if (FootPrints == null)
            {
                FootPrints = new List<RuningFootPrint>();
            }

            FootPrints.Add(new RuningFootPrint(title));
        }

        public void AddFootPrint()
        {
            AddFootPrint("default");
        }

    }

    /// <summary>
    /// x2返回数据标准格式
    /// </summary>
    [DataContract]
    public class GenReturnMsg
    {
        /// <summary>
        /// 运行情况  true  or  false
        /// </summary>
        [DataMember]
        public bool Result;

        /// <summary>
        /// 异常短编码
        /// </summary>
        [DataMember]
        public string Msg;

        /// <summary>
        /// 错误编码
        /// </summary>
        [DataMember]
        public string ErrorCode;

        /// <summary>
        /// 异常描述
        /// </summary>
        [DataMember]
        public string X2Exception;

        /// <summary>
        /// 新增请求命令名称
        /// </summary>
        [DataMember]
        public string CmdName;

        [DataMember]
        public List<RuningFootPrint> FootPrints;

        /// <summary>
        /// 如果服务器运行正常：返回给前端的json数据
        /// </summary>
        [DataMember]
        public string JasonResult;

        public void AddFootPrint(string title)
        {
            FootPrints.Add(new RuningFootPrint(title));
        }

        public void AddFootPrint()
        {
            AddFootPrint("default");
        }

        public GenReturnMsg()
        {
            Result = true;
            Msg = "";
            JasonResult = "";
            FootPrints = new List<RuningFootPrint>();
        }

        /// <summary>
        /// 快速添加错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="x2exception"></param>
        /// <param name="jsonResult"></param>
        public void SetErrorMsg(string msg, string x2exception)
        {
            Result = false;
            Msg = msg;
            X2Exception = x2exception;
        }

        /// <summary>
        /// 快速添加错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="x2exception"></param>
        /// <param name="jsonResult"></param>
        public void SetErrorMsg(object msg, object x2exception)
        {
            Result = false;
            Msg = msg.ToString();
            X2Exception = x2exception.ToString();
        }
    }

}
