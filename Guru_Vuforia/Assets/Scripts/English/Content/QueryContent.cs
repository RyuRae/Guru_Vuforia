using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    /// <summary>
    /// 疑问句（can the）
    /// </summary>
    public class QueryContent : IContent
    {
        private List<string> anims = new List<string>();
        private bool isPlaying = false;
        private bool isBreak = false;
        private GameObject model;
        private GameObject point;//标点符号句号
        private GameObject question;//标点符号问号
        private Transform mountPoint;
        private Transform parent;
        public void OnChanged(List<IListenerTracker> trackers)
        {
            if (trackers[3].OnGetCurrentTransform().name.ToLower().Equals("can"))
                trackers.Sort((p, q) => q.GetViewportPos().x.CompareTo(p.GetViewportPos().x));
            if (trackers[2].OnGetChildTransform() == null) return;
            point = CombineControl.Instance.GetPoint();
            question = CombineControl.Instance.GetQuestion();
            parent = trackers[trackers.Count - 1].OnGetCurrentTransform();
            mountPoint = parent.Find("MountPoint");
            if (mountPoint == null)
            {
                GameObject tempGo = new GameObject("MountPoint");
                mountPoint = tempGo.transform;
                mountPoint.SetParent(parent);              
            }

            string animName = trackers[3].OnGetCurrentTransform().name;
            model = trackers[2].OnGetChildTransform().gameObject;
            if (!model.activeSelf)
                model.SetActive(true);
            var anim = trackers[2].OnGetCurrentTransform().GetComponentInChildren<Animation>();
            AnimControl(anim, animName);
            if (isBreak) return;

            //设置mountPoint的位置
            mountPoint.transform.localPosition = new Vector3(2f, 0, 0);
            mountPoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
            OnMarked(mountPoint);
            SoundPlay(trackers);
           
        }

        private void OnMarked(Transform parent)
        {
            if (question != null)
            {
                if (point.activeSelf) point.SetActive(false);
                question.SetActive(true);
                question.GetComponentInChildren<MeshRenderer>().enabled = true;
                question.transform.SetParent(parent);
                question.transform.localPosition = Vector3.zero;
                question.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                question.transform.localScale = Vector3.one;
            }
        }

        private void SoundPlay(List<IListenerTracker> trackers)
        {
            //判断此句型是否正确
            string content = null;
            for (int i = 0; i < trackers.Count; i++)
            {
                content += " " + trackers[i].OnGetCurrentTransform().name.ToLower();
            }

            string sentence = content.TrimStart();

            PlayUnit unit = new Statement(sentence,
                () =>
                {
                    UIManager.Instance.SetVisible(UIName.UISceneAudio, true);
                });
            AudioManager.Instance.SetUnits(unit);
        }

        public void AnimControl(Animation anim, string _name)
        {
            string animName = _name.Substring(0, 1).ToUpper() + _name.Substring(1);
            anims.Clear();
            if (anim == null)
            {
                UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                UISceneHint.Instance.ShowStatementHint("none");
                isBreak = true;
                return;
            }
            foreach (AnimationState state in anim)
            {
                anims.Add(state.name);
            }

            if (anims.Contains(animName))
            {
                ContentHelper.Instance.StartCoroutine(PlayAnim(anim, animName));
            }
            else
            {
                if (anims.Contains("Shake"))
                    ContentHelper.Instance.StartCoroutine(PlayAnim(anim, "Shake"));
                UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                UISceneHint.Instance.ShowStatementHint("noaction");
                isBreak = true;
            }
        }

        IEnumerator PlayAnim(Animation anim, string name)
        {
            AnimationClip clip = anim.GetClip(name);
            if (!isPlaying)
            {
                isPlaying = true;
                anim.CrossFade(name);
                yield return new WaitForSeconds(clip.length);
                anim.CrossFade("Idle");
            }
        }

        public void OnReset(List<IListenerTracker> trackers)
        {
            isPlaying = false;
            isBreak = false;
            if (point != null)
                point.SetActive(false);
            if (question != null)
                question.SetActive(false);
           
            AudioManager.Instance.PlayUnitReset();
            //ContentHelper.Instance.StopAllCoroutines()
            UIManager.Instance.SetVisible(UIName.UISceneAudio, false);
        }

        public void Close()
        {
            anims.Clear();
            if (model != null)
                GameObject.Destroy(model);
        }
    }
}
