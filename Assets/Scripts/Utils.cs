using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    /// <summary>
    /// 读取Assets/Resources文件夹下的Json数据
    /// </summary>
    /// <param name="_path">路径，形如描述的是Resources文件夹下的相对路径，比如对于Resources/Data/Repo来说，就写Data/Repo</param>
    /// <returns>返回一个string类型，是读取进来的字符流</returns>
    public static string LoadJsonFromResources(string _path)
    {
        UnityEngine.TextAsset s = Resources.Load(_path) as TextAsset;
        return s.text;
    }
}
