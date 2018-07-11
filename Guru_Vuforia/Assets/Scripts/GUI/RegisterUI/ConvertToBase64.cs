using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ConvertToBase64 
{


    public ConvertToBase64()
    {

    }
    //base64转图片
    public Texture2D Base64ToTexture2d(string Base64STR)
    {
        Texture2D pic = new Texture2D(100, 100);
        byte[] data = System.Convert.FromBase64String(Base64STR);
        pic.LoadImage(data);
        return pic;
    }
    //图片转base64string
    public string Texture2dToBase64(string texture2d_path)
    {
        FileStream fs = new System.IO.FileStream(texture2d_path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        byte[] thebytes = new byte[fs.Length];
        fs.Read(thebytes, 0, (int)fs.Length);
        string base64_texture2d = Convert.ToBase64String(thebytes);
        return base64_texture2d;
    }

}
