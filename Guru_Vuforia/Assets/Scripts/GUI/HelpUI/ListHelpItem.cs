using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIRScrollView
{
    public class ListHelpItem : MonoBehaviour
    {

        Text ItemQuestion;
        Text ItemAnswer;
        Button ExpandButton;
        GameObject ExpandContent;
        Image ButtonIcon;
        int ItemDataIndex = -1;
        bool IsExpand;

        void Awake()
        {
            ItemQuestion = transform.Find("ItemDesc").GetComponent<Text>();
            ItemAnswer = transform.Find("ExpandContent/content").GetComponent<Text>();
            ExpandButton = transform.Find("ExpandButton").GetComponent<Button>();
            ButtonIcon = transform.Find("ExpandButton/bg").GetComponent<Image>();
            ExpandContent = transform.Find("ExpandContent").gameObject;
        }

        public void Init()
        {
            ExpandButton.onClick.AddListener(OnClick);
        }
        public void OnExpandChanged()
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            if (IsExpand)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250f);
                ExpandContent.SetActive(true);
                ButtonIcon.sprite = Resources.Load<Sprite>("Icons/pickdown");
            }
            else
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 90f);
                ExpandContent.SetActive(false);
                ButtonIcon.sprite = Resources.Load<Sprite>("Icons/pickup");
            }

        }

        public void OnClick()
        {
            HelpInfoItem data = HelpUIView.Instance.GetItemDataByIndex(ItemDataIndex);
            if (data == null)
            {
                return;
            }
            IsExpand = !IsExpand;
            data.isExpand = IsExpand;
            OnExpandChanged();
            LoopListViewItem item = gameObject.GetComponent<LoopListViewItem>();
            item.ParentListView.OnItemSizeChanged(item.ItemIndex);
        }

        public void SetItemData(HelpInfoItem itemData, int itemIndex)
        {
            ItemDataIndex = itemIndex;
            ItemQuestion.text = itemData.question;
            ItemAnswer.text = itemData.answer;
            IsExpand = itemData.isExpand;
            OnExpandChanged();
        }
    }
}