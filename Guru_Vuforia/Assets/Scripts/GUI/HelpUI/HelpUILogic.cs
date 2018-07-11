using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIRScrollView;
public class HelpUILogic
{
    private static HelpUILogic m_instance;
    public static HelpUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new HelpUILogic();
            }

            return HelpUILogic.m_instance;
        }
    }

    Transform HelpInfoList;
    LoopListView loopListView;
    public void Initialize()
    {
        HelpInfoList =  GameObject.Find("ParentalLock/Panel/HelpFeedUI/HelpUI/HelpInfo/HelpInfoList").transform;
        loopListView = HelpInfoList.GetComponent<LoopListView>();
        loopListView.InitListView(HelpUIView.Instance.TotalItemCount, OnGetItemByIndex);
    }
    public void Release()
    {

    }
    LoopListViewItem OnGetItemByIndex(LoopListView listView, int index)
    {
        if (index < 0 || index >= HelpUIView.Instance.TotalItemCount)
        {
            return null;
        }

        HelpInfoItem itemData = HelpUIView.Instance.GetItemDataByIndex(index);
        if (itemData == null)
        {
            return null;
        }
        LoopListViewItem item = listView.NewListViewItem("InfoItem");
        ListHelpItem itemScript = item.GetComponent<ListHelpItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            itemScript.Init();
        }

        itemScript.SetItemData(itemData, index);
        return item;
    }
}
