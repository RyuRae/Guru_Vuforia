using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 界面控件基类
/// </summary>
public class UISceneWidget : MonoBehaviour, IDragHandler, IDropHandler, ISelectHandler, IPointerClickHandler,IPointerEnterHandler
{
    DateTime OnClickTime;
    public float Throughtime = 0.5f;
    /// - OnHover (isOver) 悬停，悬停时传入true，移出时传入false
    //public delegate void onMouseHover(UISceneWidget eventObj, bool isOver);
    //public onMouseHover OnMouseHover = null;
    //void OnHover(bool isOver)
    //{
    //    if (OnMouseHover != null) OnMouseHover(this, isOver);
    //}
    /// - OnPress （isDown）按下时传入true，抬起时传入false
    public delegate void onMousePress(UISceneWidget eventObj, bool isDown);
    public onMousePress OnMousePress = null;
    void OnPress(bool isDown)
    {
        if (OnMousePress != null) OnMousePress(this, isDown);
    }
    /// - OnSelect 相似单击，区别在于选中一次以后再选中将不再触发OnSelect
    public delegate void onMouseSelect(UISceneWidget eventObj, bool selected);
    public onMouseSelect OnMouseSelect = null;
    void OnSelect(bool selected)
    {
        if (OnMouseSelect != null) OnMouseSelect(this, selected);
    }

    public delegate void onMouseHover(UISceneWidget eventObj);
    public onMouseHover OnHover = null;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnHover != null) OnHover(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (OnMouseSelect != null) OnMouseSelect(this, eventData.used);
    }
    /// - OnClick 单击 Throughtime点击间隔时间
    public delegate void onMouseClick(UISceneWidget eventObj);
    public onMouseClick OnMouseClick = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Throughtime > (float)(DateTime.UtcNow - OnClickTime).TotalSeconds)
        {
            return;
        }
        OnClickTime = DateTime.UtcNow;
        if (Input.GetMouseButtonUp(0) && OnMouseClick != null) OnMouseClick(this);
    }

    //void OnClick()
    //{
    //    if (Throughtime > (float)(DateTime.UtcNow - OnClickTime).TotalSeconds)
    //    {
    //        return;
    //    }
    //    OnClickTime = DateTime.UtcNow;
    //    if (Input.GetMouseButtonUp(0) && OnMouseClick != null) OnMouseClick(this);
    //}
    /// - OnDoubleClick 双击（双击间隔小于0.25秒）时触发。
    public delegate void onMouseDoubleClick(UISceneWidget eventObj);
    public onMouseDoubleClick OnMouseDoubleClick = null;
    void OnDoubleClick()
    {
        if (OnMouseDoubleClick != null) OnMouseDoubleClick(this);
    }
    /// - OnDrag 按下并移动时触发，delta为传入的位移
    public delegate void onMouseDrag(UISceneWidget eventObj, Vector2 delta);
    public onMouseDrag OnMouseDrag = null;
    //void OnDrag(Vector2 delta)
    //{
    //    if (OnMouseDrag != null) OnMouseDrag(this, delta);
    //}
    public void OnDrag(PointerEventData eventData)
    {
        if (OnMouseDrag != null) OnMouseDrag(this, eventData.delta);
    }

    public delegate void onMouseDrop(UISceneWidget eventObj, GameObject dropObject);
    public onMouseDrop OnMouseDrop = null;
    //void OnDrop(GameObject dropObject)
    //{
    //    if (OnMouseDrop != null) OnMouseDrop(this, dropObject);
    //}

    public void OnDrop(PointerEventData eventData)
    {
        if (OnMouseDrop != null) OnMouseDrop(this, eventData.selectedObject);
    }

   
}
