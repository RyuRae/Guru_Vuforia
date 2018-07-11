using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 文本类型
/// </summary>
public enum ContentType
{
    Getword = 1,
    Inthe = 2,
    Declarative,
    Query,
    Question,
    State,
    Video,
    Operation,
    Fruit,
    Iam,
    Areyou,
    Ihave,
    Ilike,
    Thisis,
    Iwant,
    Itis
}

/// <summary>文本内容</summary>
public interface IContent {

    void OnChanged(List<IListenerTracker> trackers);

    void OnReset(List<IListenerTracker> trackers);

    void Close();

}
