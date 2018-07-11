using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LocalAsset;

namespace Common
{
    /// <summary>
    /// 存储模型对象，需要模型去这里找
    /// </summary>
    public class GameobjectPool : MonoSingleton<GameobjectPool>
    {
        private Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        public GameObject GetObjectByName(string name)
        {
            if (!cache.ContainsKey(name))
            {
                GameObject tempObj = null;
                //先从持久化路径找
                //再从streaming里找
                Debug.Log(Tips.BUNDLEPATH + "/" + name);
                AssetBundle bundle = LocalAssetManager.Instace.getBundle(Tips.BUNDLEPATH + name);
                if (bundle.Contains(name))
                {
                    tempObj = Instantiate(bundle.LoadAsset(name)) as GameObject;
                    //卸载当前bundle
                    bundle.Unload(false);
                }
                cache.Add(name, tempObj);
                
            }
            return cache[name];
        }
        
    }
}
