using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*----------------------------------------------------------------
 * 模块名：ParentalLockMenuUILogic
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：家长锁菜单界面逻辑
 *----------------------------------------------------------------*/
public class ParentalLockMenuUILogic
{
    private static ParentalLockMenuUILogic m_instance;
    public static ParentalLockMenuUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ParentalLockMenuUILogic();
            }

            return ParentalLockMenuUILogic.m_instance;
        }
    }
    GameObject parent;
    GameObject RegisterUI;
    GameObject SetupUI;
    GameObject HelpFeedUI;
    
    public void Initialize()
    {
        parent = GameObject.Find("ParentalLock/Panel");
        RegisterUI = parent.transform.Find("RegisterUI").gameObject;
        SetupUI = parent.transform.Find("SetupUI").gameObject;
        HelpFeedUI = parent.transform.Find("HelpFeedUI").gameObject;
        ParentalLockMenuUIView.Instance.INFOMATION += Information;
        ParentalLockMenuUIView.Instance.SETUP += Setup;
        ParentalLockMenuUIView.Instance.HELPANDFEEDBACK += HelpAndFeedback;     

    }
    public void Release()
    {
        ParentalLockMenuUIView.Instance.INFOMATION -= Information;
        ParentalLockMenuUIView.Instance.SETUP -= Setup;
        ParentalLockMenuUIView.Instance.HELPANDFEEDBACK -= HelpAndFeedback;     
    }
    void Information()
    {      
        if(!RegisterUI.activeSelf)
        {
            RegisterUI.SetActive(true);
            SetupUI.SetActive(false);
            HelpFeedUI.SetActive(false);
            if (RegisterUI.GetComponent<RegisterUIView>() == null)
                RegisterUI.AddComponent<RegisterUIView>();
        } 
       
    }
    void Setup()
    {
        if(!SetupUI.activeSelf)
        {
            SetupUI.SetActive(true);
            RegisterUI.SetActive(false);
            HelpFeedUI.SetActive(false);
            if (SetupUI.GetComponent<SetupUIView>() == null)
                SetupUI.AddComponent<SetupUIView>();
        }
        
    }
    void HelpAndFeedback()
    {
        if (!HelpFeedUI.activeSelf)
        {
            HelpFeedUI.SetActive(true);
            RegisterUI.SetActive(false);
            SetupUI.SetActive(false);
            if (HelpFeedUI.GetComponent<HelpUIView>() == null)
                HelpFeedUI.AddComponent<HelpUIView>();
        }
    }
     

}
