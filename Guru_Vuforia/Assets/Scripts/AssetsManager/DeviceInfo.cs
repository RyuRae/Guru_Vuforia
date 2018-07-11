using System;
using System.Collections;
using UnityEngine;
using System.Text;
using System.IO;

public class DeviceInfo : MonoSingleton<DeviceInfo>
{
    public bool isKeepGettingInfo = true;
    //-----------------------------------------------------------------------------------------
    #region 信息数据
    private string _BatteryLevelStr = "?";
    public string BatteryLevelStr
    {
        get { return _BatteryLevelStr; }
    }

    private string _SystemTimeStr = "";
    public string SystemTimeStr
    {
        get { return _SystemTimeStr; }
    }

    private NetworkReachability _NetWorkerState = NetworkReachability.NotReachable;
    public string NetWorkerStateStr
    {
        get
        {
            string result = "???";
            switch (_NetWorkerState)
            {
                case NetworkReachability.NotReachable:
                    result = "无网络";
                    break;

                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    result = "正在使用WiFi或网线";
                    break;

                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    result = "正在使用移动网络";
                    break;
            }
            return result;
        }
    }

    private bool _KeepGettingInfo = false;
    public bool KeepGettingInfo
    {
        get { return _KeepGettingInfo; }
        set
        {
            _KeepGettingInfo = value;
            if (_KeepGettingInfo)
            {
                //StartCoroutine("UpdataBatteryLevel");
                //StartCoroutine("UpdataSystemTime");
                //StartCoroutine("UpdataNetWorker");
                StartCoroutine("UpdateFreeDiskSpace");
                //StartCoroutine("PrintInfo");
            }
            else
            {
                //StopCoroutine("UpdataBatteryLevel");
                //StopCoroutine("UpdataSystemTime");
                //StopCoroutine("UpdataNetWorker");
                StopCoroutine("UpdateFreeDiskSpace");
                //StopCoroutine("PrintInfo");
            }
        }
    }


    private long _FreeDiskSpace = 0L;

    public long FreeDiskSpace
    {
        get { return _FreeDiskSpace; }
    }


    #endregion
    //-----------------------------------------------------------------------------------------

    private IEnumerator UpdataBatteryLevel()
    {
        while (true)
        {
            int result = GetBatteryLevel();
            _BatteryLevelStr = result == -1 ? "?" : result.ToString();
            yield return new WaitForSeconds(300f);
        }
    }
    private int GetBatteryLevel()
    {
        try
        {
            string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
            return int.Parse(CapacityString);
        }
        catch
        {
            return -1;
        }
    }

    private IEnumerator UpdataSystemTime()
    {
        DateTime now = DateTime.Now;
        SetSystemTimeStr(now);
        yield return new WaitForSeconds(60f - now.Second);
        while (true)
        {
            now = DateTime.Now;
            SetSystemTimeStr(now);
            yield return new WaitForSeconds(60f);
        }
    }

    private void SetSystemTimeStr(DateTime now)
    {
        _SystemTimeStr = string.Format("{0:00}:{1:00}", now.Hour, now.Minute);
    }

    private IEnumerator UpdataNetWorker()
    {
        while (true)
        {
            _NetWorkerState = Application.internetReachability;
            yield return new WaitForSeconds(300f);
        }
    }

    //获取沙盒空间目录大小
    public IEnumerator UpdateFreeDiskSpace()
    {
        while (true)
        {
            DirectoryInfo fi = new DirectoryInfo(Application.persistentDataPath);

            if (fi.Exists)
            {
                

            }


            Debug.Log(Application.persistentDataPath);
            yield return new WaitForSeconds(1f);
        }
    }


    //-----------------------------------------------------------------------------------------
    public IEnumerator PrintInfo()
    {
        while (true)
        {
            string info = new StringBuilder().AppendFormat("电池：{0}, 时间：{1}, 网络：{2}",
                                         BatteryLevelStr, SystemTimeStr, NetWorkerStateStr).ToString();
            Debug.Log(info);
            yield return new WaitForSeconds(1f);
        }
    }


    


}
