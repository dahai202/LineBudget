using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Web;
using X2Lib.PublicInfo;
using X2Lib.X2Sys;


namespace X2Lib.IO
{
    public static class ZipUtil
    {

        /// <summary>
        /// 文件压缩
        /// </summary>
        /// <param name="inputFolderPath"></param>
        /// <param name="outputPathAndFile"></param>
        /// <param name="password"></param>
        public static void ZipFiles(string inputFolderPath, string outputPathAndFile, string password)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            string outPath = inputFolderPath + @"\" + outputPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
            try
            {
                if (password != null && password != String.Empty)
                    oZipStream.Password = password;
                oZipStream.SetLevel(9); // maximum compression
                ZipEntry oZipEntry;
                foreach (string Fil in ar) // for each file, generate a zipentry
                {
                    oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                    oZipStream.PutNextEntry(oZipEntry);

                    if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                    {
                        ostream = File.OpenRead(Fil);
                        obuffer = new byte[ostream.Length];
                        ostream.Read(obuffer, 0, obuffer.Length);
                        oZipStream.Write(obuffer, 0, obuffer.Length);
                        ostream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oZipStream.Finish();
                oZipStream.Close();
            }
        }

        /// <summary>
        /// 多个文件压缩为一个文件 *.zip
        /// </summary>
        /// <param name="files"></param>
        /// <param name="outputFullPathAndFile"></param>
        /// <param name="password"></param>
        public static void ZipFilesList(List<string> files, string outputFullPathAndFile, string password)
        {
            FileStream ostream;
            byte[] obuffer;
            string outPath = outputFullPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
            if (password != null && password != String.Empty)
                oZipStream.Password = password;
            oZipStream.SetLevel(9); // maximum compression
            ZipEntry oZipEntry;
            foreach (string Fil in files) // for each file, generate a zipentry
            {
                ///将文件名称解码为字符串
                oZipEntry = new ZipEntry(HttpUtility.UrlDecode(X2File.GetFileName(Fil)));
                oZipStream.PutNextEntry(oZipEntry);
                if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                {
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[ostream.Length];
                    ostream.Read(obuffer, 0, obuffer.Length);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                    ostream.Close();
                }
            }
            oZipStream.Finish();
            oZipStream.Close();
        }

        /// <summary>
        /// 获取目录下所有文件列表，包含子目录
        /// </summary>
        /// <param name="Dir">目录路径</param>
        /// <returns>所有文件列表，包含子目录</returns>
        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                if (Directory.GetDirectories(Dir).Length == 0)
                // if directory is completely empty, add it
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils; // return file list
        }

        /// <summary>
        /// 将zip压缩文件解压
        /// </summary>
        /// <param name="zipPathAndFile"></param>
        /// <param name="outputFolder"></param>
        /// <param name="password"></param>
        /// <param name="deleteZipFile"></param>
        public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
        {
            ///
            ZipInputStream s = null;
            try
            {
                s = new ZipInputStream(File.OpenRead(zipPathAndFile));
                if (password != null && password != String.Empty)
                    s.Password = password;
                ZipEntry theEntry;
                string tmpEntry = String.Empty;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = outputFolder;
                    string fileName = Path.GetFileName(theEntry.Name);
                    // create directory 
                    if (directoryName != "")
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    if (fileName != String.Empty)
                    {
                        if (theEntry.Name.IndexOf(".ini") < 0)
                        {
                            string fullPath = directoryName + "\\" + theEntry.Name;
                            fullPath = fullPath.Replace("\\ ", "\\");
                            string fullDirPath = Path.GetDirectoryName(fullPath);
                            if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                            FileStream streamWriter = File.Create(fullPath);
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                        }
                    }
                }
                s.Close();

                if (deleteZipFile)
                    File.Delete(zipPathAndFile);
            }
            finally
            {
                if (s != null) s.Close();
                GC.Collect();
                GC.Collect(1);
            }
        }

