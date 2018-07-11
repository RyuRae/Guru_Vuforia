using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalAsset;
using System.Linq;
using FileTools;

namespace Content
{
    /// <summary>
    /// 字母组词
    /// </summary>
    public class GetwordContent : IContent/*, IDisposable*/
    {
        //private bool IsShow;//是否展示单词
       
        private Vector3 centerChild;//子物体中心点

        //private Vector3 center;//中心点

        private float cell = 1.5f;//两个字母之间的间隔

        private List<IListenerTracker> listeners;

        //private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();//缓存池，存储物体
       
        private GameObject showModel;
        private GameObject effect;//特效
        public void OnChanged(List<IListenerTracker> trackers)
        {
            if (listeners != null)
                OnReset(listeners);
            string word = "";
            for (int i = 0; i < trackers.Count; i++)
            {
                word += trackers[i].OnGetCurrentTransform().name;
            }
            string name = word.ToLower();
            if (name.Length > 1 && ContentHelper.Instance.FindWordInDictByName(name))
            {
                if (name.Length > 5)
                {
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    //UISceneHint.Instance.SetBullet("Image_bullet2");
                    UISceneHint.Instance.ShowStatementHint("long");
                }
                //展示单词

                //Vector3 sum = Vector3.zero;
                Vector3 sumChild = Vector3.zero;            
                for (int i = 0; i < trackers.Count; i++)
                {
                    //sum += trackers[i].GetRelativePos();                
                    sumChild += trackers[i].OnGetChildTransform().position;                   
                }
                parent = trackers[trackers.Count / 2].OnGetCurrentTransform();
                //center = sum / (float)trackers.Count;
                centerChild = sumChild / (float)trackers.Count;
                Effects.Instance.GetEffect("explode_model", (obj) => {

                    if (name.Length <= 5)
                    {
                        AudioManager.Instance.PlayUnitReset();//清空播放队列
                        for (int i = 0; i < trackers.Count; i++)
                        {
                            var child = trackers[i].OnGetChildTransform();
                            child.gameObject.SetActive(false);
                            string currName = trackers[i].OnGetCurrentTransform().name.ToLower();
                            AudioManager.Instance.SetUnits(new SingleTone(currName));
                        }
                    }
                    if (obj == null)
                    {
                        Debug.LogError("没有此特效，需要添加！");
                        return;
                    }
                    effect = obj;
                    if (!effect.activeSelf)
                        effect.SetActive(true);
                    effect.transform.position = centerChild;
                    effect.transform.SetParent(parent);
                    effect.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    Effects.Instance.WaitHandler(effect,
                     () =>
                     {                      
                         OnShow(name, centerChild, trackers);
                         if (name.Length <= 5)
                         { //播放声音
                             string pare = ContentHelper.Instance.GetPare(name);
                             PlayUnit unit = new Bilingual(name, pare);
                             AudioManager.Instance.SetUnits(unit);
                             //准备重复队列
                             AudioReset.Instance.Reset();
                             AudioReset.Instance.SetUnitList(unit);
                         }
                     });
                });
               
            }
            else//超过五张构不成单词
            {
                if (name.Length > 5)
                {
                    //AudioReset.Instance.Reset();
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    //UISceneHint.Instance.SetBullet("Image_bullet1");
                    UISceneHint.Instance.ShowStatementHint("help");
                    //Debug.LogError("当前字母数量已经大于5个");
                }
                else
                {
                    AudioReset.Instance.Reset();
                    for (int i = 0; i < name.Length; i++)
                    {
                        AudioReset.Instance.SetUnitList(new SingleTone(name[i].ToString()));
                    }
                }
            }

            listeners = trackers;
        }

        private void OnReseting(List<IListenerTracker> trackers)
        {
            //隐藏或者销毁模型
            if (showModel != null)
            {
                //隐藏时的特效
                //隐藏模型
                showModel.SetActive(false);
                showModel.transform.SetParent(null);
            }

            if (effect != null)
            {
                Effects.Instance.Stop();
                effect.SetActive(false);
                effect.transform.SetParent(null);
            }

            if (child != null && child.activeInHierarchy)
                child.transform.parent = null;
            if (go != null && go.activeInHierarchy)
                go.transform.parent = null;

            for (int i = 0; i < trackers.Count; i++)
            {
                trackers[i].OnStatusReset();
            }
        }

