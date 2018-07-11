using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestContentSize : MonoBehaviour {

    GameObject text;
	void Start () {
        //ContentSizeFitter fitter = GetComponent<ContentSizeFitter>();
        //fitter.CallBack(delegate (Vector2 size) {
        //    Debug.Log("size =" + size);
        //});
        text = Resources.Load<GameObject>("Panel/content");
    }
    bool isclick = false;
    GameObject tempText;
   public void CreateText()
    {
        if (!isclick)
        {
            if (tempText == null)
            {
                tempText = Instantiate(text, transform.parent) ;
                tempText.GetComponent<Text>().text = "555555555555\n555555555555\n66666666666";
                tempText.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 60, tempText.GetComponent<Text>().preferredHeight);              
                transform.parent.GetComponent<ImageFitterText>().targetText = tempText.GetComponent<Text>();       
            }
            isclick = true;
        }
        else
        {
            if (tempText != null)
            {
                DestroyImmediate(tempText);
                transform.parent.GetComponent<ImageFitterText>().targetText = null;
            }
            isclick = false;
        }
    }
	void Update () {
		
	}
}
