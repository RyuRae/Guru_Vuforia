using ServerAsset;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Tips {
    public const int MAX_LOAD_COUNT = 5;
    public static string PERSISPATH = Application.persistentDataPath + "/temp/";
    public static string SOUNDS = Application.persistentDataPath + "/sounds/";
    public static string STREAMINGPATH =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR   
        Application.streamingAssetsPath;
#elif UNITY_ANDROID
       Application.dataPath + "!assets";
#elif UNITY_IPHONE
        Application.dataPath + "Raw";
#endif
    public static string serverInfo = "serverInfo";
    public static string address = "address";
    public static string requestPath = "/file/download?isAIR=1&uuid=";
    public const string SERVER_FILE_PATH = @"http://192.168.8.150:8099/file/download?isAIR=1&uuid=";
    public const string SERVER_RESINFO_PATH = "ResInfo.xml";
    public static string LOCAL_RESINFO_PATH = PERSISPATH + "ResInfo.xml";
    public const string SERVER_SAAS_PATH = @"http://192.168.8.149:9999/air/list";
    public const string NAME = "name";
    public const string VERSION = "versionNum";
    public const string FILENAME = "ResInfo.xml";
    public static string LOCAL_FILE_LIST_PATH = Application.persistentDataPath + "/file_list.csv";
    public const string SERVER_BOOTSTRAP_PATH = "bootstrap.txt";
    public static string LOCAL_BOOTSTRAP_PATH = Application.persistentDataPath + "/bootstrap.txt";
    public static string LOCAL_FILE_PATH = Application.persistentDataPath + "/asset_file/";
    public const string BOOTSTRAP_NAME = "bootstrap";
    public static string WORD_LIB = "file:///" + Application.persistentDataPath + @"/release/configfiles/word_lib.csv";
    //#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    //        "file://" + Application.streamingAssetsPath + "/word_lib.csv";
    //#elif UNITY_ANDROID
    //        "jar:file://" + Application.dataPath + "!/assets/" + "word_lib.csv";
    //#endif


    public static string configPath = "file:///" + Application.persistentDataPath + @"/release/configfiles/config.csv";
    public static string DictPath = "file:///" + Application.persistentDataPath + @"/release/configfiles/dictionary.csv";
    public static string SentencePath = "file:///" + Application.persistentDataPath + @"/release/configfiles/sentence.csv";
    public static string BUNDLEPATH =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        Application.streamingAssetsPath + "/TestBundles/";
#elif UNITY_ANDROID
        Application.dataPath + "!assets"+ "/TestBundles/";
#endif
    //STREAMINGPATH + "/TestBundles/";

    //#if UNITY_ANDROID
    //    Application.dataPath + "!assets" + "/TestBundles/";
    //#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
    //        STREAMINGPATH + "/TestBundles/";
    //#elif UNITY_IPHONE
    //        Application.dataPath + "/Raw/" + "TestBundles/"; 
    //#endif

    public static string AppID = "10586537";
    public static string APIKey = "f2sIe4axsbqGc8WtsFLEw6Pr";
    public static string SecretKey = "4bc5210a0016b71f859c9d69cc3247dc";
    public static string MP3 = ".mp3";
    public static string WAV = ".wav";
    public static string PCM = ".pcm";
    public static string TAGS = "Tags";
    public static string BUTTONS = "Buttons";

    /// <summary>棕色</summary>
    public static Color brown = new Color(101f / 255, 66f / 255, 34f / 255f, 1);
    /// <summary>银色</summary>
    public static  Color silver = new Color(192f / 255, 192f / 255, 192f / 255f, 1);
    /// <summary>紫色</summary>
    public static Color purple = new Color(119f / 255, 23f / 255, 238f / 255, 1);
    /// <summary>粉色</summary>
    public static Color pink = new Color(253f / 255, 118f / 255, 215f / 255, 1);
    /// <summary>金色</summary>
    public static Color blond = new Color(1, 215f / 255, 43f / 255, 1);
}

public class AssetFileInfo
{
    public AssetFileInfo()
    {

    }

    public AssetFileInfo(string text)
    {
        string[] str = text.Split(',');

        nameId = str[0];
        version = str[1];
        path = str[2];
        fileType = str[3];

        if (str[4] == "t")
        {
            isLoad = true;
        }
        else
        {
            isLoad = false;
        }
    }

    public AssetFileInfo(SaasAir air)
    {
        nameId = air.Name;
        version = air.VersionNum;
        path = air.FilePath;
        fileType = AssetType.bytes.ToString();
        isLoad = air.IsDel == 1? true : false;
    }

    public string nameId { get; set; }
    public string version { get; set; }
    public string path { get; set; }
    public string fileType { get; set; }
    public bool isLoad { get; set; }

    private AssetType aType;
    public AssetType getFileType
    {
        get
        {
            switch (int.Parse(fileType))
            {
                case 0:
                    aType = AssetType.bootstrap;
                    break;
                case 1:
                    aType = AssetType.manifest;
                    break;
                case 2:
                    aType = AssetType.lesson_list;
                    break;
                case 3:
                    aType = AssetType.lesson;
                    break;
                case 4:
                    aType = AssetType.assetBundle;
                    break;
                case 5:
                    aType = AssetType.texture2D;
                    break;
                case 6:
                    aType = AssetType.text;
                    break;
                case 7:
                    aType = AssetType.audioClip;
                    break;
                case 8:
                    aType = AssetType.video;
                    break;
                case 9:
                    aType = AssetType.scene;
                    break;
                default:
                    aType = AssetType.bytes;
                    break;
            }

            return aType;
        }
    }


    public string getCsvText()
    {
        StringBuilder buffer = new StringBuilder();

        buffer.Append(nameId);
        buffer.Append(",");
        buffer.Append(version);
        buffer.Append(",");
        buffer.Append(path);
        buffer.Append(",");
        buffer.Append(fileType);
        buffer.Append(",");

        if (isLoad)
        {
            buffer.Append("t");
        }
        else
        {
            buffer.Append("f");
        }


        return buffer.ToString();
    }
}
