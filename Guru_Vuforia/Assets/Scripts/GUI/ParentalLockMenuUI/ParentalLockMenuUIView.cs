using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AIR.Util;
using System;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：ParentalLockMenuUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：家长锁菜单界面
 *----------------------------------------------------------------*/
#endregion
public class ParentalLockMenuUIView : MonoBehaviour
{

    private static ParentalLockMenuUIView m_instance;
    public static ParentalLockMenuUIView Instance
    {
        get {       
                  return m_instance;
             }
    }
    public Action INFOMATION;
    public Action SETUP;
    public Action HELPANDFEEDBACK;

    Transform menu;
    Transform back;
    public  void Awake()
    {
        m_instance = this;
        menu = transform.Find("Menu");
        back=transform.Find("Back");
    
    }
    private void OnEnable()
    {
        Initialize();
    }
    private void OnDisable()
    {
        Release();
    }
    public  void OnClick()
    {
        GameObject.Find("ParentalLock/Panel").SetActive(false);
    }
    public void OnToggle(Toggle toggle, bool isOn)
    {     
        if(isOn)
        {
            if(toggle.name== "Information")
            {
                INFOMATION();
            }else if(toggle.name == "Setup")
            {
                SETUP();
            }
            else if (toggle.name == "HelpAndFeedback")
            {
                HELPANDFEEDBACK();
            }         
        }
    }
    void Initialize()
    {
        ParentalLockMenuUILogic.Instance.Initialize();
        Button button = back.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }
        foreach (Transform child in menu)
        {
            if (child.GetComponent<Toggle>() != null)
            {
                Toggle toggle = child.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.onValueChanged.RemoveListener((bool value) => OnToggle(toggle, value));
                    toggle.onValueChanged.AddListener((bool value)=>OnToggle(toggle,value));
                }
            }               
        }
    }
    public void Release()
    {
        ParentalLockMenuUILogic.Instance.Release();
        //RegisterUIView.Instance.Release();
        //SetupUIView.Instance.Release();
        //HelpUIView.Instance.Release();
    }

}
