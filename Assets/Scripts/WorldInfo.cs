using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//暂时用作配置文件，写一些配置信息
public class WorldInfo 
{
    public static string repo_developerNetFilePath = "Assets/Resources/repo/develop";
    public static string repo_repoNetFilePath = "Assets/Resources/repo/repo";




    public static string detailScenceName = "RepoDetailScence";

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
    "yairm210/Unciv",
    "torvalds/linux",
    "jaywcjlove/linux-command",
    "apache/superset",
    "apache/camel",
    "apache/echarts",
    "apache/spark",
    "apache/kafka",
    "apache/dubbo",
    "apache/airflow",
    "grpc/grpc",
    "nothings/stb",
    "facebook/infer",
    "shadowsocks/shadowsocks-windows",
    "Light-City/CPlusPlusThings",
    "nlohmann/json",
    "msgpack/msgpack-c",
    "danmar/cppcheck",
    "gabime/spdlog",
    "civetweb/civetweb",
    "spmallick/learnopencv",
    "confluentinc/librdkafka",
    "abseil/abseil-cpp"
    };


    public static List<string> metrics = new List<string>()
    {
        "developer_network"
    };

    public static string repo_developNetRequest = "";


    public static string gameDataName = "GameData";

    public static string mainScenceName = "MainScence";
}
