using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Util
{
    public class Loom :MonoBehaviour
    {
        private const int MaxThreads = 10;
        static int NumThreads;
        private static bool _isInit = false;
        // 主线程运行函数的队列
        private List<Action> actions = new List<Action>();
        private List<DelayQueueAction> delayActions = new List<DelayQueueAction>();

        //主线程执行队列
        private List<Action> runActions = new List<Action>();
        private List<DelayQueueAction> delayRunActions = new List<DelayQueueAction>();

        private static Loom _current;
        public static Loom Current
        {
            get
            {
                Init();
                return _current;
            }
        }

        void OnDisable()
        {
            if (_current != null)
                _current = null;
            _isInit = false;
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;
            //清空本次运行的值
            lock (Current.actions)
            {
                Current.runActions.Clear();
                Current.runActions.AddRange(Current.actions);
                Current.actions.Clear();
            }
            Current.runActions.ForEach(a => a());
            lock (Current.delayActions)
            {
                Current.delayRunActions.Clear();
                Current.delayRunActions.AddRange(Current.delayActions.FindAll(a => a.time <= Time.time));
                Current.delayRunActions.ForEach(a => Current.delayActions.Remove(a));
            }
            Current.delayRunActions.ForEach(a => a.action());
        }

        public static void Init()
        {
            if (!_isInit)
            {
                _isInit = true;
                GameObject go = GameObject.Find("Loom");
                if (go == null)
                    go = new GameObject("Loom");
                DontDestroyOnLoad(go);
                if (!Application.isPlaying)
                    return;
                _current = go.GetComponent<Loom>();
                if (_current == null)
                    _current = go.AddComponent<Loom>();
            }
        }

        public struct DelayQueueAction
        {
            public float time;
            public Action action;
        }
        // 主线程执行
        public static void RunOnMainThread(Action action)
        {
            DelayRunOnMainThread(action, 0);
        }
        // 主线程在指定秒数内执行
        public static void DelayRunOnMainThread(Action action, int time)
        {
            if (time != 0)
            {
                lock (Current.delayActions)
                {
                    DelayQueueAction delayAction = new DelayQueueAction { action = action, time = Time.time + time };
                    Current.delayActions.Add(delayAction);
                }
            }
            else 
            {
                lock (Current.actions)
                {
                    Current.actions.Add(action);
                }
            }
        }

        public static void RunAsync(Action action)
        {
            Init();
            while (NumThreads >= MaxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref NumThreads);
            ThreadPool.QueueUserWorkItem(ThreadRun, action);
        }

        // 线程执行
        private static void ThreadRun(object obj)
        {
            try
            {
                ((Action)obj)();
            }
            finally
            {
                Interlocked.Decrement(ref NumThreads);
            }
        }

    }
}
