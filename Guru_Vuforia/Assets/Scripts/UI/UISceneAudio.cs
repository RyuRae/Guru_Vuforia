using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneAudio : UIScene {

    public static UISceneAudio Instance;

    public Sprite pause;
    private UISceneWidget mButton_Play;
    private bool isPlaying;
    private Sprite play;//存储原来的图片
    private AudioSource source;
    [HideInInspector]
    public AudioClip[] clips;
    private AudioClip currClip;//当前的声音片段
    private float currLength;

    private Animator anim;
    void Awake()
    {
        Instance = this;
    }

    protected override void Start () {
        base.Start();
        mButton_Play = GetWidget("Button_Play");
        if (mButton_Play != null)
        {
            mButton_Play.OnMouseClick = ButtonPlayOnClick;
            anim = mButton_Play.GetComponent<Animator>();
            play = mButton_Play.GetComponent<Image>().sprite;
        }     
        //source = AudioManager.Instance.source;
    }

    private void ButtonPlayOnClick(UISceneWidget eventObj)
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.speed = 1;
            anim.Play("playSound");
        }
        AudioManager.Instance.OnClickEvent(OnStop);
        //OnPlay(eventObj);
    }

    private void OnPlay(UISceneWidget eventObj)
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            eventObj.GetComponent<Image>().sprite = pause;
            index = 0;
            StartCoroutine(AudioPlay(eventObj.GetComponent<Image>()));
        }
        else
        {
            eventObj.GetComponent<Image>().sprite = pause;
            StopCoroutine(AudioPlay(eventObj.GetComponent<Image>()));
            source.Pause();
        }
    }

    //当音频停止播放时
    private void OnStop()
    {
        Debug.Log("声音停止");
        if (anim != null)
        {
            anim.speed = 0;
            anim.enabled = false;
        }
        mButton_Play.GetComponent<Image>().sprite = play;
    }

    int index;
    IEnumerator AudioPlay(Image image)
    {
        if (currClip != clips[index])
            currClip = clips[index];
        while (index < clips.Length)
        {
            currLength = currClip.length;
            source.clip = currClip;
            source.Play();
            yield return new WaitForSeconds(currLength);
            index++;
            if (index < clips.Length)
                currClip = clips[index];
        }
        image.sprite = play;
        isPlaying = false;
    }

}