        /// <summary>
        /// 文件集合打包压缩 构建文件名称和目录名称
        /// </summary>
        /// <param name="retMsg">运行情况</param>
        /// <param name="httpContext">HttpContext</param>
        /// <param name="totalFileNames">要压缩的文件名</param>
        /// <param name="fileInfoId">要压缩的目录</param>
        /// <param name="parameterList">文件集合</param>
        /// <param name="UpLoadZipFile">返回压缩文件信息</param>
        /// <returns></returns>
        public static bool FilesToZip(ref GenReturnMsg retMsg, HttpContext httpContext, string totalFileNames, string fileInfoId, List<UploadFileInfo> parameterList, ref UploadFileInfo UpLoadZipFile)
        {
            ///将第一个文件目录作为压缩包目录名称
            List<string> filePathList = new List<string>();
            List<string> allFileNames = new List<string>();

            ///遍历文件集合参数，组合为zip文件名称,
            foreach (var fileItem in parameterList)
            {
                ///不符合条件，弹出异常
                if (fileItem == null || string.IsNullOrEmpty(fileItem.fileId) || string.IsNullOrEmpty(fileItem.fileName))
                {
                    retMsg.Result = false;
                    retMsg.Msg = "param is not avaliable";
                    return false;
                }
                ///取出所有的文件路径
                string filePath = "";

                filePath = httpContext.Server.MapPath(@"Documentation\UploadFiles\" + fileItem.fileId + @"\"
                    + HttpUtility.UrlEncodeUnicode(HttpUtility.UrlDecode(fileItem.fileName).Replace("+", "%20")).Replace("+", "%2b"));

                if (!File.Exists(filePath))
                {
                    retMsg.Result = false;
                    retMsg.Msg = "NO FILE EXITS";
                    return false;
                }

                filePathList.Add(filePath);
                allFileNames.Add(X2Lib.IO.X2File.GetFileName(filePath));

            }


            #region 批量下载文件重命名解决

            string zipTempDir = httpContext.Server.MapPath(string.Format("{0}\\{1}\\", "Documentation\\UploadFiles\\Zip", Guid.NewGuid()));

            bool isHavaSameFileName = false;
            foreach (var item in allFileNames)
            {
                if (allFileNames.FindAll(x => x == item).Count > 1)
                {
                    isHavaSameFileName = true;
                    break;
                }
            }

            if (isHavaSameFileName)
            {
                List<string> fileNewPaths = new List<string>();
                X2Lib.IO.X2Directory.CreatDir(zipTempDir);

                foreach (var item in filePathList)
                {
                    string fileName = X2Lib.IO.X2File.GetFileNameNotContainStuffix(item);
                    string fileExtension = X2Lib.IO.X2File.GetExtension(item);
                    string newFilePath = CopyFile(fileName, fileExtension, item, zipTempDir, 1);
                    fileNewPaths.Add(newFilePath);
                }
                filePathList = fileNewPaths;
            }
            #endregion

            ///查询zip目录，没有就创建一个
            //if (!Directory.Exists(httpContext.Server.MapPath("Documentation\\UploadFiles\\Zip\\" + fileInfoId + "\\")))
            //{
            //    Directory.CreateDirectory(httpContext.Server.MapPath("Documentation\\UploadFiles\\Zip\\" + fileInfoId + "\\"));
            //}

            ///文件缓存
            Guid fileIdGuid = Guid.NewGuid();
            string fileCachePath = httpContext.Server.MapPath("Documentation\\UploadFiles\\FileCache\\" + fileIdGuid) + ".zip";
            ///查询zip目录，没有就创建一个
            if (!Directory.Exists(httpContext.Server.MapPath("Documentation\\UploadFiles\\FileCache\\")))
            {
                Directory.CreateDirectory(httpContext.Server.MapPath("Documentation\\UploadFiles\\FileCache\\"));
            }

            ///不存在该文件就执行压缩
            if (!File.Exists(fileCachePath))
            {
                ZipUtil.ZipFilesList(filePathList, fileCachePath, "");
            }

            if (X2Lib.IO.X2Directory.IsExist(zipTempDir))
            {
                X2Lib.IO.X2Directory.DelFolder(zipTempDir);
            }

            ///组合为压缩包信息
            UpLoadZipFile.fileName = HttpUtility.UrlEncodeUnicode(fileIdGuid + ".zip");
            UpLoadZipFile.fileId = fileInfoId;

            ///压缩包中有数据就正常返回
            if (filePathList.Count > 0)
            {
                retMsg.Result = true;
                return true;
            }
            else
            {
                retMsg.Result = false;
                retMsg.JasonResult = "NO FILE EXITS";
                return false;
            }
        }

