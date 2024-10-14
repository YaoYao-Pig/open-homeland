using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        //Debug.Log(_path);
        UnityEngine.TextAsset s = Resources.Load(_path) as TextAsset;
        return s.text;
    }



    public static List<string> GetFles(string _path)
    {
        string[] _files = Directory.GetFiles(_path, "*.json");
        List<string> files = new List<string>();

        foreach (var f in _files)
        {
            string directory = Path.GetDirectoryName(f);
            string fileName = Path.GetFileName(f); // 获取文件或目录名
            fileName = fileName.Split(".")[0];
            // 分割路径
            string[] parts = directory.Split(Path.DirectorySeparatorChar);
            if (parts.Length >= 1)
            {
                string lastPart = parts[^1]; // directory最后一部分
                string seconPart = parts[^2];
                files.Add(seconPart + Path.DirectorySeparatorChar + lastPart + Path.DirectorySeparatorChar + fileName);
            }
            else
            {
                Debug.Log("路径部分不足两层");
            }

        }

        return files;
    }


    
}
