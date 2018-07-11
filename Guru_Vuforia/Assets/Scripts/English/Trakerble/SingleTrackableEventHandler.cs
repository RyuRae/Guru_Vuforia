using Content;
using FileTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTrackableEventHandler : DefaultTrackableEventHandler, IListenerTracker
{
    private GameObject model;
    private GameObject modelObj;
    private Renderer[] renderers;
    private Dictionary<Renderer, Color> cache;
    private Vector3 scaleValue;
    private GameObject video;
    private bool IsChange = false;
    private Camera arCamera;
    private GameObject child;
    private Transform alphabet;//字母表
    private Dictionary<string, GameObject> letters;
    private GameObject sceneObj;
    private float standard;//标准距离

    void Awake()
    {
        cache = new Dictionary<Renderer, Color>();
        letters = new Dictionary<string, GameObject>();
        sceneObj = GameObject.Find("SceneObj");
        alphabet = Global.FindChild<Transform>(sceneObj.transform, "alphabet");// GameObject.Find("SceneObj/alphabet").transform;
        standard = interval * 6;
        
    }

    private float interval = 0.03f;//两个字母之间的间隔
    private float scaletor;
    private AudioClip _clip;
    protected override void Start()
    {
        base.Start();
        Init();
        video = Global.FindChild(transform, "Video");
        if (video != null)
            video.SetActive(false);
    }

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
                    clone.transform.localRotation = Quaternion.Euler(-90, 180, 0);
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
                GameObject go = obj as GameObject;
                GameObject clone = Instantiate(go, model.transform);
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                clone.transform.localScale = Vector3.one;
            });
            model.SetActive(false);
            scaleValue = model.transform.localScale;
            model.transform.localPosition = new Vector3(0.8f, 1, 0);
        }

        AssetsManager.Instance.Load(RuntimeAssetType.AUDIO, transform.name.ToLower() + Tips.MP3, false, (clip) => {

            if (clip != null)
            {
                _clip = clip as AudioClip;
                _clip.name = transform.name.ToLower();
                //Debug.Log(_clip.name);
            }
        });
    }


    public Vector3 GetRelativePos()
    {
        return Vector3.zero;
    }

    public Vector3 GetViewportPos()
    {
        return Vector3.zero;
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
        if (model != null)
            model.transform.localScale = scaleValue;
    }

    public override void TrackingFoundEvent()
    {
        var currName = transform.name;
        if (currName.Contains("1"))
            currName = currName.Replace("1", "");
        //PlaySound(currName);
        if (model != null && !IsChange)
        {
            model.SetActive(true);
        }
    }

    void PlaySound(string name)
    {
        Bilingual playUnit = null;
        //获取卡牌类型
        //不是字母的话播放双语
        string pare = null;
        //先去小库（500个单词）里找解释
        if (ContentHelper.Instance.units.ContainsKey(name))
        {
            WordUnit unit = ContentHelper.Instance.units[name];
            pare = unit.Parephrase;
        }
        else//去词典里找解释
            pare = ContentHelper.Instance.GetPare(name);
        if (name.Contains("-"))
            name = name.Replace("-", " ");
        playUnit = new Bilingual(name, pare);
        //AudioManager.Instance.PlayBilingual(playUnit);
        playUnit.Play();
        //AudioManager.Instance.SetUnits(playUnit);
    }

    public override void TrackingLostEvent()
    {
        //CombineControl.Instance.RemoveListener(this);
        if (model != null)
        {
            model.SetActive(false);
            OnStatusReset();
        }
    }

    public void PlaySound()
    {
        if (_clip != null)
        {
            AudioManager.Instance.Play(_clip);
        }
    }

    public void Stop()
    {
        AudioManager.Instance.Stop();
    }
}
