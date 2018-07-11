using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterTrackableEventHandler : DefaultTrackableEventHandler, IListenerTracker
{
    private GameObject letter;
    private Vector3 InitPos;
    private Transform InitParent;
    private GameObject sceneObj;
    private Transform alphabet;//字母表

    void Awake()
    {
        letter = Global.FindChild(transform, "child");
        if (letter != null)
        {
            letter.transform.localPosition = Vector3.zero;
            InitPos = letter.transform.localPosition;
            InitParent = letter.transform.parent;
        }
        sceneObj = GameObject.Find("SceneObj");
        alphabet = Global.FindChild<Transform>(sceneObj.transform, "alphabet");
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    private void Init()
    {
        if (letter != null)
        {
            string transName = transform.name.ToLower();
            for (int i = 0; i < alphabet.childCount; i++)
            {
                if (alphabet.GetChild(i).name.Equals(transName))
                {
                    Transform clone = Instantiate(alphabet.GetChild(i), letter.transform);
                    clone.localPosition = new Vector3(0, 0, -0.075f);
                    clone.localRotation = Quaternion.Euler(-90, 180, 0);
                    clone.localScale = Vector3.one;
                    break;
                }
            }
            letter.transform.localPosition = Vector3.zero;
            letter.transform.localScale = Vector3.one * 8f;
        }

    }

    public override void TrackingFoundEvent()
    {
        //CombineControl.Instance.SetListener(this);//注册
    }

    public override void TrackingLostEvent()
    {
       
        //CombineControl.Instance.RemoveListener(this);//注销
        OnStatusReset();
    }

    public Transform OnGetChildTransform()
    {
        return letter.transform;
    }

    public Transform OnGetCurrentTransform()
    {
        return transform;
    }

    public void OnStatusReset()
    {
        if (letter != null && letter.activeInHierarchy && InitParent.gameObject.activeInHierarchy)
        {
            //if (!letter.GetComponentInChildren<Renderer>().enabled)
            //    letter.GetComponentInChildren<Renderer>().enabled = true;
            letter.transform.SetParent(InitParent);
            letter.transform.localPosition = InitPos;
            letter.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    
    public Vector3 GetRelativePos()
    {
        return Vector3.zero;
    }

    public Vector3 GetViewportPos()
    {
        return Vector3.zero;
    }
}
