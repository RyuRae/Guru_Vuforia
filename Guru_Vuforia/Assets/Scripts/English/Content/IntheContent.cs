using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    /// <summary>
    /// In the 句型
    /// </summary>
    public class IntheContent : IContent
    {
        private Vector3 center;//中心点
        //private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();//缓存池，存储物体
        private GameObject model;
        private GameObject moveObj;
        private GameObject effect;//特效
        private float speed = 3f;
        //private bool IsPlay = false;
        public void OnChanged(List<IListenerTracker> trackers)
        {
            if (trackers[3].OnGetCurrentTransform().name.ToLower().Equals("is") && trackers[1].OnGetCurrentTransform().name.ToLower().Equals("the"))
                trackers.Sort((p, q) => q.GetViewportPos().x.CompareTo(p.GetViewportPos().x));
            string name = null;
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < trackers.Count; i++)
            {
                name += trackers[i].OnGetCurrentTransform().name.ToLower();
                sum += trackers[i].GetRelativePos();
            }
            //ContentHelper.Instance.Load(name);
            center = sum / (float)trackers.Count;
            //model = ContentHelper.Instance.GetModelByName(name);
            SetChildVisible(trackers, false);
            OnShow(name, center, trackers);
        }

        private void SetChildVisible(List<IListenerTracker> trackers, bool visible)
        {
            //if (trackers.Count != 4) return;
            for (int i = 0; i < trackers.Count; i++)
            {
                if (trackers[i].OnGetChildTransform() != null)
                    trackers[i].OnGetChildTransform().gameObject.SetActive(visible);
            }
        }

        Animation anim;
        Transform parent;
        GameObject go;
        private void OnShow(string name, Vector3 pos, List<IListenerTracker> trackers)
        {
            if(go == null)
                go = new GameObject(name);
            go.transform.position = pos;
            parent = trackers[trackers.Count / 2].OnGetCurrentTransform();
            go.transform.SetParent(parent);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Effects.Instance.GetEffect("explode_model", (obj) => {
                if (obj == null)
                {
                    Debug.LogError("特效不存在！！！");
                    return;
                }
                effect = obj;
                if (!effect.activeSelf)
                    effect.SetActive(true);
                effect.transform.SetParent(go.transform);
                effect.transform.localPosition = Vector3.zero;
                Effects.Instance.WaitHandler(effect, () => {
                    ContentHelper.Instance.GetModel(name + "_model", (modelObj) =>
                    {
                        //显示模型
                        if (modelObj == null)
                        {
                            Debug.LogError("找不到模型！！！");
                            return;
                        }
                        model = modelObj;
                        model.SetActive(true);
                        model.transform.SetParent(go.transform);
                        model.transform.localPosition = Vector3.zero;
                        model.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        if (anim == null)
                        {
                            anim = model.GetComponentInChildren<Animation>();
                            moveObj = Global.FindChild(model.transform, "MoveObj");
                        }
                        //给物体位移，重复播放动画
                        //将需要位移的物体传过去，将动画时间传过去，将初始位置传过去
                        Effects.Instance.SetAnimEvent(anim, Movement, true);
                    });
                });
            });
     
        }

        private int index = 0;
        private List<string> animNames = new List<string>();
        private void Movement(Animation anim)
        {
            foreach (AnimationState state in anim)
            {
                if (!animNames.Contains(state.name))
                    animNames.Add(state.name);
            }
            animNames.Sort((p, q) => (Convert.ToInt32(p[p.Length - 1]).CompareTo(Convert.ToInt32(q[q.Length - 1]))));
            switch (index)
            {
                case 0:
                    Effects.Instance.OnStatusChange(
                        ()=> {
                            moveObj.transform.localPosition = Vector3.zero;
                            anim.Play(animNames[0]);
                        },anim.clip.length,
                        ()=> { index = 1;});
                    
                    break;
                case 1:
                    Effects.Instance.OnStatusChange(
                        ()=> {
                            anim.Play(animNames[1]);
                            moveObj.transform.Translate(Vector3.left * speed * Time.deltaTime);
                        }, anim.clip.length, 
                        ()=> { index = 0; });
                    break;
                default:
                    break;
            }
        }

        public void OnReset(List<IListenerTracker> trackers)
        {          
            if (model != null)
            {
                model.SetActive(false);
                model.transform.SetParent(null);
                var renders = model.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renders.Length; i++)
                {
                    if (!renders[i].enabled)
                        renders[i].enabled = true;
                }
                model = null;
                if (anim != null)
                {
                    anim.transform.localPosition = Vector3.zero;
                    anim.Stop();
                    anim = null;
                    moveObj = null;
                    animNames.Clear();
                    index = 0;
                    Effects.Instance.Stop();
                }
            }

            if (effect != null)
            {
                Effects.Instance.Stop();
                effect.SetActive(false);
                effect.transform.SetParent(null);
            }

            for (int i = 0; i < trackers.Count; i++)
            {
                trackers[i].OnStatusReset();
            }
            SetChildVisible(trackers, true);
            Effects.Instance.SetAnimEvent(null, null, false);
        }

        public void Close()
        {
            if (model != null)
                GameObject.Destroy(model);
            if (moveObj != null)
                GameObject.Destroy(moveObj);
            if (effect != null)
                GameObject.Destroy(effect);
        }

    }
}
