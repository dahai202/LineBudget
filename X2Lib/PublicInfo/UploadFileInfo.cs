using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace X2Lib.PublicInfo
{

    [DataContract]
    public class UploadFileInfo
    {
        [DataMember]
        public string fileName;

        [DataMember]
        public string fileId;

        [DataMember]
        public string fileExt;

        [DataMember]
        public string Msg;

        [DataMember]
        public string filePath;
    
    }


    /// <summary>
    /// 下载打包zip文件需要的前台参数
    /// </summary>
    [DataContract]
    public class DownLoadZipFile
    {
        /// <summary>
        /// 压缩包文件名称
        /// </summary>
        [DataMember]
        public string zipName { get; set; }

        /// <summary>
        /// 压缩包所需要的文件集合
        /// </summary>
        [DataMember]
        public List<UploadFileInfo> fileList { get; set; }
    }

  
}
