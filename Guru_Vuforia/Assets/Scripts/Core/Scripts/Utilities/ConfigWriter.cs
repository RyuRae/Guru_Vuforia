using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class ConfigWriter
{

    public static void WriteDictionary(string path, string mainKey, string subKey, string text)
    {
        bool isExits = false;
        string[] ary = File.ReadAllLines(path, Encoding.Default);
        for (int i = 0; i < ary.Length; i++)
        {
            if (ary[i].Contains(mainKey))
            {
                Debug.Log("主键存在");
                isExits = true;
            }
            if (isExits && ary[i].StartsWith(subKey))
            {
                ary[i] = subKey + " = " + text;
                break;
            }
        }
        string str = string.Join("\r\n", ary);
        File.WriteAllText(path, str);

    }
}
