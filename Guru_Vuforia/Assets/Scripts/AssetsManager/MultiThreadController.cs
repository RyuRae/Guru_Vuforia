using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MultiThreadController : MonoSingleton<MultiThreadController>
{
    private List<ThreadWorker> workers;
    private List<Thread> threads;

    private void Awake()
    {
        workers = new List<ThreadWorker>();
        threads = new List<Thread>();
    }

    public void ClickStart()
    {
        ThreadWorker worker = new ThreadWorker();
        Thread workerThread = new Thread(worker.DoWork);
        workerThread.IsBackground = true;
        workerThread.Start();
        workers.Add(worker);
        threads.Add(workerThread);
    }

    private void OnApplicationQuit()
    {
        if (workers != null)
        {
            var it_workers = workers.GetEnumerator();
            while (it_workers.MoveNext())
            {
                it_workers.Current.ShouldStop = true;
            }
        }


        var it_threads = threads.GetEnumerator();
        while (it_threads.MoveNext())
        {
            it_threads.Current.Abort();
        }
    }
}

public class ThreadWorker
{
    private volatile bool _ShouldStop = false;
    public bool ShouldStop
    {
        get { return _ShouldStop; }
        set { _ShouldStop = value; }
    }

    public void DoWork()
    {
        while (!_ShouldStop)
        {
            Debug.Log(Thread.CurrentThread.ManagedThreadId + " worker thread: working...");
        }
        Debug.Log(Thread.CurrentThread.ManagedThreadId + " worker thread: terminating gracefully.");
    }

}
