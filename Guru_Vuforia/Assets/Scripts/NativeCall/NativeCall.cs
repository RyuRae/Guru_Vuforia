using UnityEngine;
using Newtonsoft.Json;
using System;
 
public enum KeyEvent
    {
        KEYCODE_DPAD_CENTER,
        KEYCODE_DPAD_UP,
        KEYCODE_DPAD_DOWN,
        KEYCODE_DPAD_DOWN_LEFT,
        KEYCODE_DPAD_RIGHT
    }
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：NativeCall
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：Unity与Android互调
 *----------------------------------------------------------------*/
#endregion

public class NativeCall : MonoBehaviour
{
    

    AndroidJavaObject currentActivity;
    ConvertToBase64 cb = null;
    AudioManager mAudio;
    //ARController arcontrol;

    public const int CONNECTIVITY_NONE = -1;
    public const int CONNECTIVITY_MOBILE = 0;
    public const int CONNECTIVITY_WLAN = 1;

    void Start()
    {       
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            cb = new ConvertToBase64();
            mAudio = AudioManager.Instance;
        }
        DontDestroyOnLoad(gameObject);
        BDTtsConfig bdtts = new BDTtsConfig(Tips.AppID, Tips.APIKey, Tips.SecretKey);
        string config = JsonUtility.ToJson(bdtts);
        InitBDTts(config);
        //arcontrol = GameObject.Find("SceneObj/ARToolKit").GetComponent<ARController>();
    }

    public void Logins()
    {
        if (currentActivity != null)
        {
            try
            {
                currentActivity.Call("startActivity", "LoginActivity");              
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public void Message(string str)
    {
        Debug.LogError("flyvrmsg" + str);
        if (currentActivity != null)
        {
            currentActivity.Call("showToast", str);
            Debug.LogError("flyvrmsg" + str);
        }
    }

    public void ShowSharePopupWindow()
    {
        string str = JsonConvert.SerializeObject(new ShareMessage("ar亲子乐园图片", "mmmmmm", "http://www.baidu.com", ""));
        currentActivity.Call("showSharePopupWindow", str);
        Debug.LogError("flyvrshow" + str);
    }
    public void ChooseBirthPopupWindow()
    {
        currentActivity.Call("showDatePickerPopupWindow");
    }
    public void ChoosePhotoPopupWindow()
    {
        currentActivity.Call("showPhotoPopupWindow");
    }
    public void ChooseAreaPopupWindow()
    {
        currentActivity.Call("showProvincePopupWindow");
        //currentActivity.Call("showAreaPickerPopupWindow");
    }
    public void OnDateSelected(string str)
    {
        GameObject.Find("RegisterUI").GetComponent<RegisterUIView>().ShowBirth(str);
    }
    public void UpdatePhoto(string str)
    {       
        var tex=cb.Base64ToTexture2d(str);
        GameObject.Find("RegisterUI").GetComponent<RegisterUIView>().ShowPic(tex);
    }
    public void OnAreaSelected(string str)
    {
        GameObject.Find("RegisterUI").GetComponent<RegisterUIView>().ShowPlace(str);
    }
    public void Login(string str)
    {
        if (currentActivity != null)
        {
            currentActivity.Call("showToast", str);
        }
    }

    /// <summary>
    /// 百度语音合成初始化
    /// </summary>
    public void InitBDTts(string config)
    {
        if (currentActivity != null)
            currentActivity.Call("initBDTts", config);
    }

    /// <summary>
    /// 语音合成（不播放）
    /// </summary>
    /// <param name="content">要合成的文本内容</param>
    public void Synthesize(string content)
    {
        if (currentActivity != null)
            currentActivity.Call("synthesize", content);
    }

    /// <summary>
    /// 语音合成（立即播放）
    /// </summary>
    /// <param name="content">合成内容</param>
    public void Speak(string content)
    {
        if (currentActivity != null)
            currentActivity.Call("speak", content);
    }

    /// <summary>
    /// 当合成或者播放过程中出错时回调此接口
    /// </summary>
    /// <param name="json">utteranceId和speechError(包含错误码和错误信息)</param>
    public void onError(string json)
    {
        BDTts value = JsonConvert.DeserializeObject<BDTts>(json);
        if (mAudio != null)
            mAudio.onError(value.utteranceId);
    }

    /// <summary>
    /// 合成数据和进度的回调接口，分多次回调。 注意：progress表示进度，与播放到哪个字无关
    /// </summary>
    /// <param name="json">utteranceId和字符串以及进度progress</param>
    public void onSynthesizeDataArrived(string json)
    {       
        BDTts value = JsonConvert.DeserializeObject<BDTts>(json);
        if (mAudio != null)
            mAudio.onSynthesizeDataArrived(value.utteranceId, Convert.FromBase64String(value.data), value.progress);
    }

    /// <summary>
    /// 播放进度回调接口，分多次回调 注意：progress表示进度，与播放到哪个字无关
    /// </summary>
    /// <param name="json">utteranceId和进度progress</param>
    public void onSpeechProgressChanged(string json)
    {

    }

    /// <summary>
    /// 播放正常结束，每句播放正常结束都会回调，如果过程中出错，则回调onError,不再回调此接口
    /// </summary>
    /// <param name="json">utteranceId</param>
    public void onSpeechFinish(string json)
    {

    }

    /// <summary>
    /// 合成正常结束，每句合成正常结束都会回调，如果过程中出错，则回调onError，不再回调此接口
    /// </summary>
    /// <param name="json">utteranceId</param>
    public void onSynthesizeFinish(string json)
    {
        //Debug.Log("onSynthesizeFinish: " + json);
        BDTts value = JsonConvert.DeserializeObject<BDTts>(json);
        if (mAudio != null)
            mAudio.onSynthesizeFinish(value.utteranceId);
    }

    /// <summary>
    /// 播放开始，每句播放开始都会回调
    /// </summary>
    /// <param name="json">utteranceId</param>
    public void onSynthesizeStart(string json)
    {
        //Debug.Log("onSynthesizeStart: " + json);
        BDTts value = JsonConvert.DeserializeObject<BDTts>(json);
        if (mAudio != null)
            mAudio.onSynthesizeStart(value.utteranceId);
    }

    private bool opened = false;
    /// <summary>
    /// 摄像头开启
    /// </summary>
    public void onStartPreview(string parm)
    {
        opened = true;
        //Debug.Log("摄像头开启");
        //UISceneTest.Instance.SetText("摄像头开启");
        //arcontrol.StartAR();
    }

    public void onStopPreview(string parm)
    {
        opened = false;
        //UISceneTest.Instance.SetText("摄像头关闭");
        //Debug.Log("摄像头关闭");
        //arcontrol.StopAR();
    }

    public void StartSetting()
    {
        currentActivity.Call("startSettingActivity");
    }

    public bool IsEyeProtectionToneOn()
    {
        if (currentActivity != null)
            return currentActivity.Call<bool>("isEyeProtectionToneOn");
        return true;
    }

    public int GetEyeProtectionDuration()
    {
        if (currentActivity != null)
            return currentActivity.Call<int>("getEyeProtectionDuration");
        return 0;
    }

    /// <summary> 获取当前摄像头连接状态</summary>
    /// <returns></returns>
    public bool GetCameraState()
    {
        //Debug.Log("摄像头是否开启：" + opened);
        return opened;
    }
    //public void OnKeyEvent(string command)
    //{
    //    KeyEvent key = (KeyEvent)Enum.Parse(typeof(KeyEvent), command);
    //    info.OnKeyEvent(key);
    //}

    /// <summary>
    /// 获取当前网络连接状态
    /// </summary>
    public int getConnectivityStatus()
    {
        int status = -1;
        if (currentActivity != null)
            status = currentActivity.Call<int>("getConnectivityStatus");
        return status;
    }

    /// <summary>
    /// 网络变化的回调
    /// </summary>
    /// <param name="connectivity"></param>
    public void onConnectivityChanged(string connectivity)
    {
        int status = Convert.ToInt32(connectivity);
        ShowConnectivityStatus(status);
    }

    private static void ShowConnectivityStatus(int status)
    {
        switch (status)
        {
            case CONNECTIVITY_NONE:
                Debug.Log("无网络连接，清先检查网络设置！");
                break;
            case CONNECTIVITY_MOBILE:
                Debug.Log("当前为移动网络连接。");
                break;
            case CONNECTIVITY_WLAN:
                Debug.Log("当前为wifi网络连接。");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 将图片加入手机相册
    /// </summary>
    /// <param name="path">图片路径</param>
    public void notifyNewMediaAdded(string path)
    {
        if (currentActivity != null)
            currentActivity.Call("notifyNewMediaAdded", path);
    }

    public void Finish()
    {
        if (currentActivity != null)
            currentActivity.Call("finish");
    }
}

public class LoginInfo
{
    public string uid;
    public string Account;
}
[System.Serializable]
public class ShareMessage
{
    public string title;
    public string summary;//描述
    public string url;
    public string bitmap;//图片

    public ShareMessage(string title, string summary, string url, string bitmap)
    {
        this.title = title;
        this.summary = summary;
        this.url = url;
        this.bitmap = bitmap;
    }   
}
