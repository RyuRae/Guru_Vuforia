using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/****************************
 * AudioReset主要实现
 * 1.计时
 * 2.声音重置
 * 3.重复写入声音到AudioManager
 * *************************/

public class AudioReset : MonoSingleton<AudioReset> {

    List<PlayUnit> unitList = new List<PlayUnit>();
   

    private const float num = 10f;
    private float interval = num;//重复播放语音的时间间隔
	void Update ()
    {
        play();
    }

    private void play()
    {
        if (AudioManager.Instance.GetPlayQueStatus())
        {
            if (interval <= 0)
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    AudioManager.Instance.SetUnits(unitList[i]);
                }
                interval = num;
            }
            else
            {
                interval -= Time.deltaTime;
                for (int i = 0; i < unitList.Count; i++)
                {
                    unitList[i].IsDone = false;
                }
            }
        }
        else
            interval = num;
    }

    /// <summary>重置播放列表</summary>
    public void Reset()
    {
        unitList.Clear();
    }

    public void SetUnitList(PlayUnit unit)
    {
        unitList.Add(unit);
    }
}
