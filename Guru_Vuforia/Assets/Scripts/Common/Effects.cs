using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalAsset;
using System;
using Util;
using FileTools;

public class Effects : MonoSingleton<Effects> {

    private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
    private static List<GameObject> list = new List<GameObject>();
    private Action<Animation> animEvent;
    private bool isLoop = false;
    private Animation anim;
    void Update()
    {
        if (isLoop)
            animEvent(anim);
    }

    public void SetAnimEvent(Animation anim, Action<Animation> animEvent, bool isLoop = false)
    {
        this.isLoop = isLoop;
        this.anim = anim;
        this.animEvent = animEvent;

    }

    public void GetEffect(string name, Action<GameObject> action)
    {
        GameObject tempObj = null;
        if (!cache.ContainsKey(name))
        {
            AssetsManager.Instance.Load(RuntimeAssetType.BUNDLE_PREFAB, name, false, (obj) =>
            {
                tempObj = GameObject.Instantiate(obj as GameObject);
                cache.Add(name, tempObj);
                if (action != null)
                    action(cache[name]);
            });
        }
        else
        {
            if (action != null)
                action(cache[name]);
        }
       
        //GameObject obj = list.Find(p => !p.activeSelf);
        //return obj;
    }

    public void SetEffect(GameObject obj)
    {
        if (!list.Contains(obj))
            list.Add(obj);
    }

    public GameObject GetEffects(string name)
    {
        if (!cache.ContainsKey(name))
        {
            GameObject tempObj = null;
            AssetBundle bundle = LocalAssetManager.Instace.getBundle(Tips.BUNDLEPATH + "effects");
            if (bundle.Contains(name))
            {
                tempObj = GameObject.Instantiate(bundle.LoadAsset(name)) as GameObject;
                //tempObj.GetComponent<ParticleSystem>(). = false;
            }
            cache.Add(name, tempObj);
            bundle.Unload(false);
        }
        return cache[name];
    }


    public void callBack(GameObject obj,Action action, Action actionBack)
    {
        if (cache.ContainsValue(obj))
        {
            StartCoroutine(WaitDO(obj, obj.GetComponent<ParticleSystem>().main.duration, actionBack, action));           
            //obj.GetComponent<ParticleSystem>().main.duration
        }
    }

    /// <summary>
    /// 适用于播放特效
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="actionBack"></param>
    /// <param name="action"></param>
    public void WaitHandler(GameObject obj, Action actionBack, Action action = null)
    {
        StartCoroutine(WaitDO(obj, obj.GetComponent<ParticleSystem>().main.duration, actionBack, action));
    }

    IEnumerator WaitDO(GameObject obj, float time, Action actionBack, Action action = null)
    {
        if (action != null)
            action();
        yield return new WaitForSeconds(time);
        if (obj != null)
            obj.SetActive(false);
        actionBack();
    }

    public void Stop()
    {
        foreach (var obj in cache.Values)
        {
            if (!obj.GetComponent<Renderer>().enabled)
                obj.GetComponent<Renderer>().enabled = true;
        }
        StopAllCoroutines();
    }

    public void OnStatusChange(Action action, float time, Action callback)
    {
        StartCoroutine(OnChange(action, time, callback));
    }

    IEnumerator OnChange(Action action, float time, Action callback)
    {
        action();
        yield return new WaitForSeconds(time);
        callback();
    }

    public void OnClear()
    {
        isLoop = false;
        cache.Clear();
    }
}
