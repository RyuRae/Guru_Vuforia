using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    public class ItisContent : IContent
    {
        private GameObject point;//标点符号句号
        private GameObject question;//标点符号问号
        private Transform mountPoint;//存放标点符号的点
        private Transform parent;
        private GameObject currObj;//当前显示的标点符号

        public void Close()
        {
            
        }

        public void OnChanged(List<IListenerTracker> trackers)
        {
            if ((trackers[3].OnGetCurrentTransform().name.ToLower().Equals("this") || trackers[3].OnGetCurrentTransform().name.ToLower().Equals("it")) && trackers[2].OnGetCurrentTransform().name.ToLower().Equals("is") && (trackers[1].OnGetCurrentTransform().name.ToLower().Equals("a") || trackers[1].OnGetCurrentTransform().name.ToLower().Equals("an"))
                    && trackers[0].OnGetCurrentTransform().GetComponent<WordTrackableEventHandler>().wordType == WordType.n)
                trackers.Sort((p, q) => q.GetViewportPos().x.CompareTo(p.GetViewportPos().x));

            var curr = trackers[3].OnGetCurrentTransform().name.ToLower();
            if (trackers[2].OnGetCurrentTransform().name.ToLower().Equals("a"))//判断a后是原音还是辅音
            {             
                if (curr.Equals("apple") || curr.Equals("orange") || curr.Equals("ice-cream"))
                {
                    //Debug.Log("元音开头的单词要用an哦！");
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    UISceneHint.Instance.ShowStatementHint("usean");
                    return;
                }
            }
            else
            {
                if (!curr.Equals("apple") && !curr.Equals("orange") && !curr.Equals("ice-cream"))
                {
                    //Debug.Log("辅音开头的单词要用a哦！");
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    UISceneHint.Instance.ShowStatementHint("usea");
                    return;
                }
            }

            SoundPlay(trackers);

            point = CombineControl.Instance.GetPoint();
            question = CombineControl.Instance.GetQuestion();
            parent = trackers[3].OnGetCurrentTransform();
            mountPoint = parent.Find("MountPoint");
            if (mountPoint == null)
            {
                GameObject tempGo = new GameObject("MountPoint");
                mountPoint = tempGo.transform;
                mountPoint.SetParent(parent);
                //设置mountPoint的位置
                mountPoint.transform.localPosition = new Vector3(2f, 0, 0);
                mountPoint.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            OnMarked(mountPoint, trackers);
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
            //在配置文件里查找此句型
            if (!ContentHelper.Instance.IsMatchWithSentencesConfig(sentence))
            {
                Debug.LogError("此句子(配置文件中)不存在!!!");
                return;
            }
            //AudioManager.Instance.PlaySound(content);
            PlayUnit unit = new Statement(sentence,
                () =>
                {
                    UIManager.Instance.SetVisible(UIName.UISceneAudio, true);
                });
            AudioManager.Instance.SetUnits(unit);
        }

        //设置并变换标点符号
        private void OnMarked(Transform parent, List<IListenerTracker> trackers)
        {
            if (trackers[0].OnGetCurrentTransform().name.ToLower().Equals("it") || trackers[0].OnGetCurrentTransform().name.ToLower().Equals("this"))
            {
                //当期句子时陈述句
                if (question.activeSelf) question.SetActive(false);
                point.SetActive(true);
                point.GetComponentInChildren<MeshRenderer>().enabled = true;
                point.transform.SetParent(parent);

                //设置位置角度和缩放
                point.transform.localPosition = Vector3.zero;
                point.transform.localScale = Vector3.one;
                point.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                currObj = point;
            }
            else if (trackers[0].OnGetCurrentTransform().name.ToLower().Equals("is"))
            {
                //当前句子时问句
                if (point.activeSelf) point.SetActive(false);
                question.SetActive(true);
                question.GetComponentInChildren<MeshRenderer>().enabled = true;
                question.transform.SetParent(parent);

                //设置位置角度和缩放
                question.transform.localPosition = Vector3.zero;
                question.transform.localScale = Vector3.one;
                question.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                currObj = question;
            }
        }

        public void OnReset(List<IListenerTracker> trackers)
        {
            if (currObj != null)
            {
                currObj.SetActive(false);
                currObj.transform.SetParent(null);
            }
         
            AudioManager.Instance.PlayUnitReset();
            //ContentHelper.Instance.StopAllCoroutines();
            UIManager.Instance.SetVisible(UIName.UISceneAudio, false);
        }
    }
}
