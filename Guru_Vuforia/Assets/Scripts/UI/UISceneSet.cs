using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

public class UISceneSet : UIScene {

    private UISceneWidget mButton_Set;
    private NativeCall native;
    private bool IsLogin;
    //private GameObject parentalLock;
    //private GameObject unlock;
    protected override void Start () {
        base.Start();
        mButton_Set = GetWidget("Button_Set");
        if (mButton_Set != null)
            mButton_Set.OnMouseClick = ButtonSetOnClick;
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        //parentalLock = GameObject.Find("ParentalLock");
        //unlock = Global.FindChild(parentalLock.transform, "Unlock");
    }


    private void ButtonSetOnClick(UISceneWidget eventObj)
    {

        //显示家长锁提示
        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
        UISceneHint.Instance.ShowUnlockHint();
    }
}
