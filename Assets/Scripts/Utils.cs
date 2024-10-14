using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Utils 
{
    /// <summary>
    /// ��ȡAssets/Resources�ļ����µ�Json����
    /// </summary>
    /// <param name="_path">·����������������Resources�ļ����µ����·�����������Resources/Data/Repo��˵����дData/Repo</param>
    /// <returns>����һ��string���ͣ��Ƕ�ȡ�������ַ���</returns>
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
            string fileName = Path.GetFileName(f); // ��ȡ�ļ���Ŀ¼��
            fileName = fileName.Split(".")[0];
            // �ָ�·��
            string[] parts = directory.Split(Path.DirectorySeparatorChar);
            if (parts.Length >= 1)
            {
                string lastPart = parts[^1]; // directory���һ����
                string seconPart = parts[^2];
                files.Add(seconPart + Path.DirectorySeparatorChar + lastPart + Path.DirectorySeparatorChar + fileName);
            }
            else
            {
                Debug.Log("·�����ֲ�������");
            }

        }

        return files;
    }


    
}
