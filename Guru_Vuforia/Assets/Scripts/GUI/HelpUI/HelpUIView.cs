using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AIRScrollView;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：HelpUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：帮助UI界面
 *----------------------------------------------------------------*/
#endregion
public class HelpUIView : MonoBehaviour
{

    private static HelpUIView m_instance;
    public static HelpUIView Instance
    {
        get
        {
            return m_instance;
        }
    }
    Transform submit;

    List<HelpInfoItem> helpInfoItem = new List<HelpInfoItem>();
    Action OnRefreshFinished = null;
    Action OnLoadMoreFinished = null;
    int loadMoreCount = 20;
    float dataLoadLeftTime = 0;
    bool isWaittingRefreshData = false;
    bool isWaitLoadingMoreData = false;
    public int totalDataCount = 6;

    public int TotalItemCount
    {
        get
        {
            return helpInfoItem.Count;
        }
    }
    private void Awake()
    {
        m_instance = this;
        submit = transform.Find("HelpUI/Submit");  
        Initialize();
    }
    private void Update()
    {
        if (isWaittingRefreshData)
        {
            dataLoadLeftTime -= Time.deltaTime;
            if (dataLoadLeftTime <= 0)
            {
                isWaittingRefreshData = false;
                RefreshDataSource();
                if (OnRefreshFinished != null)
                {
                    OnRefreshFinished();
                }
            }
        }
        else if (isWaitLoadingMoreData)
        {
            dataLoadLeftTime -= Time.deltaTime;
            if (dataLoadLeftTime <= 0)
            {
                isWaitLoadingMoreData = false;
                LoadMoreDataSource();
                if (OnLoadMoreFinished != null)
                {
                    OnLoadMoreFinished();
                }
            }
        }
    }
    void OnClick()
    {   
        Transform helpUI  = transform.Find("HelpUI");
        Transform feedbackUI =transform.Find("FeedbackUI");
        if(!feedbackUI.gameObject.activeSelf)
        {
            feedbackUI.gameObject.SetActive(true);
            if (feedbackUI.gameObject.GetComponent<FeedbackUIView>() == null)
                feedbackUI.gameObject.AddComponent<FeedbackUIView>();
        }
        helpUI.gameObject.SetActive(false);
    }
    public void Initialize()
    {
        RefreshDataSource();
        HelpUILogic.Instance.Initialize();     
        Button button = submit.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }
      
    }
    public void Release()
    {
        HelpUILogic.Instance.Release();
    }

    public HelpInfoItem GetItemDataByIndex(int index)
    {
        if (index < 0 || index >= helpInfoItem.Count)
            return null;
        else
            return helpInfoItem[index];
    }

    void RefreshDataSource()
    {
        helpInfoItem.Clear();
        for(int i=0;i<totalDataCount;i++)
        {
            HelpInfoItem data = new HelpInfoItem();
            data.id = i;
            data.question = "问题" + i;
            data.answer = "答案" + i;
            data.isExpand = false;
            helpInfoItem.Add(data);
        }
    }

    void LoadMoreDataSource()
    {
        int count = helpInfoItem.Count;
        for(int k=0;k<loadMoreCount;++k)
        {
            int j = k + count;
            HelpInfoItem data = new HelpInfoItem();
            data.id = j;
            data.question = "问题" + j;
            data.answer = "答案" + j;
            data.isExpand = false;
            helpInfoItem.Add(data);
        }
    }
    public void RequestRefreshDataList(Action onReflushFinished)
    {
        dataLoadLeftTime = 1;
        OnRefreshFinished = onReflushFinished;
        isWaittingRefreshData = true;
    }

    public void RequestLoadMoreDataList(int loadCount, Action onLoadMoreFinished)
    {
        loadMoreCount = loadCount;
        dataLoadLeftTime = 1;
        OnLoadMoreFinished = onLoadMoreFinished;
        isWaitLoadingMoreData = true;
    }

}

public class HelpInfoItem
{
    public int id;
    public string question;
    public string answer;
    public bool isExpand;
}