using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：FeedbackUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：反馈UI界面
 *----------------------------------------------------------------*/
#endregion
public class FeedbackUIView : MonoBehaviour
{
    private static FeedbackUIView m_instance;
    public static FeedbackUIView Instance
    {
        get
        {
            return m_instance;
        }
    }

    Transform submit;
    Transform feedbackContent;
    Transform addImage;
    Transform phoneNum;
    Transform feedBackOption;

    public Action SUBMIT;
    public Action ADDIMAGE;

    private void Awake()
    {
        m_instance = this;
        feedBackOption = transform.Find("FeedBackOption");
        Initialize();
    }
    public void Initialize()
    {
        FeedbackUILogic.Instance.Initialize();    
        foreach(Transform child in feedBackOption)
        {
            if (child.GetComponent<Toggle>() != null)
            {
                Toggle toggle = child.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.onValueChanged.RemoveListener((bool value) => OnToggle(toggle, value));
                    toggle.onValueChanged.AddListener((bool value) => OnToggle(toggle, value));
                }
            }
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveListener(()=>OnClick(button));
                    button.onClick.AddListener(() => OnClick(button));
                }             
            }       
        }
    }
    public void Release()
    {
        FeedbackUILogic.Instance.Release();
    }

    void OnClick(Button button)
    {
        GameObject parent = GameObject.Find("ParentalLock/Panel/HelpFeedUI");
        Transform helpUI = parent.transform.Find("HelpUI");
        if (button.name == "Back")
        {
            if (!helpUI.gameObject.activeSelf)
            {
                helpUI.gameObject.SetActive(true);
            }
            transform.gameObject.SetActive(false);
        }
        else if (button.name == "AddImage")
        {
           // ADDIMAGE();
            Debug.LogError("AddImage");
        }
        else if (button.name == "Submit")
        {
           // SUBMIT();
            Debug.LogError("Submit");
        }
    }
    public void OnToggle(Toggle toggle, bool isOn)
    {
        if (isOn)
        {
            if (toggle.name == "Function")
            {
                //功能问题
                Debug.LogError("功能问题");
            }
            else 
            {
                //性能问题
                Debug.LogError("性能问题");
            }
           
        }
    }

}
