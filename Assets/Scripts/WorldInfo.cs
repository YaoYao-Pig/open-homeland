using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//��ʱ���������ļ���дһЩ������Ϣ
public class WorldInfo 
{
    public static string repo_developerNetFilePath = "Assets/Resources/repo/develop";
    public static string repo_repoNetFilePath = "Assets/Resources/repo/repo";



    public static string requestHead = "https://oss.x-lab.info/";
    public static string httpSeparatorChar = "/";
    public static string platform = "open_digger/github";

    public static List<string> initeRepoNameList = new List<string>()
    {
        "CleverRaven/Cataclysm-DDA",
        "Anuken/Mindustry",
        "ppy/osu",
        "OpenRCT2/OpenRCT2",
        "Whisky-App/Whisky",
        "fogleman/Craft",
        "diasurgical/devilution",
        "yairm210/Unciv"
    };


    public static List<string> metrics = new List<string>()
    {
        "developer_network"
    };

    public static string repo_developNetRequest = "";
    
}
