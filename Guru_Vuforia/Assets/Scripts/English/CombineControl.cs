using Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombineControl : MonoBehaviour {

    public static CombineControl Instance;
    //public GameObject arToolKit;
    //public GameObject arCamera;
    //public GameObject the;
    //public GameObject green;
    //事件监听器
    private List<IListenerTracker> listeners = new List<IListenerTracker>();

    private IContent content;

    private IContent currCont;

    private IContent lastCont;//上一个IContent
    private GameObject point;//标点符号句号
    private GameObject question;//标点符号问号

    //private List<ARTrackedObject> trackables;
    private delegate bool Handler(bool v);
    Handler action = null;
    void Awake()
    {
        Instance = this;
    }

    bool test;
    void Start()
    {
        point = Global.FindChild(transform, "Point");
        question = Global.FindChild(transform, "Question");
        //trackables = new List<ARTrackedObject>();
    }


    public void ARInit()
    {
        //var array = FindObjectsOfType<ARTrackedObject>();
        //trackables.AddRange(array);
    }

    /// <summary>
    /// 设置mark卡牌的显隐
    /// </summary>
    public void SetVisible(bool visible)
    {
        //trackables.ForEach(p => p.gameObject.SetActive(visible));
        listeners.Clear();
    }

    /// <summary>
    /// 获取点号
    /// </summary>
    public GameObject GetPoint()
    {
        return point;
    }

    /// <summary>
    /// 获取问号
    /// </summary>
    public GameObject GetQuestion()
    {
        return question;
    }


    //卡牌处理
    void OnExecute()
    {
        if (listeners.Count > 0)
        {
            //对放入的卡牌按实际顺序排序，按x轴排序
            listeners.Sort((p, q) => p.GetViewportPos().x.CompareTo(q.GetViewportPos().x));
            //处理排序后的卡牌
            TrackerHandler(listeners);
        }
    }

    private List<IListenerTracker> currTrackers;
    //处理所有卡牌，根据卡牌展示内容
    private void TrackerHandler(List<IListenerTracker> trackers)
    { 
        //TODO 获取句式类型（可以是字母组词，可以是单词组句）
        content = ContentFactory.CreateContent(trackers);

        //将卡牌都传过去根据卡牌的排序和具体类型表现相应的结果
        if (content != null)
        {
            content.OnChanged(trackers);
            lastCont = content;
            if (currCont != content)
            {
                currCont = content;
                currTrackers = trackers;
            }
        }
        else if (lastCont != null && trackers.Count == 5)
        {
            //Debug.Log("没有这个句子哦！");
            UIManager.Instance.SetVisible(UIName.UISceneHint, true);
            UISceneHint.Instance.ShowStatementHint("none");
            lastCont = null;
        }
    }

    public void OnReset()
    {
        //当前重置
        if (currCont != null)
        {
            currCont.OnReset(currTrackers);
            currCont = null;
        }
        UIManager.Instance.SetVisible(UIName.UISceneAudio, false);
    }

    /// <summary>卡牌物体的注册</summary>
    public void SetListener(IListenerTracker listener)
    {
        OnReset();
        if (OnCheck(listener))
            return;
        if (!listeners.Contains(listener))
            listeners.Add(listener);
        PlaySound(listener);
        OnExecute();     
    }

    /// <summary>播放单个语音</summary>
    private void PlaySound(IListenerTracker listener)
    {
        string name = null;
        name = listener.OnGetCurrentTransform().name.ToLower();
        PlayUnit playUnit = null;
        //获取卡牌类型
        //不是字母的话播放双语
        if (listener.Type != TrackerType.LETTER)
        {
            string pare = null;
            //先去小库（500个单词）里找解释
            if (ContentHelper.Instance.units.ContainsKey(name))
            {
                WordUnit unit = ContentHelper.Instance.units[name];
                pare = unit.Parephrase;
            }
            else//去词典里找解释
                pare = ContentHelper.Instance.GetPare(name);
            playUnit = new Bilingual(name, pare);
        }
        else//只播放声音（非双语）
            playUnit = new SingleTone(name);
        AudioManager.Instance.SetUnits(playUnit);
    }

    /// <summary>卡牌物体的注销</summary>
    public void RemoveListener(IListenerTracker listener)
    {      
        if (listeners.Contains(listener))
            listeners.Remove(listener);
        OnReset();
        OnExecute();
    }

    //检测卡牌是否属于此场景
    private bool OnCheck(IListenerTracker listener)
    {
        if (listener.Type == TrackerType.NUM)//或者AR小舞台的卡牌
        {
            //显示场景提示提示
            Debug.Log("请在数学场景使用此卡牌!");
            return true;
        }
        return false;
    }

    public void OnClear()
    {
        ContentHelper.Instance.OnClear();
        Effects.Instance.OnClear();
        //CancelInvoke("OnReset");
        CancelInvoke();
    }

    public void OnStarted()
    {
        Effects.Instance.CancelInvoke("callBack");
        Effects.Instance.StopAllCoroutines();
        
    }

    void OnApplicationQuit()
    {
        content = null;
        currCont = null;
        //停止所有协程
        StopAllCoroutines();
        OnClear();
        //trackables.Clear();
    }

    //void OnDestroy()
    //{
    //    if (UISceneHome.Instance != null)
    //    {
    //        UISceneHome.Instance.OnClear -= OnClear;
    //        UISceneHome.Instance.OnStarted -= OnStarted;
    //    }
    //    CancelInvoke();
    //}

}
