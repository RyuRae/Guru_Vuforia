using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：SetupUILogic
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：设置UI界面逻辑
 *----------------------------------------------------------------*/
#endregion
public class SetupUILogic
{
    private static SetupUILogic m_instance;
    public static SetupUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SetupUILogic();
            }

            return SetupUILogic.m_instance;
        }
    }

    private Transform setup;
    public Transform menu;
    private Transform health;
    private Transform about;
    public void Initialize()
    {
        GameObject parent = GameObject.Find("SetupUI");
        setup = parent.transform;
        menu = setup.Find("Menu");
        health = setup.Find("SetHealth");
        about = setup.Find("SetAbout");
        SetupUIView.Instance.HEALTH += SetHealth;
        SetupUIView.Instance.CLEAR += SetClear;
        SetupUIView.Instance.SHARE += SetShare;
        SetupUIView.Instance.ABOUT += SetAbout;
        SetupUIView.Instance.QUIT += SetQuit;
         
    }
    public void Release()
    {
        SetupUIView.Instance.HEALTH -= SetHealth;
        SetupUIView.Instance.CLEAR -= SetClear;
        SetupUIView.Instance.SHARE -= SetShare;
        SetupUIView.Instance.ABOUT -= SetAbout;
        SetupUIView.Instance.QUIT -= SetQuit;
    }

    void SetHealth()
    {
        if(!IsActive(health))
        {
            health.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            about.gameObject.SetActive(false);
            if (health.GetComponent<HealthUIView>() == null)
            {
                health.gameObject.AddComponent<HealthUIView>();
            }
        }
        Debug.LogError("health");
    }
    void SetClear()
    {
        Debug.LogError("clear");
    }
    void SetShare()
    {
        if (Application.platform == RuntimePlatform.Android)
            GameObject.Find("NativeCall").GetComponent<NativeCall>().ShowSharePopupWindow();
        Debug.LogError("share");
    }
    void SetAbout()
    {
        if (!IsActive(about))
        {
            about.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            health.gameObject.SetActive(false);
            if (about.GetComponent<AboutUIView>() == null)
            {
                about.gameObject.AddComponent<AboutUIView>();
            }
        }
        Debug.LogError("about");
    }
    void SetQuit()
    {
        Debug.LogError("quit");
    }

    bool IsActive(Transform transform)
    {
        if (!transform.gameObject.activeSelf)
            return false;
        else
            return true;
    }
}
