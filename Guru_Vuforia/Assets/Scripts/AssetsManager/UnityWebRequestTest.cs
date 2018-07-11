using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System;

public class UnityWebRequestTest : MonoSingleton<UnityWebRequestTest>
{
    private const long l = 1024L;
    private void OnApplicationQuit()
    {
        StopAllCoroutines();//关闭所有协程  
    }

    private void OnDestroy()
    {
        //if (request != null)
        //{
        //    //以下步骤很重要，用户中止时，放弃请求，如果不放弃，连续多次请求，就会出现卡死。  
        //    request.Abort();
        //    UnityEngine.Debug.Log("request.Abort();");
        //}
    }

    public void TestLoad()
    {
        StartCoroutine(HttpWebRequestTest());
    }

    IEnumerator HttpWebRequestTest()
    {
        ServicePointManager.DefaultConnectionLimit = 200; //原本默认是2，此步骤应该移动到外部
        string netFilePath = "http://127.0.0.1:8080/testLoad.rar";
        string tempFilePath = @"D:/MyStuff/Desktop/testLoad.rar";
        //check netFilePath 存在？
        //check tempFilePath 存在？
        HttpWebRequest request = null;
        HttpWebResponse response = null;
        bool isResumeFromBreakPoint = false;
        //打开网络连接

        FileStream fs = new FileStream(tempFilePath, FileMode.OpenOrCreate);
        long totalLength = 0;
        if (fs.Length > 0)
        {
            //获取临时文件大小，注意，不要再创建request，连续请求可能卡死，开启下次请求前请先结束上一次的请求。 
            request = OpenConnection(netFilePath);
            SetRequestParam(ref request);
            response = (HttpWebResponse)request.GetResponse();
            totalLength = response.ContentLength;
            CloseConnection(ref response, ref request);
            if (fs.Length >= totalLength)
            {
                fs.Close();
                Debug.Log("不用下载");
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
            catch(Exception e)
            {
                Debug.Log(e.StackTrace);
                keepWritting = false;
                yield break;
                //调用终止现在函数
            }

            nReadSize = ns.Read(nbytes, 0, memoryBuffer);
            Debug.Log("当前大小 : " + ((double)((fs.Length - last) / l) / Time.deltaTime).ToString("0") +" KB/S");
            yield return false;
        }

        ns.Close();
        fs.Close();
        CloseConnection(ref response, ref request);

        Debug.Log("下载完毕 ！！！！！！！！！！！！！！！！！！");
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

    private void CloseConnection(ref HttpWebResponse response, ref HttpWebRequest request)
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

    // 设置下载缓冲区大小
    private int GetDownloadMemoryBuffer()
    {
        return 4 * 1024 * 1024;
    }

    //如果下载到一半终止 之后再更新时 服务器端已经发生变化 记得将之前的临时文件删掉 之后从新下载 所以在索引文件对比后生成要删除的临时文件队列
    //request 做一个缓存以便 被终止时调用释放方法
}