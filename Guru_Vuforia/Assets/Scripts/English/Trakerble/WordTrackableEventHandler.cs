using Common;
using Content;
using FileTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


public class WordTrackableEventHandler : DefaultTrackableEventHandler, IListenerTracker
{
    private GameObject model;
    private Renderer[] renderers;
    private Dictionary<Renderer, Color> cache;
    private Vector3 scaleValue;
    private GameObject video;
    private bool IsChange = false;
    private GameObject child;
    private Transform alphabet;//字母表
    private Dictionary<string, GameObject> letters;
    private GameObject sceneObj;
    private float standard;//标准距离
    private GameObject cacheModel;

    void Awake()
    {
        cache = new Dictionary<Renderer, Color>();
        letters = new Dictionary<string, GameObject>();
        sceneObj = GameObject.Find("SceneObj");
        alphabet = Global.FindChild<Transform>(sceneObj.transform, "alphabet");// GameObject.Find("SceneObj/alphabet").transform;
        standard = interval * 4;
    }


    protected override void Start()
    {
        base.Start();
        Init();
        video = Global.FindChild(transform, "Video");
        if (video != null)
            video.SetActive(false);
    }

    private float interval = 0.03f;//两个字母之间的间隔
    private float scaletor;
    private void Init()
    {
        child = Global.FindChild(transform, "name");
        if (child != null)
        {

            for (int i = 0; i < alphabet.childCount; i++)
            {
                if (!letters.ContainsKey(alphabet.GetChild(i).name))
                    letters.Add(alphabet.GetChild(i).name, alphabet.GetChild(i).gameObject);
            }
            string transName = transform.name.ToLower();
            for (int i = 0; i < transName.Length; i++)
            {
                if (letters.ContainsKey(transName[i].ToString()))
                {
                    string letter = transName[i].ToString();
                    GameObject clone = Instantiate(letters[letter], child.transform);
                    float xpos = 0;
                    xpos = transName.Length % 2 != 0 ? (i - transName.Length / 2) * interval : (i - transName.Length / 2) * interval + interval / 2;
                    clone.transform.localPosition = new Vector3(xpos, 0, -0.075f);
                    clone.transform.localRotation = Quaternion.Euler(new Vector3(-90, 180, 0));
                    clone.transform.localScale = Vector3.one;
                }
            }
            float value = interval * (transName.Length - 1) - standard;
            scaletor = value > 0 ? standard / (interval * (transName.Length - 1)) : 1;
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one * 8f * scaletor;
        }

        model = Global.FindChild(transform, "model");
        if (model != null)//从模型库加载模型到model下
        {
            AssetsManager.Instance.Load(RuntimeAssetType.BUNDLE_PREFAB, transform.name.ToLower() + "_model", false, (obj) =>
            {
                if (obj != null)
                {
                    GameObject go = obj as GameObject;
                    GameObject clone = Instantiate(go, model.transform);
                    clone.transform.localPosition = Vector3.zero;
                    clone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    clone.transform.localScale = Vector3.one;
                }
            });
            model.SetActive(false);
            scaleValue = model.transform.localScale;
            model.transform.localPosition = Vector3.zero;
            cacheModel = Global.FindChild(transform, "cache");
            CacheInit();
        }
    }

    void CacheInit()
    {
        if (cacheModel == null)
            cacheModel = Global.FindChild(transform, "cache");
        if (cacheModel != null)
        {
            for (int i = 0; i < cacheModel.transform.childCount; i++)
            {
                cacheModel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        IsActive = false;
    }

    public Transform OnGetChildTransform()
    {
        if (model != null)
        {
            IsChange = true;
            return model.transform;
        }
        else
            return null;
    }

    public Transform OnGetCurrentTransform()
    {
        return transform;
    }

    public void OnStatusReset()
    {
        IsChange = false;
        ResetColor();
        if (model != null)
            model.transform.localScale = scaleValue;
    }

    public override void TrackingFoundEvent()
    {
        //CombineControl.Instance.SetListener(this);
        if (model != null && !IsChange)
        {
            model.SetActive(true);
            InitColor();
        }
    }

    private void InitColor()
    {
        renderers = model.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!cache.ContainsKey(renderers[i]))
                cache.Add(renderers[i], renderers[i].material.color);
        }
    }

    public override void TrackingLostEvent()
    {
        //CombineControl.Instance.RemoveListener(this);
        if (model != null)
        {
            model.SetActive(false);
            OnStatusReset();
        }
        CacheInit();
    }

    public void ChangeColor(Color _color)
    {
        if (renderers == null)
            InitColor();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = _color;
        }

    }

    //颜色的消失事件
    public void ResetColor()
    {
        if (renderers != null)
            for (int i = 0; i < renderers.Length; i++)
            {
                if (cache.ContainsKey(renderers[i]))
                    renderers[i].material.color = cache[renderers[i]];
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

    bool IsActive;
    string currName;

    public void SetCurrStatus(string status, bool visible)
    {
        currName = status;
        GameObject currObj = Global.FindChild(transform, status);
        if (currObj != null)
        {
            currObj.SetActive(visible);
            IsActive = visible;
        }
    }

    void OnApplicationQuit()
    {
        if (cache != null)
            cache.Clear();
    }
}
