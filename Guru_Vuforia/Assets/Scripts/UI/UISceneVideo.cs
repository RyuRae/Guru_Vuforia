using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UISceneVideo : UIScene {

    public Sprite play;
    UISceneWidget mButton_Play;
    bool isPlaying;
    private Sprite pause;//存储原来的图片
    private static VideoPlayer player;//视频播放器
	
	protected override void Start () {
        base.Start();
        mButton_Play = GetWidget("Button_Play");
        if (mButton_Play != null)
            mButton_Play.OnMouseClick = ButtonPlayOnClick;
        pause = mButton_Play.GetComponent<Image>().sprite;
        //player = FindObjectOfType<VideoPlayer>();
    }

    private void ButtonPlayOnClick(UISceneWidget evevtObj)
    {      
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            evevtObj.GetComponent<Image>().sprite = play;
            player.Pause();
        }
        else
        {
            evevtObj.GetComponent<Image>().sprite = pause;
            player.Play();
        }
    }

    /// <summary>初始化播放器</summary>
    public static void InitVideo(VideoPlayer videoPlayer)
    {
        player = videoPlayer;
    }

}
