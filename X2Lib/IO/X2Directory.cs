using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace X2Lib.IO
{
    /// <summary>
    /// x2文件夹公共处理类
    /// </summary>
    public class X2Directory
    {
        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="dirPath"></param>
        public static void CreatDir(string dirPath)
        {
            if (!IsExist(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool IsExist(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        /// <summary>
        /// 获取指定文件夹下的子目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetSubDirs(string dirPath)
        {
            return new List<string>(Directory.GetDirectories(dirPath));
        }

        /// <summary>
        ///  获取指定文件夹下的所有子目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetAllSubDirs(string dirPath)
        {
            List<string> allDirs = new List<string>();

            DirectoryInfo curDir = new DirectoryInfo(dirPath);
            DirectoryInfo[] subDirs = curDir.GetDirectories();
            foreach (var item in subDirs)
            {
                allDirs.AddRange(GetSubDirs(item.FullName));
            }

            return allDirs;
        }

        /// <summary>
        /// 获取指定文件夹下文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetSubFiles(string dirPath)
        {
            return new List<string>(Directory.GetFiles(dirPath));
        }

        /// <summary>
        /// 获取指定文件夹下文件和目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetFileSystemEntries(string dirPath)
        {
            return new List<string>(Directory.GetFileSystemEntries(dirPath));
        }

        /// <summary>
        /// 获取指定文件夹下所有文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetAllSubFiles(string dirPath)
        {
            List<string> allFiles = new List<string>();

            DirectoryInfo curDir = new DirectoryInfo(dirPath);
            allFiles.AddRange(GetSubFiles(dirPath));

            DirectoryInfo[] subDirs = curDir.GetDirectories();
            foreach (var item in subDirs)
            {
                allFiles.AddRange(GetSubFiles(item.FullName));
            }

            return allFiles;
        }

        /// <summary>
        /// 递归删除目录
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DelFolder(string dirPath)
        {
            if (IsExist(dirPath))
            {
                ///修改目录属性为可写
                ModifyDirectoryAttribuate(dirPath);

                DirectoryInfo mainDir = new DirectoryInfo(dirPath);

                DirectoryInfo[] subDir = mainDir.GetDirectories();
                if (subDir != null && subDir.Length > 0)
                {
                    foreach (var item in subDir)
                    {
                        DelFolder(item.FullName);
                    }
                }
                if (mainDir.Exists)
                    mainDir.Delete(true);
            }
        }

        /// <summary>
        /// 递归删除目录
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DelFolder(string dirPath, bool recursive)
        {
            if (IsExist(dirPath))
            {
                ///修改目录属性为可写
                ModifyDirectoryAttribuate(dirPath);
                DelFolder(dirPath);
                //DirectoryInfo mainDir = new DirectoryInfo(dirPath);
                //mainDir.Delete(true);
            }
        }

        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void CreateFolder(string orignFolder, string newFloder)
        {
            Directory.SetCurrentDirectory(orignFolder);
            Directory.CreateDirectory(newFloder);
        }

        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        /// <param name="isOnlyFiles">是否只复制文件</param>
        public static void CopyDir(string srcPath, string aimPath, CopyType copyType, string extName)
        {
            try
            {
                ///检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;

                ///判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);


                ///修改目录属性为可写
                ModifyDirectoryAttribuate(aimPath);


                ///得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                string[] fileList = new string[] { };

                ///根据复制的类型获取文件/目录数组
                switch (copyType)
                {
                    case CopyType.OnlyFile:
                        fileList = Directory.GetFiles(srcPath);
                        break;
                    case CopyType.OnlyDirectory:
                        fileList = Directory.GetDirectories(srcPath);
                        break;
                    case CopyType.AllFileAndDirectory:
                        fileList = Directory.GetFileSystemEntries(srcPath);
                        break;
                    default:
                        break;
                }

                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    if (!string.IsNullOrEmpty(extName) && file.EndsWith(extName)) continue;

                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))

                        CopyDir(file, aimPath + Path.GetFileName(file), copyType, extName);
                    //否则直接Copy文件
                    else
                    {
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }

        }

        public static void CopyDir(string srcPath, string aimPath, CopyType copyType)
        {
            CopyDir(srcPath, aimPath, copyType, string.Empty);
        }

        /// <summary>
        /// 添加字符,该字符用于在反映分层文件系统组织的路径字符串中分隔目录级别
        /// </summary>
        /// <param name="aimPath"></param>
        /// <returns></returns>
        public static string AddDirectorySeparatorChar(string aimPath)
        {
            ///检查目标目录是否以目录分割字符结束如果不是则添加之 
            if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                aimPath += Path.DirectorySeparatorChar;
            return aimPath;
        }

        /// <summary>
        /// 文件夹复制类型
        /// </summary>
        public enum CopyType
        {
            /// <summary>
            /// 只复制文件
            /// </summary>
            OnlyFile = 0,

            /// <summary>
            /// 只复制目录
            /// </summary>
            OnlyDirectory = 1,

            /// <summary>
            /// 文件和目录
            /// </summary>
            AllFileAndDirectory = 3
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns>返回是long类型</returns>
        public static long GetDirectoryLength(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return 0;

            long len = 0;
            DirectoryInfo di = new DirectoryInfo(dirPath);

            ///遍历文件
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }

            ///递归目录
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 差异文件夹的复制
        /// </summary>
        /// <param name="newDirPath">复制到的新文件夹</param>
        /// <param name="srcDirPath">原始的目录</param>
        /// <param name="aimDirPath">目标对比的目录</param>
        /// <returns>成功与否</returns>
        public static void CopyDifferenceFiles(string newDirPath, string srcDirPath, string aimDirPath)
        {
            if (IsExist(newDirPath))
            {
                CreatDir(newDirPath);
            }

            ///源目标不存在，则直接复制
            if (string.IsNullOrEmpty(srcDirPath))
            {
                CopyDir(aimDirPath, newDirPath, CopyType.AllFileAndDirectory);
                return;
            }

            DirectoryInfo srcInfo = new DirectoryInfo(srcDirPath);
            DirectoryInfo aimInfo = new DirectoryInfo(aimDirPath);

            ///文件对比
            foreach (var aimItem in aimInfo.GetFiles())
            {
                bool isSameFile = false;
                foreach (var srcItem in srcInfo.GetFiles())
                {
                    if (X2File.CompareFile(srcItem.FullName, aimItem.FullName))
                    {
                        isSameFile = true;
                        break;
                    }
                }
                ///新增或者不同的文件
                if (!isSameFile)
                    File.Copy(aimItem.FullName, newDirPath + @"\" + aimItem.Name);
            }

            ///目录对比
            foreach (var aimItem in aimInfo.GetDirectories())
            {
                ///空目录直接排出
                if (aimItem.GetFileSystemInfos().Length == 0)
                {
                    continue;
                }

                bool isSameFile = false;
                foreach (var srcItem in srcInfo.GetDirectories())
                {
                    ///相同目录递归对比
                    if (srcItem.Name == aimItem.Name)
                    {
                        CopyDifferenceFiles(newDirPath + @"\" + aimItem.Name, srcItem.FullName, aimItem.FullName);
                        isSameFile = true;
                        break;
                    }
                }
                ///新增的目录
                if (!isSameFile)
                    CopyDir(aimItem.FullName, newDirPath + @"\" + aimItem.Name, CopyType.AllFileAndDirectory);
            }
        }

        /// <summary>
        /// 获取目录名称
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <returns>返回名称</returns>
        public static string GetPathName(string folderPath)
        {
            return new DirectoryInfo(folderPath).Name;
        }

        /// <summary>
        /// 获取父目录
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <returns>返回</returns>
        public static string GetParent(string folderPath)
        {
            return new DirectoryInfo(folderPath).Parent.FullName;
        }

        /// <summary>
        /// 修改文件夹的只读属性
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ModifyDirectoryAttribuate(string dirPath)
        {
            if (X2Directory.IsExist(dirPath))
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                ModifyDirectoryAttribuate(dir);
            }
            return true;
        }


        /// <summary>
        /// 修改文件夹的只读属性
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ModifyDirectoryAttribuate(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                ////递归修改属性
                DirectoryInfo[] subDirs = dir.GetDirectories();
                foreach (var item in subDirs)
                {
                    ModifyDirectoryAttribuate(item);
                }
                FileInfo[] subFiles = dir.GetFiles();
                foreach (var item in subFiles)
                {
                    X2File.ModifyFileAttribuate(item);
                }
                if (dir.Attributes.ToString().Contains("ReadOnly"))
                    dir.Attributes = FileAttributes.Normal;

            }
            return true;
        }

    }


}
