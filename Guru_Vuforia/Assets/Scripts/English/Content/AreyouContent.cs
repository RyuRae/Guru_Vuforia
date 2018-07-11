using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    public class AreyouContent : IContent
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
            //在卡牌放反的情况下，倒序排列
            if ((trackers[3].OnGetCurrentTransform().name.ToLower().Equals("are") && trackers[2].OnGetCurrentTransform().name.ToLower().Equals("you")) || (trackers[2].OnGetCurrentTransform().name.ToLower().Equals("you") && trackers[3].OnGetCurrentTransform().name.ToLower().Equals("are")))
                trackers.Sort((p, q) => q.GetRelativePos().x.CompareTo(p.GetRelativePos().x));
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
                mountPoint.transform.localPosition = new Vector3(1.6f, 2.3f, 0);
            }
            OnMarked(mountPoint, trackers);
        }

        //设置并变换标点符号
        private void OnMarked(Transform parent, List<IListenerTracker> trackers)
        {
            if (trackers[0].OnGetCurrentTransform().name.ToLower().Equals("you"))
            {
                //当期句子时陈述句
                if (question.activeSelf) question.SetActive(false);
                point.SetActive(true);
                point.GetComponentInChildren<MeshRenderer>().enabled = true;
                point.transform.SetParent(parent);

                //设置位置角度和缩放
                point.transform.localPosition = Vector3.zero;
                point.transform.localScale = Vector3.one;
                currObj = point;
            }
            else if (trackers[0].OnGetCurrentTransform().name.ToLower().Equals("are"))
            {
                //当前句子时问句
                if (point.activeSelf) point.SetActive(false);
                question.SetActive(true);
                question.GetComponentInChildren<MeshRenderer>().enabled = true;
                question.transform.SetParent(parent);

                //设置位置角度和缩放
                question.transform.localPosition = Vector3.zero;
                question.transform.localScale = Vector3.one;
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
        }
    }
}