        Transform parent;
        GameObject child;
        GameObject go;
        //展示单词若有模型加载模型
        void OnShow(string name, Vector3 pos, List<IListenerTracker> trackers)
        {
            string childName = name + "Child";
            if(child == null)
                child = new GameObject(childName);

            child.transform.position = pos;
            for (int i = 0; i < trackers.Count; i++)
            {
                Transform _trans = trackers[i].OnGetChildTransform();
                if(!_trans.gameObject.activeSelf)
                    _trans.gameObject.SetActive(true);
                _trans.parent = child.transform;
                _trans.localRotation = Quaternion.Euler(Vector3.zero);
                if (!_trans.GetComponentInChildren<Renderer>().enabled)
                    _trans.GetComponentInChildren<Renderer>().enabled = true;
                float xpos = 0;
                xpos = trackers.Count % 2 != 0 ? (i - trackers.Count / 2) * cell : (i - trackers.Count / 2) * cell + cell / 2;
                _trans.localPosition = new Vector3(xpos, 0, 0);
            }
            //parent = trackers[trackers.Count / 2].OnGetCurrentTransform();
            child.transform.SetParent(parent);
            child.transform.localRotation = Quaternion.Euler(Vector3.zero);

            if (go == null)
                go = new GameObject(name);
            go.transform.position = pos;
            go.transform.SetParent(parent);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            #region 模型暂不做展示
            //if (ContentHelper.Instance.GetWordTypeByName(name))
            //    OnModelShow(name, go.transform);
            #endregion
        }

        private void OnModelShow(string name, Transform modelParent)
        {
            modelParent.transform.localPosition = new Vector3(modelParent.transform.localPosition.x, 0.4f, modelParent.transform.localPosition.z);
            if (!ContentHelper.Instance.GetModelVisible(name))
                return;

            //显示模型出现时的特效           
            Effects.Instance.GetEffect("explode_model", (obj) => {
                if (obj == null)
                {
                    Debug.LogError("没有此特效，需要添加！");
                    return;
                }
                effect = obj;
                if (!effect.activeSelf)
                    effect.SetActive(true);
                effect.transform.SetParent(modelParent);
                effect.transform.localPosition = Vector3.zero;

                Effects.Instance.WaitHandler(effect,
                 () =>
                 {
                     ContentHelper.Instance.GetModel(name + "_model", (modelObj) => {
                         if (modelObj == null)
                         {
                             Debug.LogError("找不到模型！！！");
                             return;
                         }
                         showModel = modelObj;
                         showModel.SetActive(true);
                         var renders = showModel.GetComponentsInChildren<Renderer>();
                         for (int i = 0; i < renders.Length; i++)
                         {
                             if (!renders[i].enabled)
                                 renders[i].enabled = true;
                         }
                         showModel.transform.SetParent(modelParent);
                         showModel.transform.localPosition = Vector3.zero;
                         showModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                     });
                    
                 });
            });
        }

        public void OnReset(List<IListenerTracker> trackers)
        {
            OnReseting(listeners);
            for (int i = 0; i < trackers.Count; i++)
            {
                var _child = trackers[i].OnGetChildTransform().gameObject;
                if (!_child.activeSelf)
                    _child.SetActive(true);
                trackers[i].OnStatusReset();
            }
            AudioReset.Instance.Reset();
            //go = null;
            //child = null;
            //Close();
        }

        public void Close()
        {
            if (showModel != null)
                GameObject.Destroy(showModel);
            if (effect != null)
                GameObject.Destroy(effect);
            if (listeners != null)
                listeners.Clear();
            if (go != null)
                GameObject.Destroy(go);
            if (child != null)
                GameObject.Destroy(child);
            if (parent != null)
                GameObject.Destroy(parent);
        }
    }
}
