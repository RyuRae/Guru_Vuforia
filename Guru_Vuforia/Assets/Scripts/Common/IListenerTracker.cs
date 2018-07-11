using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackerType
{
    LETTER,//字母
    MODEL,//模型
    WORD,//单词
    VIDEO,//视频
    NUM,
    OPERATION,
    FRUIT,
    TOOL
}

public interface IListenerTracker {

    TrackerType Type { get; }

    void OnStatusReset();


    Transform OnGetCurrentTransform();

    Transform OnGetChildTransform();

    Vector3 GetRelativePos();

    Vector3 GetViewportPos();


}