        /// <summary>
        /// 批量下载重名文件处理
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <param name="filePath"></param>
        /// <param name="zipTempDir"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static string CopyFile(string fileName, string extension, string filePath, string zipTempDir, int i)
        {
            string copyFilePath = string.Concat(zipTempDir, fileName, extension);
            if (X2Lib.IO.X2File.IsExist(copyFilePath))
            {
                string fileReName = string.Empty;
                string strNum = fileName.Substring(fileName.Length-1);

                int num = 0;
                if (fileName.IndexOf("_") == fileName.Length - 2)
                {
                    if (!int.TryParse(strNum, out num))
                    {
                        fileReName = string.Concat(fileName, "_", i);
                    }
                    else
                    {
                        fileReName = string.Concat(fileName.Substring(0, fileName.Length - 2), "_", i);
                    }
                }
                else
                {
                    fileReName = string.Concat(fileName, "_", i);
                }

                return CopyFile(fileReName, extension, filePath, zipTempDir, ++i);
            }
            else
            {
                X2Lib.IO.X2File.CopyFile(copyFilePath, filePath);
                return copyFilePath;
            }
        }


        #region  元素的导入和导出


        /// <summary>
        /// 文件压缩
        /// </summary>
        /// <param name="inputFolderPath"></param>
        /// <param name="outputPathAndFile"></param>
        /// <param name="password"></param>
        public static void ZipElementFiles(string inputFolderPath, string outputPathAndFile, string password)
        {
            ArrayList ar = GenerateFileList(inputFolderPath); // generate file list
            int TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
            // find number of chars to remove     // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            string outPath = outputPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
            try
            {
                if (password != null && password != String.Empty)
                    oZipStream.Password = password;
                oZipStream.SetLevel(9); // maximum compression
                ZipEntry oZipEntry;
                foreach (string Fil in ar) // for each file, generate a zipentry
                {
                    oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                    oZipStream.PutNextEntry(oZipEntry);

                    if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
                    {
                        ostream = File.OpenRead(Fil);
                        obuffer = new byte[ostream.Length];
                        ostream.Read(obuffer, 0, obuffer.Length);
                        oZipStream.Write(obuffer, 0, obuffer.Length);
                        ostream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                X2Error.Log("ZipElementFiles", ex, "ZipElementFiles");
                throw ex;
            }
            finally
            {
                oZipStream.Finish();
                oZipStream.Close();
            }
        }
        #endregion

        #region  目录文件包压缩

        /// <summary>
        /// 文件安全锁
        /// </summary>
        private static object objLock = new object();

        /// <summary>
        /// 目录压缩
        /// </summary>
        /// <param name="inputFolderPath">源目录文件</param>
        /// <param name="outputPathAndFile">压缩包路径</param>
        /// <param name="password">压缩包密码</param>
        public static void ZipFolderFiles(string inputFolderPath, string outputPathAndFile, string password)
        {

            ///所有的文件信息
            ArrayList allFiles = GenerateFileList(inputFolderPath);

            ///父目录的长度
            int parentFolderLength = (Directory.GetParent(inputFolderPath)).ToString().Length;

            ///添加 '\\'长度
            parentFolderLength += 2;

            FileStream ostream;
            byte[] obuffer;
            ///压缩包的路径
            string outPath = outputPathAndFile;
            ///创建压缩文件
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath));

            ///防止文件被不同线程操作
            lock (objLock)
            {
                try
                {
                    ///压缩包密码
                    if (password != null && password != String.Empty)
                        oZipStream.Password = password;

                    ///压缩等级
                    oZipStream.SetLevel(9);
                    ZipEntry oZipEntry;

                    ///遍历所有文件，写入压缩包
                    foreach (string file in allFiles)
                    {
                        ///去除父目录得到文件相对路径
                        oZipEntry = new ZipEntry(file.Substring(parentFolderLength));
                        oZipStream.PutNextEntry(oZipEntry);

                        ///文件结构直接写入
                        if (!file.EndsWith(@"/"))
                        {
                            ostream = File.OpenRead(file);
                            obuffer = new byte[ostream.Length];
                            ostream.Read(obuffer, 0, obuffer.Length);
                            oZipStream.Write(obuffer, 0, obuffer.Length);
                            ostream.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    X2Error.Log("Zip.ElementFiles", ex, "Zip.ElementFiles");
                    throw ex;
                }
                finally
                {
                    ///数据的释放
                    oZipStream.Finish();
                    oZipStream.Close();
                }
            }

        }

        #endregion

        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        private static bool ZipFileDictory(string folderToZip, ZipOutputStream s, string parentFolderName)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
            try
            {
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                filenames = Directory.GetFiles(folderToZip);
                foreach (string file in filenames)
                {
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string folderZipName = Path.GetFileName(folderToZip);
                    folderZipName = string.IsNullOrEmpty(folderZipName) ? string.Empty : string.Concat(folderZipName, "/");
                    entry = new ZipEntry(Path.Combine(parentFolderName, folderZipName + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(folderToZip);
            foreach (string folder in folders)
            {
                if (!ZipFileDictory(folder, s, Path.Combine(parentFolderName, Path.GetFileName(folderToZip))))
                {
                    return false;
                }
            }
            return res;
        }

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="FolderToZip">待压缩的文件夹，全路径格式</param>
        /// <param name="ZipedFile">压缩后的文件名，全路径格式</param>
        public static bool ZipFileDictory(string folderToZip, string zipedFile, int level, string password)
        {
            bool res;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            ZipOutputStream s = new ZipOutputStream(File.Create(zipedFile));
            s.SetLevel(level);
            ///压缩包密码
            if (password != null && password != String.Empty)
                s.Password = password;
            res = ZipFileDictory(folderToZip, s, "");
            s.Finish();
            s.Close();
            return res;
        }


        #region 可以排除文件和指定目录的压缩方法
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="FolderToZip">待压缩的文件夹，全路径格式</param>
        /// <param name="ZipedFile">压缩后的文件名，全路径格式</param>
        public static bool ZipFileDictory(string folderToZip, string zipedFile, List<string> eliminateDirs,
            List<string> eliminateFilePaths, int level, string password)
        {
            bool res;
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            ZipOutputStream s = new ZipOutputStream(File.Create(zipedFile));
            s.SetLevel(level);
            ///压缩包密码
            if (password != null && password != String.Empty)
                s.Password = password;

            List<DirectoryInfo> eliminateDirInfo = new List<DirectoryInfo>();
            List<FileInfo> eliminateFileInfo = new List<FileInfo>();

            foreach (var item in eliminateDirs)
            {
                eliminateDirInfo.Add(new DirectoryInfo(item));
            }

            foreach (var item in eliminateFilePaths)
            {
                eliminateFileInfo.Add(new FileInfo(item));
            }

            res = ZipFileDictory(folderToZip, s, "", eliminateDirInfo, eliminateFileInfo);
            s.Finish();
            s.Close();
            return res;
        }


        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        private static bool ZipFileDictory(string folderToZip, ZipOutputStream s, string parentFolderName, List<DirectoryInfo> eliminateInfos,
            List<FileInfo> eliminateFileInfos)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
            try
            {
                entry = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                filenames = Directory.GetFiles(folderToZip);
                foreach (string file in filenames)
                {

                    FileInfo fileInfo = new FileInfo(file);
                    if (eliminateFileInfos.Contains(fileInfo)) continue;


                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string folderZipName = Path.GetFileName(folderToZip);
                    folderZipName = string.IsNullOrEmpty(folderZipName) ? string.Empty : string.Concat(folderZipName, "/");
                    entry = new ZipEntry(Path.Combine(parentFolderName, folderZipName + Path.GetFileName(file)));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                {
                    entry = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(folderToZip);
            foreach (string folder in folders)
            {
                DirectoryInfo folderInfo = new DirectoryInfo(folder);
                if (eliminateInfos.Contains(folderInfo)) continue;

                if (!ZipFileDictory(folder, s
                      , Path.Combine(parentFolderName, Path.GetFileName(folderToZip)), eliminateInfos, eliminateFileInfos))
                {
                    return false;
                }
            }
            return res;
        }


        #endregion
    }
}
