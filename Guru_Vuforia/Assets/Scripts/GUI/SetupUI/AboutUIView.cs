using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：AboutUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.26
 * 模块描述：关于我们UI界面
 *----------------------------------------------------------------*/
#endregion
public class AboutUIView : MonoBehaviour {

    private static AboutUIView m_instance;
    public static AboutUIView Instance
    {
        get
        {
            return m_instance;
        }
    }

    Transform back;
    Transform menu;
    private void Awake()
    {
        m_instance = this;
        back = transform.Find("Back");
        menu = transform.parent.Find("Menu");
        Initialize();
    }
    public void Initialize()
    {
        AboutUILogic.Instance.Initialize();
        Button button = back.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener( OnClick);
            button.onClick.AddListener(OnClick);
        }
    }
    public void Release()
    {
        AboutUILogic.Instance.Release();
    }
    void OnClick()
    {
        menu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
