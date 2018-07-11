using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text;
using System;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：RegisterUIView
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：注册UI界面
 *----------------------------------------------------------------*/
#endregion
public class RegisterUIView : EventTriggerListener
{

    private static RegisterUIView m_instance;
    public static RegisterUIView Instance
    {
        get
        {
            return m_instance;
        }
    }

    InputField input;
    Image image;
    Text birth,place;
    Button submit;
    public Action SUBMIT;
    StringBuilder sb = new StringBuilder();
    public override void Awake()
    {
        base.Awake();
        m_instance = this;
        input = transform.Find("RegisterOption/Name/InputField").GetComponent<InputField>();
        birth = transform.Find("RegisterOption/Birth/LabelBirth").GetComponent<Text>();
        place = transform.Find("RegisterOption/Place/LabelPlace").GetComponent<Text>();
        image = transform.Find("ProfilePhoto/Mask/Photo").GetComponent<Image>();
        submit = transform.Find("RegisterOption/Submit").GetComponent<Button>();
        Initialize();
    }
    public override void OnClick()
    {
        base.OnClick();
        SUBMIT();
    }
    public void ShowName(string str)
    {
        sb=new StringBuilder(str);
        input.text = str;
    }
    public void ShowBirth(string str)
    {
        birth.text = str;
    }
    public void ShowPlace(string str)
    {
        place.text = str;
    }
    public void ShowPic(Texture2D tex)
    {
        image.sprite= Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        if (target.gameObject.GetComponent<Text>() != null)
        {
            if (target.name == "LabelPlace")
            {
                GameObject.Find("NativeCall").GetComponent<NativeCall>().ChooseAreaPopupWindow();
                eventData = null;
                return;
            }
            else if (target.name == "LabelBirth")
            {
                GameObject.Find("NativeCall").GetComponent<NativeCall>().ChooseBirthPopupWindow();
                eventData = null;
                return;
            }
            else
            {
               ///SSS
            }
        }else if (target.gameObject.GetComponent<Image>() != null)
        {
            if (target.name == "Photo")
            {
                GameObject.Find("NativeCall").GetComponent<NativeCall>().ChoosePhotoPopupWindow();
                eventData = null;
                return;
            }
        }
    }
  
    void Initialize()
    {
        if (submit != null)
        {
            submit.onClick.RemoveListener(OnClick);
            submit.onClick.AddListener(OnClick);
        }
        RegisterUILogic.Instance.Initialize();
    }
    public void Release()
    {
        RegisterUILogic.Instance.Release();
    }

}
