using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

#region 模块信息
/*----------------------------------------------------------------
 * 模块名：EventTriggerListener
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：EventTriggerListener
 *----------------------------------------------------------------*/
#endregion
public class EventTriggerListener : EventTrigger
{

    static AudioSource audioSource;
    static Dictionary<String, AudioClip> audios = new Dictionary<string, AudioClip>();
    bool isPause = false;
    bool isFocus = false;

    public virtual void Awake()
    {
        RecursiveAddClickEvent(transform.gameObject, OnClick);
        RecursiveAddToggleEvent(transform.gameObject, OnToggle);
    }

    public virtual void OnEnable()
    {
        isPause = false;
        isFocus = false;
    }
    public virtual void OnClick()
    {
        //
    }
    public virtual void OnToggle(bool isOn)
    {
        //
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause  " + pause);
        if (pause)
        {
            isPause = true;
        }
    }

    void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus  " + focus);
        if (focus)
        {           
            isPause = false;
            ApplicationFocus();
        }
    }
    public virtual void ApplicationFocus()
    {
        //GameObject.Find("NativeCall").GetComponent<NativeCall>().BackToSystem();
    }
    public void AddClickEvent(GameObject target, UnityAction callback)
    {
        Debug.Log("AddClick Event");
        Button button = target.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(callback);
            button.onClick.AddListener(callback);
        }
    }
    private void RecursiveAddClickEvent(GameObject parent, UnityAction callback)
    {
        foreach (Transform child in parent.transform)
        {
            AddClickEvent(child.gameObject, callback);
            RecursiveAddClickEvent(child.gameObject, callback);
        }
    }

    public void AddToggleEvent(GameObject target, UnityAction<bool> callback)
    {
        Toggle toggle = target.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(callback);
            toggle.onValueChanged.AddListener(callback);
        }
    }

    private void RecursiveAddToggleEvent(GameObject parent, UnityAction<bool> callback)
    {
        foreach (Transform child in parent.transform)
        {
            AddToggleEvent(child.gameObject, callback);
            RecursiveAddToggleEvent(child.gameObject, callback);
        }
    }

}
