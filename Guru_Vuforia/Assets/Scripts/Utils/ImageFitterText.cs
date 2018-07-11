using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#region 模块信息
/*----------------------------------------------------------------
 * 模块名：ImageFitterText
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.22
 * 模块描述：image自适应text文本框大小
 *----------------------------------------------------------------*/
#endregion
[System.Serializable]
public class ImageFitterText : MonoBehaviour {

    [Tooltip("目标Image，如果为空则从挂载Gameobject上查找Image组件")]
    public Image image;
    private RectTransform imageRectTrans;
    [Tooltip("根据Text的长度自动调整Image的宽度")]
    public Text targetText;
    private Vector2 textScale;
    [Tooltip("上一次Text的size")]
    protected Vector2 lastTextSize;
    [Tooltip("根据Button的长度自动调整Image的宽度")]
    public RectTransform targetBtn;
    [Tooltip("左右宽度，上下宽度")]
    public Vector2 sizeOffset = new Vector2(890, 50);
    [Tooltip("对Text使用建议的设置，比如设置TextAlign")]
    public bool useSuggestTextSetting = true;
    [Tooltip("计算时加入Text的Scale")]
    public bool useTextScale = true;

    void Start()
    {
        if (image == null)
        {
            image = transform.GetComponent<Image>();
        }
        if (image != null)
        {
            imageRectTrans = image.GetComponent<RectTransform>();
        }
        if (targetText == null)
        {
            targetText = transform.GetComponentInChildren<Text>();
        }
        if (targetText != null)
        {
            textScale = targetText.GetComponent<RectTransform>().localScale.ToVector2XY();
            lastTextSize = new Vector2(targetText.preferredWidth, targetText.preferredHeight);

            if (useSuggestTextSetting)
            {
                targetText.alignment = TextAnchor.MiddleCenter;
                targetText.horizontalOverflow = HorizontalWrapMode.Overflow;
                targetText.verticalOverflow = VerticalWrapMode.Overflow;
            }
            Refresh();
        }
        if (image == null || targetText == null)
        {
            Debug.LogErrorFormat("请检查，目标Text是否为null：{0},目标Image是否为null:{1}", targetText, image);
        }
    }
    /// <summary>
    /// 获取Text的实际size，计算结果含rectTransform的scale
    /// </summary>
    /// <returns></returns>
    /// 
    Vector2 GetTextPreferredSize()
    {
        if (targetText == null) return Vector2.zero;
        var size = new Vector2(targetText.preferredWidth, targetText.preferredHeight);
        if (useTextScale)
        {
            size = new Vector2(size.x * textScale.x, size.y * textScale.y);
        }
        return size;
    }
    Vector2 GetButtonPreferredSize()
    {
        if (targetBtn == null) return Vector2.zero;
        var size = targetBtn.sizeDelta;
        return size;
    }
    void UpdateImageSize(Vector2 Bsize, Vector2 size, Vector2 offset)
    {
        if (imageRectTrans != null)
        {
            imageRectTrans.sizeDelta= Bsize+ size + offset;
        }
    }

    public void Refresh()
    {
        UpdateImageSize(GetButtonPreferredSize(),GetTextPreferredSize(), sizeOffset * 2);
        lastTextSize = GetTextPreferredSize();
    }


    void Update()
    {
        Refresh();
        if (targetText != null && imageRectTrans != null)
        {
            if (lastTextSize != GetTextPreferredSize())
            {
                Refresh();
            }
        }
    }
}
