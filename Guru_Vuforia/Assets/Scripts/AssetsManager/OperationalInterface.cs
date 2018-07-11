using FileTools;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OperationalInterface :  MonoSingleton<OperationalInterface>
{
    [HideInInspector]
    public bool isShowConsoleView = false;

    private void Awake()
    {
        AssetsManager assetsManager = GetComponent<AssetsManager>();
        if (assetsManager == null)
        {
            assetsManager = gameObject.AddComponent<AssetsManager>();
        }

        MultiThreadController multiThreadController = GetComponent<MultiThreadController>();
        if (multiThreadController == null)
        {
            multiThreadController = gameObject.AddComponent<MultiThreadController>();
        }
    }

    // ---------------------------------------------------------------------------------------------
    #region 测试

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(550, 50, 200, 30), "开启ConsoleView"))
    //    {
    //        isShowConsoleView = !isShowConsoleView;
    //    }

    //    if (GUI.Button(new Rect(800, 50, 200, 30), "多线程测试"))
    //    {
    //        MultiThreadController.Instance.ClickStart();
    //    }


    //    if (!AssetsManager.Instance.IsInitialized)
    //    {
    //        if (GUI.Button(new Rect(300, 50, 200, 30), "进行更新检查"))
    //        {
    //            CheckOnStart();
    //        }
    //    }
    //    else
    //    {
    //        if (GUI.Button(new Rect(50, 50, 200, 30), "TestLoadGameObject"))
    //        {
    //            TestLoadGameObject();
    //        }

    //        if (GUI.Button(new Rect(50, 90, 200, 30), "TestLoadAudio"))
    //        {
    //            TestLoadAudio();
    //        }

    //        if (GUI.Button(new Rect(50, 130, 200, 30), "TestLoadMultiGameObject"))
    //        {
    //            TestLoadMultiGameObject();
    //        }

    //        if (GUI.Button(new Rect(50, 170, 200, 30), "TestLoadImage"))
    //        {
    //            TestLoadTextrue();
    //        }

    //        if (GUI.Button(new Rect(50, 210, 200, 30), DeviceInfo.Instance.KeepGettingInfo ? "关闭 设备信息" : "开启 设备信息"))
    //        {
    //            TestGetDeviceInfo();
    //        }
    //    }
    //}

    public void CheckOnStart()
    {
        UpdateFilesManager.Instance.CheckAndUpdate(() =>
        {
            //SetWorldLib(() => 
            //{

            //});
        });
    }

    public void TestLoadGameObject()
    {
        AssetsManager.Instance.Load(RuntimeAssetType.BUNDLE_PREFAB, "zaria", false, (obj) =>
        {
            GameObject go = obj as GameObject;
            go.transform.position = new Vector3(0, 0, 2);
            go.transform.localScale = new Vector3(3,3,3);
            Instantiate(go);
        });
    }

    public void TestLoadMultiGameObject()
    {
        List<string> namesRange = new List<string> { "a_card", "rgd-5 green", "big_card", "blue_card", "c_card" };
        List<string> nameList = new List<string>();

        for (int i = 0; i < 500; i++)
        {
            int num = Random.Range(0, namesRange.Count);
            nameList.Add(namesRange[num]);
        }

        //初始化nameList
        AssetsManager.Instance.LoadMulti(RuntimeAssetType.BUNDLE_PREFAB, nameList, false, LoadMethod.BUNDLE_FILE, (obj) =>
        {
            if (obj != null)
            {
                GameObject go = obj as GameObject;
                Instantiate(go);
            }
            else
            {
                Debug.Log("加载失败");
            }

        }, () =>
        {
            Debug.Log("所有物体加载完毕！！！！！！！！！！！");
        });
    }

    public void TestLoadAudio()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        AssetsManager.Instance.Load(RuntimeAssetType.AUDIO, "Owl City - The Saltwater Room.mp3", true, (obj) =>
        {
            AudioClip ac = obj as AudioClip;
            audioSource.clip = ac;
            if (ac.loadState == AudioDataLoadState.Loaded)
            {
                audioSource.Play();
            }
        });
    }

    public void TestLoadTextrue()
    {
        AssetsManager.Instance.Load(RuntimeAssetType.TEXTRUE, "bule_47.png", true, (obj) =>
        {
            Texture2D txt2d = obj as Texture2D;
            Sprite sprite = Sprite.Create(txt2d, new Rect(0, 0, txt2d.width, txt2d.height), new Vector2(0.5f, 0.5f));
            UIController.Instance.testImage.sprite = sprite;
        });
    }

    public void TestLoadScene()
    {
        AssetsManager.Instance.Load(RuntimeAssetType.BUNDLE_SCENE, "test_sphere", false, (obj) =>
        {
            Debug.Log("场景切换完毕");
        });
    }

    public void TestGetDeviceInfo()
    {
        DeviceInfo.Instance.KeepGettingInfo = !DeviceInfo.Instance.KeepGettingInfo;
    }

    #endregion
    // ---------------------------------------------------------------------------------------------

    //private void SetWorldLib(Action onSetted)
    //{
    //    Content.WorldLib wordLib = new Content.WorldLib();
    //    if (wordLib != null)
    //    {
    //        LoadFile loadFile = wordLib.GetLoadFile((loaded) =>
    //        {
    //            if (onSetted != null) onSetted();
    //            //Debug.Log(wordLib.units.Count);
    //            //wordLib.PrintInfo();
    //        });
    //        LoadFileController.Instance.Load(loadFile);
    //    }
    //    else
    //    {
    //        if (onSetted != null) onSetted();
    //    }
    //}
}
