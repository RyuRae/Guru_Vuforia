using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//播放单元
public class PlayUnit
{
    private Action callBack;
    private bool isDone;

    /// <summary>
    /// 此播放操作是否完成
    /// </summary>
    public bool IsDone
    {
        get { return isDone; }
        set { isDone = value; }
    }

    public Action CallBack
    {
        get { return callBack; }
        set { callBack = value; }
    }

    public PlayUnit() { }

    public virtual void Play()
    {
    }
}


/// <summary>
/// 单音
/// </summary>
public class SingleTone : PlayUnit
{
    private string en;//英语

    /// <summary>
    /// 需要播放的英文
    /// </summary>
    public string EN { get { return en; } }

    public SingleTone()
    {
    }

    public SingleTone(string en)
    {
        this.en = en;
    }

    public SingleTone(string en, Action callBack)
    {
        this.en = en;
        CallBack = callBack;
    }

    public override void Play()
    {
        AudioManager.Instance.PlaySound(en, CallBack, this);
    }
}

/// <summary>
/// 双语
/// </summary>
public class Bilingual : PlayUnit
{
    private string en;//英语
    private string cn;//中文

    /// <summary>
    /// 需要播放的英文
    /// </summary>
    public string EN { get { return en; } }

    /// <summary>
    /// 需要播放的中文
    /// </summary>
    public string CN { get { return cn; } }

    public Bilingual()
    {
    }

    public Bilingual(string en, string cn)
    {
        this.en = en;
        this.cn = cn;
    }

    public Bilingual(string en, string cn, Action callBack)
    {
        this.en = en;
        this.cn = cn;
        CallBack = callBack;
    }

    public override void Play()
    {
        AudioManager.Instance.PlayBilingual(this, CallBack);
    }
}

/// <summary>
/// 语句
/// </summary>
public class Statement : PlayUnit
{
    private string sentence;//句子

    public string Sentence { get { return sentence; } }

    public Statement()
    {
    }

    public Statement(string sentence)
    {
        this.sentence = sentence;
    }

    public Statement(string sentence, Action callBack)
    {
        this.sentence = sentence;
        CallBack = callBack;
    }

    public override void Play()
    {
        AudioManager.Instance.PlaySentence(this, CallBack);
    }
}

