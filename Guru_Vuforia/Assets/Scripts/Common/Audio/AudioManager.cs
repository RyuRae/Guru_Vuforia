using Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class BDTts
{

    public string utteranceId;
    public string data;
    public int progress;


    public void setProgress(int progress)
    {
        this.progress = progress;
    }
}

public class AudioManager : MonoBehaviour {

    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }
    public AudioSource backMusic;
    public AudioSource source;
    private Dictionary<string, AudioClip> clips;
    private AudioClip[] audios;
      
    private NativeCall native;
    ///// <summary>音频合成初始化</summary>
    //public Action<string, string, string> InitBDTts;
 
    // 保存文件的目录
    private string destDir;
    private FileInfo fileInfo;
    private string fileName;

    //音频合成完成回调事件
    private Action callBack = null;
    private PlayUnit currUnit = null;
    void Awake()
    {
        instance = this;
        
    }
    List<AudioClip> systemAudios; 
	void Start () {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(backMusic.gameObject);
        var temp = Resources.LoadAll<AudioClip>("SysytemAudios/");
        systemAudios = new List<AudioClip>();
        systemAudios.AddRange(temp);
        clips = new Dictionary<string, AudioClip>();
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
    }

    public void Init()
    {
        //LoadSourceByResources();
        if (native != null)
        {
            BDTtsConfig bdtts = new BDTtsConfig(Tips.AppID, Tips.APIKey, Tips.SecretKey);
            string config = JsonUtility.ToJson(bdtts);
            native.InitBDTts(config);
        }
    }

    //通过resources加载声音资源
    private void LoadSourceByResources()
    {
        audios = Resources.LoadAll<AudioClip>("Pron/");
        for (int i = 0; i < audios.Length; i++)
        {
            if (!clips.ContainsKey(audios[i].name))
                clips.Add(audios[i].name, audios[i]);
        }
        //Debug.Log(audios.Length);
    }

    Queue<PlayUnit> queUnits = new Queue<PlayUnit>();
    private PlayUnit currPlay = null;
    void Update()
    {
        PlayByOrderInQueue();
    }

    // 按队列顺序播放声音
    private void PlayByOrderInQueue()
    {
        if (queUnits.Count > 0)
        {
            if (currPlay != queUnits.Peek())
            {
                currPlay = queUnits.Peek();
                currPlay.Play();
            }
            if (currPlay != null && currPlay.IsDone)
            {
                queUnits.Dequeue();
                currPlay = null;
            }
        }
    }

    /// <summary>
    /// 获取播放队列状态（重复播放的依据）
    /// </summary>
    public bool GetPlayQueStatus()
    {
        return queUnits.Count == 0 ? true : false;
    }

    /// <summary>向队列中塞入要播放的音频</summary>
    /// <param name="unit">要播放的音频</param>
    public void SetUnits(PlayUnit unit)
    {
        queUnits.Enqueue(unit);
    }

    public void PlayUnitReset()
    {
        if (queUnits != null && queUnits.Count > 0)
            queUnits.Clear();
    }

    /// <summary>
    /// 背景音乐重新播放
    /// </summary>
    public void Replay()
    {
        backMusic.volume = 1;
        backMusic.time = 0;
        backMusic.Play();
    }

    /// <summary>
    /// 背景音乐静音
    /// </summary>
    public void Mute()
    {
        backMusic.Pause();
        backMusic.volume = 0;
    }

    private string path;
    /// <summary>
    /// 点击卡牌左右播放声音
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    public void Play(AudioClip clip)
    {
        //string clipName = null;
        //if (ContentHelper.Instance.units.ContainsKey(parent))
        //{
        //    switch (name)
        //    {
        //        case "word":
        //            clipName = ContentHelper.Instance.units[parent].Word;
        //            break;
        //        case "paraphrase":
        //            clipName = ContentHelper.Instance.units[parent].Parephrase;
        //            break;
        //        default:
        //            break;
        //    }

        //}
        //if (!string.IsNullOrEmpty(clipName))
        //{
        //    fileName = clipName;
        //    PlaySound(clipName);
        //}

        StartCoroutine(PlayCallBack(clip));
    }

    //临时存储需要播放的声音
    Queue<string> queue = new Queue<string>();
    /// <summary>
    /// 通过名称播放声音
    /// </summary>
    public void PlaySound(string clipName, Action callback = null, PlayUnit unit = null)//string clipName, string content = null, Action callback = null, PlayUnit unit = null
    {
        if (!clips.ContainsKey(clipName))//如果clips不包含此音频，从本地加载，本地没有则合成
        {
            path = Tips.SOUNDS + clipName + Tips.WAV;
            if (File.Exists(path))//加载本地音频              
                StartCoroutine(LoadSound(path, clipName, callback, true, unit));
            else //先合成并播放音频，而后加载合成的音频
            {
                //Debug.Log("要合成了!");
                callBack = callback;
                currUnit = unit;
                Speak(clipName);
            }
        }
        else
            StartCoroutine(PlayCallBack(clips[clipName], callback, true, unit));
    }

    /// <summary>
    /// 播放双语
    /// </summary>
    public void PlayBilingual(Bilingual unit = null, Action callback = null)
    {
        StartCoroutine(PlayTwice(unit.EN, unit.CN, callback, unit));
    }

    IEnumerator PlayTwice(string en, string cn, Action callback = null, PlayUnit unit = null)
    {
        if (clips.ContainsKey(en))
        {
            //Debug.Log("播放英文（已存在）！！！");
            source.clip = clips[en];
            source.Play();
            yield return new WaitForSeconds(clips[en].length);
            StartCoroutine(PlayOther(cn, callback, unit));
        }
        else
        {
            //Debug.Log("播放英文！！！");
            PlaySound(en, () => {
                //Debug.Log("播放中文！！！");
                StartCoroutine(PlayOther(cn, callback, unit));
            });
        }
    }

    IEnumerator PlayOther(string cn, Action callback = null, PlayUnit unit = null)
    {
        yield return new WaitForSeconds(0f);
        if (clips.ContainsKey(cn))
        {
            source.clip = clips[cn];
            source.Play();
            yield return new WaitForSeconds(clips[cn].length);
            if (callback != null)
                callback();            
            if (unit != null)
                unit.IsDone = true;
        }
        else
            PlaySound(cn, callback, unit);
    }

    private string currQue = null;
    IEnumerator PlaySOundInQueue(Action callback)
    {
        //执行发音,直到队列为空
        while (queue.Count > 0)
        {
            string tempQue = queue.Peek();
            if (!tempQue.Equals(currQue))
            {
                currQue = tempQue;
                callback += () =>
                {
                    if (queue.Count > 0)
                        queue.Dequeue();
                    currQue = null;
                };
                if (!clips.ContainsKey(tempQue))//如果clips不包含此音频，从本地加载，本地没有则合成
                {
                    path = Tips.SOUNDS + tempQue + Tips.WAV;
                    if (File.Exists(path))//加载本地音频              
                        StartCoroutine(LoadSound(path, tempQue, callback));
                    else //先合成并播放音频，而后加载合成的音频
                    {
                        callBack = callback;
                        Speak(tempQue);
                    }
                }
                else
                    StartCoroutine(PlayCallBack(clips[tempQue], callback));
            }
            yield return null;
        }
    }

    public void Stop()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
    }

    IEnumerator PlayCallBack(AudioClip clip, Action callback = null, bool isPlay = true, PlayUnit unit = null)
    {
        if (isPlay)
        {
            source.clip = clip;
            source.Play();
        }
        yield return new WaitForSeconds(clip.length);     
        if (callback != null)
            callback();
        if (unit != null)
            unit.IsDone = true;
    }

    //加载声音，并根据参数确定是否播放
    IEnumerator LoadSound(string path, string name, Action callback = null, bool isPlay = true, PlayUnit unit = null)
    {
        string url = "file://" + path;
        //Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;
        var clip = www.GetAudioClip();
        clip.name = name;
        //Debug.Log("音频名称:" + clip.name);
        if (!clips.ContainsKey(clip.name))
            clips.Add(clip.name, clip);
        StartCoroutine(PlayCallBack(clip, callback, isPlay, unit));
    }

    List<AudioClip> sounds;
    public bool GetSoundState(string name, string text)
    {
        if (clips.ContainsKey(name))
            return true;
        var strs = text.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        sounds = new List<AudioClip>();
        for (int i = 0; i < strs.Length; i++)
        {
            if (clips.ContainsKey(strs[i].ToLower()))
                sounds.Add(clips[strs[i].ToLower()]);
            else
                return false;
        }
        UIManager.Instance.SetVisible(UIName.UISceneAudio, true);
        //UISceneAudio.Instance.clips = sounds.ToArray();
        return true;
    }

    private string sentence;
    /// <summary>播放句子</summary> 
    /// <param name="name">句子内容</param>
    public void PlaySentence(Statement unit = null, Action callback = null)
    {
        sentence = unit.Sentence;
        fileName = unit.Sentence;
        //string content = null;
        //if (sentence.Contains("_"))
        //    content = sentence.Replace('_', ' ');
        PlaySound(unit.Sentence, callback, unit);
    }

    //点击按钮播放句子
    public void OnClickEvent(Action callback = null)
    {      
        if (!string.IsNullOrEmpty(sentence))
        {
            //Debug.Log("sentence: " + sentence);
            if (clips.ContainsKey(sentence))
            {
                StartCoroutine(PlayCallBack(clips[sentence], callback));
            }
        }
    }

    //播放系统声音
    public void PlaySystem(string name, Action action = null)
    {
        var clip = systemAudios.Find(p => p.name.Equals(name));       
        if (clip != null)
        {
            PlayUnitReset();
            StartCoroutine(PlayCallBack(clip, action));
        }
    }

    /// <summary>
    /// 语音合成（不播放）
    /// </summary>
    /// <param name="content">合成的语音内容</param>
    public void Synthesize(string content)
    {
        if (native != null)
            native.Synthesize(content);
    }

    /// <summary>语音播放（边合成边播放）</summary>
    /// <param name="content">合成的内容</param>
    public void Speak(string content)
    {
        //Debug.Log("开始播放声音!!!");
        if (native != null)
            native.Speak(content);
    }

    private FileStream fs;
    private BinaryWriter binaryWriter;
    /// <summary>
    /// 播放开始，每句播放开始都会回调
    /// </summary>
    /// <param name="utteranceId"></param>
    public void onSynthesizeStart(string utteranceId)
    {
        //Debug.LogError("onSynthesizeStart: ");
        try
        {
            DirectoryInfo di = new DirectoryInfo(Tips.SOUNDS);
            if (!di.Exists) di.Create();
            CreateSoundFile(path);//创建音频文件并写入wav头
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    int count = 0; 
    /// <summary>
    /// 合成数据和进度的回调接口，分多次回调。 注意：progress表示进度，与播放到哪个字无关
    /// </summary>
    /// <param name="utteranceId"></param>
    /// <param name="data"></param>
    /// <param name="progress"></param>
    public void onSynthesizeDataArrived(string utteranceId, byte[] data, int progress)
    {
        try
        {
            //Debug.Log("当前长度： " + data.Length);
            count += data.Length;
            binaryWriter.Write(data, 0, data.Length);
        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
        }
        
    }

    /// <summary>
    /// 合成正常结束，每句合成正常结束都会回调，如果过程中出错，则回调onError，不再回调此接口
    /// </summary>
    /// <param name="utteranceId"></param>
    public void onSynthesizeFinish(string utteranceId)
    {
        OnFinshWrite();
        close();
        //加载音频
        StartCoroutine(LoadSound(path, fileName, callBack, false, currUnit));
    }

    //结束时整理文件流
    private void OnFinshWrite()
    {
        //Debug.Log("总长： " + count);
        binaryWriter.Seek(4, SeekOrigin.Begin);
        binaryWriter.Write(count + 36);   // 写文件长度  
        binaryWriter.Seek(40, SeekOrigin.Begin);
        binaryWriter.Write(count);      
    }

    /// <summary>
    ///  当合成或者播放过程中出错时回调此接口
    /// </summary>
    /// <param name="utteranceId"></param>
    public void onError(string utteranceId)
    {
        close();
        //删除当前合成的音频，以便重新合成
        if (path != null)
        {
            File.Delete(path);
            path = null;
        }
    }

    //关闭流
    private void close()
    {
        if (fs != null)
        {
            try
            {
                fs.Flush();
                fs.Close();
                fs = null;
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        count = 0;
    }

    //创建音频文件并添加wav格式头文件
    private void CreateSoundFile(string path)
    {

        try
        {
            fs = new FileStream(path, FileMode.Create);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            //mWaveFile = new FileStream(System.DateTime.Now.ToString("yyyyMMddHHmmss") + "test2.wav", FileMode.Create);  
        }

        binaryWriter = new BinaryWriter(fs);

        //Set up file with RIFF chunk info. 每个WAVE文件的头四个字节便是“RIFF”。  
        char[] ChunkRiff = { 'R', 'I', 'F', 'F' };
        char[] ChunkType = { 'W', 'A', 'V', 'E' };
        char[] ChunkFmt = { 'f', 'm', 't', ' ' };
        char[] ChunkData = { 'd', 'a', 't', 'a' };

        short shPad = 1;                // File padding  

        int nFormatChunkLength = 0x10; // Format chunk length.  

        int nLength = 0;                // File length, minus first 8 bytes of RIFF description. This will be filled in later.  

        short shBytesPerSample = 0;     // Bytes per sample.  

        short BitsPerSample = 16; //每个采样需要的bit数    

        //这里需要注意的是有的值是short类型，有的是int，如果错了，会导致占的字节长度过长or过短  
        short channels = 1;//声道数目，1-- 单声道；2-- 双声道  

        // 一个样本点的字节数目  
        shBytesPerSample = 2;

        // RIFF 块  
        binaryWriter.Write(ChunkRiff);
        binaryWriter.Write(nLength);
        binaryWriter.Write(ChunkType);

        // WAVE块  
        binaryWriter.Write(ChunkFmt);
        binaryWriter.Write(nFormatChunkLength);
        binaryWriter.Write(shPad);


        binaryWriter.Write(channels); // Mono,声道数目，1-- 单声道；2-- 双声道  
        binaryWriter.Write(16000);// 16KHz 采样频率                     
        binaryWriter.Write(32000); //每秒所需字节数  
        binaryWriter.Write(shBytesPerSample);//数据块对齐单位(每个采样需要的字节数)  
        binaryWriter.Write(BitsPerSample);  // 16Bit,每个采样需要的bit数    

        // 数据块  
        binaryWriter.Write(ChunkData);
        binaryWriter.Write((int)0);   // The sample length will be written in later.  
    }

    bool IsChange;
    public bool limit;
    float _time;//记录当前时间
    Vector3 currVec;//鼠标当前位置

    //void FixedUpdate()
    //{
    //    SetMuteByTime();

    //}

    //根据时间设置静音
    private void SetMuteByTime()
    {
        if (!limit)
        {
            if (Input.mousePosition != currVec)
            {
                //获取声音播放状态
                if (!backMusic.isPlaying)
                    Replay();
                _time = Time.time;//记录初始时间
                currVec = Input.mousePosition;
                IsChange = false;
            }
            else
            {
                if (!IsChange && Time.time - _time >= 5)
                {
                    Mute();
                    IsChange = true;
                }
            }
        }
    }
}
