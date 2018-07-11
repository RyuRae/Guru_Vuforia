using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    public enum AdjType
    {
        BIG,
        LITTLE,
        RED,
        YELLOW,
        BLUE,
        GREEN,
        GREY,
        WHITE,
        PURPLE,
        SILVER,
        BLACK,
        BROWN,
        PINK,
        BLOND
    }
    /// <summary>
    /// 陈述句（the  is）
    /// </summary>
    public class StateContent : IContent
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
            if (trackers[3].OnGetCurrentTransform().name.ToLower().Equals("the") && trackers[1].OnGetCurrentTransform().name.ToLower().Equals("is"))
                trackers.Sort((p, q) => q.GetViewportPos().x.CompareTo(p.GetViewportPos().x));
            if (trackers[1].OnGetChildTransform() == null || trackers[3].OnGetCurrentTransform().GetComponent<WordTrackableEventHandler>().wordType != WordType.adj) return;
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
            model = trackers[1].OnGetChildTransform().gameObject;
            if (!model.activeSelf)
                model.SetActive(true);
            OnStatusChange(model, adj);
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

        private void OnMarked(Transform parent)
        {
            if (point != null)
            {
                if (question.activeSelf) question.SetActive(false);
                point.SetActive(true);
                point.GetComponentInChildren<MeshRenderer>().enabled = true;
                point.transform.SetParent(parent);
                point.transform.localPosition = new Vector3(0, -0.005f, 0);
                point.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                point.transform.localScale = Vector3.one;
            }
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
                ContentHelper.Instance.StopCoroutine("ScaleObj");
                scaleValue = 1;
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
                ResetScale(resetValue);
                model.transform.parent.GetComponent<WordTrackableEventHandler>().ResetColor();
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
