using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour {

    AnimationEvent mEvent;//这是你要加的事件
    public AnimationClip Clip;//这是你要加特效事件的动画剪辑
    //public AnimationClip Idle;//默认动画\
    [Tooltip("播放动画事件的时间")]
    public float time;
    AudioSource sound;
    Animation anim;
    void Start()
    {
        anim = GetComponent<Animation>();
        sound = GetComponent<AudioSource>();
        AddEvent();
    }

    //这个方法来往Clip里添加动画事件（mEvent.time是事件触发时间）
    private void AddEvent()
    {
        mEvent = new AnimationEvent();
        mEvent.functionName = "AnimEventLaunch";
        mEvent.time = time;
        Clip.AddEvent(mEvent);
       
    }

    public void AnimEventLaunch()
    {
        if (!sound.isPlaying)
            sound.Play();
        StartCoroutine(ChangeClip());
    }

    IEnumerator ChangeClip()
    {
        yield return new WaitForSeconds(sound.clip.length);
        anim.CrossFade("Idle", 0.5f);
        anim.wrapMode = WrapMode.Loop;
    }

}
