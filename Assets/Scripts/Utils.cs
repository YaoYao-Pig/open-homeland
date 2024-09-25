using System.Collections;
using System.Collections.Generic;
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
        UnityEngine.TextAsset s = Resources.Load(_path) as TextAsset;
        return s.text;
    }
}
