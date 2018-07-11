using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneTest : UIScene {

    public static UISceneTest Instance;
    private Text text_Test;

    void Awake()
    {
        Instance = this;
    }

	protected override void Start () {
        base.Start();
        text_Test = Global.FindChild<Text>(transform, "Text_Test");

    }

    public void SetText(string text)
    {
        if (text_Test != null)
            text_Test.text = text;
    }
}
