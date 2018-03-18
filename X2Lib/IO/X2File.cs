using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace X2Lib.IO
{
    /// <summary>
    ///  x2文件操作公共方法集合
    /// </summary>
    public class X2File
    {
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadFile(string filePath)
        {
            string content = "";
            StreamReader sr;
            try
            {
                sr = new StreamReader(filePath);
                content = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception)
            {
                content = "";
            }
            return content;
        }

        /// <summary>
        ///  创建一个文件或者为该文件添加一条记录
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void AppendFile(string filePath, string fileContent)
        {
            StreamWriter sr;
            if (File.Exists(filePath))
            {
                sr = File.AppendText(filePath);
            }
            else
            {
                sr = File.CreateText(filePath);
            }

            sr.WriteLine(fileContent);
            sr.Close();
        }

        /// <summary>
        /// 创建或打开一个文件用于写入 UTF-8 编码的文本
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void SaveFieContent(string filePath, string fileContent)
        {
            StreamWriter sr;
            try
            {
                sr = File.CreateText(filePath);

                sr.WriteLine(fileContent);
                sr.Close();

            }
            finally
            {
            }
        }

        /// <summary>
        /// 创建一个文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public static void SaveFile(string filePath, string fileContent)
        {
            StreamWriter sr;
            //if (File.Exists(filePath))
            //{
            //    sr = File.AppendText(filePath);
            //}
            //else
            //{
            //   sr = File.CreateText(filePath);
            //}

            sr = File.CreateText(filePath);

            sr.WriteLine(fileContent);
            sr.Close();
        }

        /// <summary>
        /// 指定目录下所有文件名称（包含扩展名称）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetFileNames(string path)
        {
            try
            {
                List<string> files = new List<string>(Directory.GetFiles(path).Select(x => GetFileName(x)));
                return files;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// 指定目录下所有文件名称（包含扩展名称）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extName">需要筛选的后缀名条件</param>
        /// <returns></returns>
        public static List<string> GetFileNames(string path, string extName)
        {

            List<string> files = GetFileNames(path);
            List<string> newFiles = new List<string>();
            if (files == null) return null;
            foreach (string item in files)
            {
                if (item.ToLower().EndsWith(extName.ToLower()))
                {
                    newFiles.Add(item);
                }
            }
            if (newFiles.Count <= 0) return null;
            return newFiles;

        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsExist(string fullPath)
        {
            return File.Exists(fullPath);
        }

        /// <summary>
        /// 移动文件到指定目标位置
        /// </summary>
        /// <param name="destFullPath"></param>
        /// <param name="orgFullPath"></param>
        public static void MoveFile(string destFullPath, string orgFullPath)
        {
            File.Move(orgFullPath, destFullPath);
        }

        /// <summary>
        /// 复制一个文件到指定位置
        /// </summary>
        /// <param name="destFullPath"></param>
        /// <param name="orgFullPath"></param>
        public static void CopyFile(string destFullPath, string orgFullPath)
        {
            File.Copy(orgFullPath, destFullPath, true);
        }

        /// <summary>
        /// 获取指定目录的所有目录名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetSubFolderNames(string path)
        {
            List<string> ret = new List<string>();
            try
            {
                DirectoryInfo current = new DirectoryInfo(path);
                DirectoryInfo[] items = current.GetDirectories();

                foreach (DirectoryInfo item in items)
                {
                    ///屏蔽隐藏文件
                    if ((item.Attributes & FileAttributes.Hidden) == 0)
                    {
                        ret.Add(item.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                ret.Add(ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetFileName(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        public static void DeleteFile(string filename)
        {

            ///修改文件的属性为可写
            ModifyFileAttribuate(filename);

            File.Delete(filename);
        }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            if (IsExist(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 对比两个文件
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="aimPath"></param>
        /// <returns></returns>
        public static bool CompareFile(string srcPath, string aimPath)
        {
            FileInfo srcFile = new FileInfo(srcPath);
            FileInfo aimFile = new FileInfo(aimPath);
            if (srcFile.Name == aimFile.Name && srcFile.CreationTime == aimFile.CreationTime)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 对文件流进行MD5加密 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MD5Stream(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            md5.ComputeHash(fs);
            fs.Close();

            byte[] b = md5.Hash;
            md5.Clear();

            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 不包含扩展名的文件名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameNotContainStuffix(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 文件所在目录名称
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetTheFileDirectoryName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Directory.Name;
        }

        /// <summary>
        /// 文件所在目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetTheFileDirectorFullyName(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        /// <summary>
        /// 修改文件的只读属性
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ModifyFileAttribuate(string filePath)
        {
            if (X2File.IsExist(filePath))
            {
                FileInfo fi = new FileInfo(filePath);
                ModifyFileAttribuate(fi);

            }
            return true;
        }

        /// <summary>
        /// 修改文件的只读属性为未读
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ModifyFileAttribuate(FileInfo fi)
        {
            if (fi.Exists)
            {
                if (fi.Attributes.ToString().Contains("ReadOnly"))
                    fi.Attributes = FileAttributes.Normal;

            }
            return true;
        }
    }
}
