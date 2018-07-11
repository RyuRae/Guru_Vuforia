using AIR.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace LocalAsset
{
    public class LocalAssetManager : MonoBehaviour
    {

        private static LocalAssetManager _instace = null;
        public static LocalAssetManager Instace
        {
            get
            {
                if (_instace == null)
                {
                    GameObject go = new GameObject("LocalAssetManager");
                    _instace = go.AddComponent<LocalAssetManager>();
                }

                return _instace;
            }
        }

        public AssetBundle getBundle(string path)
        {
            //string path = Application.persistentDataPath + "/" + bundleName;
            //if (!File.Exists(path))
            //{
            //    return null;
            //}

            //AssetBundle bundle = AssetBundle.LoadFromFile(path);
            return AssetBundle.LoadFromFile(path);
        }



        public Texture2D getTexture2D(string path, int textureWidth, int textureHeight)
        {
            try
            {
                //创建文件读取流
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                fileStream.Seek(0, SeekOrigin.Begin);
                //创建文件长度缓冲区
                byte[] bytes = new byte[fileStream.Length];
                //读取文件
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                //释放文件读取流
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;

                //创建Texture
                Texture2D texture = new Texture2D(textureWidth, textureHeight);
                texture.LoadImage(bytes);

                return texture;
            }
            catch (Exception e)
            {
                Debug.LogError(this.name + "  getTextrue2D: " + e.StackTrace);
                return null;
            }
        }

        public Sprite getSprite(Texture2D texture, Vector2 pivot)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);

            return sprite;
        }

        public Sprite getSprite(string path, int textureWidth, int textureHeight, Vector2 pivot)
        {
            Texture2D texture = getTexture2D(path, textureWidth, textureHeight);
            if (texture == null)
            {
                return null;
            }

            return getSprite(texture, pivot);
        }

        public void setText(string path, string str)
        {
            if (!File.Exists(path))
            {
                FileStream aFile = new FileStream(path, FileMode.OpenOrCreate);

                StreamWriter sw = new StreamWriter(aFile);
                sw.Write(str);
                sw.Close();
                sw.Dispose();
            }
            else
            {
                File.WriteAllText(path, str);
            }
        }

        public void saveFile(string fileName, byte[] bytes)
        {
            //如果没有这个文件夹 就创建一个
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }


            //FileStream stream = new FileStream(Application.persistentDataPath + "/" + fileName, FileMode.Create);
            FileStream stream = new FileStream(fileName, FileMode.Create);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }

        public void setXml(string path, string xml)
        {
            //写入xml文件

        }

        public string[] getText(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllLines(path);
            //return File.ReadAllText(path);
        }

        /// <summary>
        /// 可读取xml
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string getAllText(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }

        public void deleteFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

    }
}
