using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace FileTools
{
    public class ExecuteLoad_HttpWebRequest : ExecuteLoadByLoadFile, ExecuteLoad
    {
        public const byte MAX_EXECUTE_COUNT = 1;
        private const int connectionLimit = 200;
        private HttpWebRequest request = null;
        private HttpWebResponse response = null;
        private bool isResumeFromBreakPoint = false;

        public ExecuteLoad_HttpWebRequest(LoadFile loadFile) : base(loadFile)
        {
            this.loadFile = loadFile;
        }

        public IEnumerator Execute()
        {
            int defaultConnectionLimit = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = connectionLimit; //原本默认是2，此步骤应该移动到外部

            string tempFilePath = FileHelper.GetFilePath(loadFile.CorrelateRecord, FileAddressType.DOWNLOAD_TEMP);
            string netFilePath = FileHelper.GetFilePath(loadFile.CorrelateRecord, FileAddressType.SERVER);

            DirectoryInfo folderPathInfo = new DirectoryInfo(Path.GetDirectoryName(tempFilePath));
            if (!folderPathInfo.Exists)
            {
                folderPathInfo.Create();
            }
            FileStream fs = new FileStream(tempFilePath, FileMode.OpenOrCreate);
            long totalLength = 0;
            if (fs.Length > 0)
            {
                //获取临时文件大小，注意，不要再创建request，连续请求可能卡死，开启下次请求前请先结束上一次的请求。 
                request = OpenConnection(netFilePath);
                SetRequestParam(ref request);
                response = (HttpWebResponse)request.GetResponse();
                totalLength = response.ContentLength;
                CloseConnection();
                if (fs.Length >= totalLength)
                {
                    loadFile.IsLoadSuccess = true;
                    //Debug.Log(loadFile.CorrelateRecord.IndexName + "不用下载 ！");
                    fs.Close();
                    ServicePointManager.DefaultConnectionLimit = defaultConnectionLimit;
                    ClearAndClose();
                    yield break;
                }
                isResumeFromBreakPoint = true;
            }

            //向服务器请求，获得服务器回应数据流  
            request = OpenConnection(netFilePath);
            SetRequestParam(ref request);
            if (isResumeFromBreakPoint)
            {
                //设置Range值 
                request.AddRange((int)fs.Length);
                fs.Seek(fs.Length, SeekOrigin.Begin);
            }
            response = (HttpWebResponse)request.GetResponse();
            Stream ns = response.GetResponseStream();
            int memoryBuffer = GetDownloadMemoryBuffer();
            byte[] nbytes = new byte[memoryBuffer];
            int nReadSize = 0;
            bool keepWritting = true;
            nReadSize = ns.Read(nbytes, 0, memoryBuffer);
            while (keepWritting && (nReadSize > 0))
            {
                long last = fs.Length;
                try
                {
                    fs.Write(nbytes, 0, nReadSize);
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                    keepWritting = false;
                    yield break;
                    //调用终止现在函数
                }

                nReadSize = ns.Read(nbytes, 0, memoryBuffer);
                //Debug.Log("当前大小 : " + ((double)((fs.Length - last) / 1024L) / Time.deltaTime).ToString("0") + " KB/S");
                yield return false;
            }

            loadFile.IsLoadSuccess = true;
            ns.Close();
            fs.Close();
            
            ServicePointManager.DefaultConnectionLimit = defaultConnectionLimit;
            //Debug.Log(loadFile.CorrelateRecord.IndexName + " 下载完毕 ！！！！！！！！！！！！！！！！！！");
            ClearAndClose();
            ////这里放更新安装代码,或者可以测试这个下载的包有没有出错,验证sha和md5 
        }

        private HttpWebRequest OpenConnection(string netFilePath)
        {
            GC.Collect();
            return (HttpWebRequest)WebRequest.Create(new Uri(netFilePath));
        }

        private void SetRequestParam(ref HttpWebRequest request)
        {
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.AllowWriteStreamBuffering = false;
            //request.ServicePoint.ConnectionLimit = 65500;
            request.Method = "GET";
            request.Proxy = null; //要不速度会很慢
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Timeout = 5000;
        }

        public void CloseConnection()
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }

            if (request != null)
            {
                request.Abort();
                request = null;
            }
        }

        public void ClearAndClose()
        {
            CloseConnection();
            loadFile = null;
        }

        // 设置下载缓冲区大小
        private int GetDownloadMemoryBuffer()
        {
            return 4 * 1024 * 1024;
        }
    }
}
