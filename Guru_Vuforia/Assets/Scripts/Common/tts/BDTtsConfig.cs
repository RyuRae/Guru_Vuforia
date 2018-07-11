using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDTtsConfig
{
    public string appId;
    public string appKey;
    public string secretKey;

    public string speaker = "0";           // 设置在线发声音人： 0 普通女声（默认） 1 普通男声 2 特别男声 3 情感男声<度逍遥> 4 情感儿童声<度丫丫>
    public string volume = "5";            // 设置合成的音量，0-9 ，默认 5
    public string speed = "5";             // 速度，0-9 ，默认 5
    public string pitch = "5";             // 语调，0-9 ，默认 5

    public BDTtsConfig()
    {
    }

    public BDTtsConfig(string appId, string appKey, string secretKey)
    {
        this.appId = appId;
        this.appKey = appKey;
        this.secretKey = secretKey;
    }

    public BDTtsConfig(string appId, string appKey, string secretKey, string speaker)
    {
        this.appId = appId;
        this.appKey = appKey;
        this.secretKey = secretKey;
        this.speaker = speaker;
    }

    public BDTtsConfig(string appId, string appKey, string secretKey, string speaker, string volume, string speed, string pitch)
    {
        this.appId = appId;
        this.appKey = appKey;
        this.secretKey = secretKey;
        this.speaker = speaker;
        this.volume = volume;
        this.speed = speed;
        this.pitch = pitch;
    }
}

