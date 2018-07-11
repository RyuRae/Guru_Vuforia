using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：SetupUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：设置UI界面逻辑
 *----------------------------------------------------------------*/
#endregion
public class SetupUIView : MonoBehaviour
{
    private static SetupUIView m_instance;
    public static SetupUIView Instance
    {
        get
        {      
            return m_instance;
        }
    }

    Transform menu;
    public Action HEALTH;
    public Action CLEAR;
    public Action SHARE;
    public Action ABOUT;
    public Action QUIT;
    private void Awake()
    {
        m_instance = this;
        menu = transform.Find("Menu");
        Initialize();
    }
    void OnClick(Button button)
    {
        switch (button.name)
        {
            case "Health":
                HEALTH();
                break;
            case "Clear":
                CLEAR();
                break;
            case "Share":
                SHARE();
                break;
            case "About":
                ABOUT();
                break;
            case "Quit":
                QUIT();
                break;
            default:
                break;

        }
    }
    public void Initialize()
    {
        SetupUILogic.Instance.Initialize();
        foreach (Transform child in menu)
        {
            if(child.GetComponent<Button>()!=null)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveListener(()=>OnClick(button));
                    button.onClick.AddListener(()=>OnClick(button));
                }
            }
        }
        
    }
    public void Release()
    {
        SetupUILogic.Instance.Release();
    }
}
