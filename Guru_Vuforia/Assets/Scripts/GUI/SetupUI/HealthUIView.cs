using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：HealthUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.26
 * 模块描述：健康设置UI界面
 *----------------------------------------------------------------*/
#endregion
public class HealthUIView : MonoBehaviour {

    private static HealthUIView m_instance;
    public static HealthUIView Instance
    {
        get
        {
            return m_instance;
        }
    }

    Transform menu;
    Transform amount;
    Transform eyeshield;
    Transform back;

    public Action FIRSTVBL;
    public Action SECONDVBL;
    public Action THIRDVBL;

    public Action FIRSTMIN;
    public Action SECONDMIN;
    public Action THIRDMIN;
    private void Awake()
    {
        m_instance = this;
        back = transform.Find("Back");
        amount = transform.Find("Amount");
        eyeshield = transform.Find("Eyeshield");
        menu = transform.parent.Find("Menu");
        Initialize();
    }
    public void Initialize()
    {
        HealthUILogic.Instance.Initialize();
        Button button = back.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }
        foreach (Transform child in amount)
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
        foreach (Transform child in eyeshield)
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
    }
    public void Release()
    {
        HealthUILogic.Instance.Release();
    }

    public void OnToggle(Toggle toggle, bool isOn)
    {
        if (isOn)
        {
            switch (toggle.name)
            {
                case "FiveTog":
                    FIRSTVBL();
                    break;
                case "TenTog":
                    SECONDVBL();
                    break;
                case "FifTog":
                    THIRDVBL();
                    break;
                case "FiveMinTog":
                    FIRSTMIN();
                    break;
                case "FifMinTog":
                    SECONDMIN();
                    break;
                case "TwMinTog":
                    THIRDMIN();
                    break;
                default:
                    break;
            }
           
        }
    }

    void OnClick()
    {
        menu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
