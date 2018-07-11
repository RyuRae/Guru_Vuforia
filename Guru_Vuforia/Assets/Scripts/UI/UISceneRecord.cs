using cn.sharerec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UISceneRecord : UIScene {

    public static UISceneRecord Instance;
    private Image signal;
    private Text text_Time;
    private Color _color = new Color(1, 1, 1, 100/255f);
    private float _time;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start () {
        base.Start();
        signal = Global.FindChild<Image>(transform, "Signal");
        text_Time = Global.FindChild<Text>(transform, "Text_Time");
        SetSignal();
        _time = Time.time;
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    /// <param name="text"></param>
    public void SetTime(string text)
    {
        text_Time.text = text;
    }

    /// <summary>
    /// 设置信号闪烁
    /// </summary>
    public void SetSignal()
    {
        signal.DOColor(_color, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }  
}
