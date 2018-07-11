using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    /// <summary>
    /// 问句（Is the）
    /// </summary>
    public class QuestionContent : IContent
    {
        private bool IsChange = false;
        private float resetValue = 1;
        private float scaleValue = 1;
        private GameObject model;
        private GameObject point;//标点符号句号
        private GameObject question;//标点符号问号
        private Transform mountPoint;
        private Transform parent;

        public void OnChanged(List<IListenerTracker> trackers)
        {
            if (trackers[3].OnGetCurrentTransform().name.ToLower().Equals("is"))
                trackers.Sort((p, q) => q.GetViewportPos().x.CompareTo(p.GetViewportPos().x));
            if (trackers[2].OnGetChildTransform() == null || trackers[3].OnGetCurrentTransform().GetComponent<WordTrackableEventHandler>().wordType != WordType.adj) return;
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
            //设置mountPoint的位置
            mountPoint.transform.localPosition = new Vector3(2f, 0, 0);
            mountPoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
            OnMarked(mountPoint);
            SoundPlay(trackers);
            string sub = trackers[3].OnGetCurrentTransform().name.ToUpper();
            AdjType adj = (AdjType)Enum.Parse(typeof(AdjType), sub);
            model = trackers[2].OnGetChildTransform().gameObject;
            if (!model.activeSelf)
                model.SetActive(true);
            OnStatusChange(model, adj);
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
            UIManager.Instance.SetVisible(UIName.UISceneAudio, true);
        }

        private void OnStatusChange(GameObject obj, AdjType type)
        {
            WordTrackableEventHandler track = obj.transform.parent.GetComponent<WordTrackableEventHandler>();
            switch (type)
            {
                case AdjType.BIG:
                    ChangeScale(obj, 1.5f);
                    resetValue = 1f / 1.5f;
                    break;
                case AdjType.LITTLE:
                    ChangeScale(obj, 1f / 1.5f);
                    resetValue = 1.5f;
                    break;
                case AdjType.RED:
                    track.ChangeColor(Color.red);
                    break;
                case AdjType.YELLOW:
                    track.ChangeColor(Color.yellow);
                    break;
                case AdjType.BLUE:
                    track.ChangeColor(Color.blue);
                    break;
                case AdjType.GREEN:
                    track.ChangeColor(Color.green);
                    break;
                case AdjType.BLACK:
                    track.ChangeColor(Color.black);
                    break;
                case AdjType.BLOND:
                    track.ChangeColor(Tips.blond);
                    break;
                case AdjType.BROWN:
                    track.ChangeColor(Tips.brown);
                    break;
                case AdjType.PINK:
                    track.ChangeColor(Tips.pink);
                    break;
                case AdjType.SILVER:
                    track.ChangeColor(Tips.silver);
                    break;
                case AdjType.PURPLE:
                    track.ChangeColor(Tips.purple);
                    break;
                case AdjType.WHITE:
                    track.ChangeColor(Color.white);
                    break;
                case AdjType.GREY:
                    track.ChangeColor(Color.grey);
                    break;
                default:
                    break;
            }
        }

        private void ChangeScale(GameObject obj, float value)
        {
            if (!IsChange)
            {
                ContentHelper.Instance.StartCoroutine(ScaleObj(obj, value));
                IsChange = true;
            }
        }

        public IEnumerator ScaleObj(GameObject obj, float target)
        {
            scaleValue *= target;
            scaleValue = Mathf.Clamp(scaleValue, 0.25f, 4);

            while (target > 1 ? obj.transform.localScale.x < scaleValue : obj.transform.localScale.x > scaleValue)
            {
                yield return new WaitForFixedUpdate();
                obj.transform.localScale = Vector3.MoveTowards(obj.transform.localScale, Vector3.one * scaleValue, Time.deltaTime * 5f);
            }

        }

        public void OnReset(List<IListenerTracker> trackers)
        {
            if (model != null && model.activeSelf)
            {
                model.transform.parent.GetComponent<WordTrackableEventHandler>().ResetColor();
                ResetScale(resetValue);
                scaleValue = 1;
            }
            if (point != null)
                point.SetActive(false);
            if (question != null)
                question.SetActive(false);
           
            AudioManager.Instance.PlayUnitReset();
            //ContentHelper.Instance.StopAllCoroutines();
            UIManager.Instance.SetVisible(UIName.UISceneAudio, false);
        }

        private void ResetScale(float value)
        {
            if (IsChange)
            {
                ContentHelper.Instance.StartCoroutine(ScaleObj(model, value));
                IsChange = false;
            }
        }

        public void Close()
        {
            if (model != null)
                GameObject.Destroy(model);
        }

    }
}
