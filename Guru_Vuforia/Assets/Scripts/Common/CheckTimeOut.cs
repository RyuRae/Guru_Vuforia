using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /*********************
     * 模块信息：超时检测
     * 制作人：刘磊
     * 制作时间：2018.03.28
     * Copy right: 讯飞幻境（北京）科技有限公司
     * *******************/
    public class CheckTimeOut : MonoSingleton<CheckTimeOut>
    {

        private bool isEyeProtectionToneOn;//是否播放超时提示语音
        private int getEyeProtectionDuration;//语音提示时间
        private NativeCall native;
        private List<RecordTime> records;
        public RecordTime currRecord;//当前超时检测
        void Start()
        {
            records = new List<RecordTime>();
            native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
        }

        public bool IsEyeProtectionToneOn()
        {
            isEyeProtectionToneOn = native.IsEyeProtectionToneOn();
            return isEyeProtectionToneOn;
        }

        public int GetEyeProtectionDuration()
        {
            getEyeProtectionDuration = native.GetEyeProtectionDuration();
            return getEyeProtectionDuration;
        }
        public void RegiRecord()
        {
            if (currRecord != null && !records.Contains(currRecord))
                records.Add(currRecord);
        }

        public void RegiRecord(RecordTime record)
        {
            if (!records.Contains(record))
                records.Add(record);
        }
     
        public void UnRegiRecord()
        {
            if (currRecord != null && records.Contains(currRecord))
                records.Remove(currRecord);
        }

        public void UnRegiRecord(RecordTime record)
        {
            if (records.Contains(record))
                records.Remove(record);
        }

        void Refresh()
        {
            var it = records.GetEnumerator();
            while (it.MoveNext())
            {
                RecordTime rt = it.Current;
                // 更新
                rt.currTime += Time.deltaTime;
                //判断
                if (rt.onTrigger != null && rt.triggerTime != 0f)
                {
                    if (rt.currTime >= rt.triggerTime)
                    {
                        rt.onTrigger();
                        if (rt.isRepeat)
                            rt.currTime = 0f;
                        else
                            UnRegiRecord(it.Current);
                    }
                }
            }
        }

        void Update()
        {
            Refresh();
        }
    }
    public class RecordTime
    {
        public float currTime;
        public float triggerTime;
        public Action onTrigger;
        public bool isRepeat;

        public RecordTime()
        {
            currTime = 0f;
            isRepeat = false;
        }

        public RecordTime(bool isRepeat)
        {
            this.isRepeat = isRepeat;
        }

        public RecordTime(bool isRepeat, float triggerTime)
        {
            currTime = 0f;
            this.isRepeat = isRepeat;
            this.triggerTime = triggerTime;
        }

        public RecordTime(bool isRepeat, float triggerTime, Action onTrigger = null)
        {
            currTime = 0f;
            this.isRepeat = isRepeat;
            this.triggerTime = triggerTime;
            this.onTrigger = onTrigger;
        }

    }
}
