using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIName
{
    public const string UIAirMain = "UIAirMain";
    public const string UISceneStart = "UIScene_Start";
    public const string UISceneChoose = "UIScene_Choose";
    public const string UISceneHome = "UIScene_Home";
    public const string UISceneVideo = "UIScene_Video";
    public const string UISceneAudio = "UIScene_Audio";
    public const string UISceneHint = "UIScene_Hint";
    public const string UINumberChoose = "UINumber_Choose";
    public const string UISceneIntro = "UIScene_Intro";
    public const string UISceneRecord = "UIScene_Record";
    public const string UISceneMain = "UIScene_Main";
    public const string UISceneSet = "UIScene_Set";
    public const string UISceneTools = "UIScene_Tools";
    public const string UISceneProgress = "UIScene_Progress";
    public const string UISceneTest = "UIScene_Test";

    //public List<string> 
}
public class UIManager : MonoSingleton<UIManager>
{

    private Dictionary<string, UIScene> mUIScene = new Dictionary<string, UIScene>();
    //private Dictionary<UIAnchor.Side, GameObject> mUIAnchor = new Dictionary<UIAnchor.Side, GameObject>();
    private List<UIScene> lists = new List<UIScene>();

    public void InitializeUIs()
    {
        //mUIAnchor.Clear();
        //Object[] objs = FindObjectsOfType(typeof(UIAnchor));
        //if (objs != null)
        //{
        //    foreach (Object obj in objs)
        //    {
        //        UIAnchor uiAnchor = obj as UIAnchor;
        //        if (!mUIAnchor.ContainsKey(uiAnchor.side))
        //            mUIAnchor.Add(uiAnchor.side, uiAnchor.gameObject);
        //    }
        //}
        mUIScene.Clear();
        UIScene[] scenes = FindObjectsOfType<UIScene>();
        lists.AddRange(scenes);
        Object[] uis = FindObjectsOfType(typeof(UIScene));
       
        if (uis != null)
        {
            foreach (Object obj in uis)
            {
                UIScene ui = obj as UIScene;
                ui.SetVisible(false);
                mUIScene.Add(ui.gameObject.name, ui);
            }
        }
    }

    public void SetVisible(string name, bool visible)
    {
        if (visible && !IsVisible(name))
        {
            OpenScene(name);
        }
        else if (!visible && IsVisible(name))
        {
            CloseScene(name);
        }
    }

    public bool IsVisible(string name)
    {
        UIScene ui = GetUI(name);
        if (ui != null)
            return ui.IsVisible();
        return false;
    }
    private UIScene GetUI(string name)
    {
        UIScene ui;
        return mUIScene.TryGetValue(name, out ui) ? ui : null;
    }

    public T GetUI<T>(string name) where T : UIScene
    {
        return GetUI(name) as T;
    }

    private bool isLoaded(string name)
    {
        if (mUIScene.ContainsKey(name))
        {
            return true;
        }
        return false;
    }

    private void OpenScene(string name)
    {
        if (isLoaded(name))
        {
            mUIScene[name].SetVisible(true);
        }
    }
    private void CloseScene(string name)
    {
        if (isLoaded(name))
        {
            mUIScene[name].SetVisible(false);
        }
    }

    public List<UIScene> GetActiveScenes()
    {
        var scenes = lists.FindAll(p => p.gameObject.activeSelf);
        return scenes;
    }

    public void SetStartVisible(bool visible)
    {
        SetVisible(UIName.UIAirMain, visible);
        SetVisible(UIName.UISceneStart, visible);
        SetVisible(UIName.UISceneTest, visible);
        SetVisible(UIName.UISceneSet, visible);
        SetVisible(UIName.UISceneChoose, visible);
    }

    public void SetMainVisible(bool visible)
    {
        SetVisible(UIName.UISceneTools, visible);
        SetVisible(UIName.UISceneHome, visible);
    }

    private Dictionary<GameObject, System.Action> panelManage = new Dictionary<GameObject, System.Action>();
    /// <summary>注册二级面板</summary>
    /// <param name="panel">要注册的面板</param>
    /// <param name="isHaveParent">是否有父级</param>
    /// <param name="parent">父级名称</param>
    public void RegiSecondPanel(GameObject panel, System.Action action = null)
    {
        if (!panelManage.ContainsKey(panel))
            panelManage.Add(panel, action);
        else
            panelManage[panel] = action;
    }

    /// <summary>注销二级面板</summary>
    /// <param name="panel">需要注销的面板</param>
    public void UnRegiSecondPanel(GameObject panel)
    {
        if (panelManage.ContainsKey(panel))
            panelManage.Remove(panel);
    }

    /// <summary>
    /// 获取当前要执行的方法
    /// </summary>
    public System.Action GetCurrSecondPanel()
    {
        List<GameObject> list = new List<GameObject>(panelManage.Keys);
        List<GameObject> objs = list.FindAll(p => p.activeSelf);
        GameObject obj = null;
        if (objs.Count > 1)
            obj = objs.Find(p => p.transform.parent.name.Equals(UIName.UISceneHint));
        else
            obj = objs.Find(p => p.activeSelf);
        if (obj != null)
            return panelManage[obj];
        else
            return () => { Application.Quit(); };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GetCurrSecondPanel()();
    }
}
