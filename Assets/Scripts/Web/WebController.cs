using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WebController : MonoBehaviour
{


    private List<string> urls;

    void SaveJsonToFile(string json, string relativePath)
    {
        string fullPath = Application.dataPath+"/"+ relativePath;
        Debug.Log(fullPath);

        // 确保文件夹存在
        string directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 写入文件
        File.WriteAllText(fullPath, json);
        Debug.Log($"JSON data saved to: {fullPath}");
    }
    IEnumerator FetchJson(string _url,string _name)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_url))
        {
            // 发送请求并等待响应
            yield return request.SendWebRequest();

            // 检查请求是否成功
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // 请求成功，处理 JSON 数据
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("JSON Response: " + jsonResponse);
                //存储Json数据
                SaveJsonToFile(jsonResponse, "Resources/repo/develop/"+ _name + ".json");

                
                //  JSON 数据存储

            }
        }
    }
    IEnumerator StartFetchingData()
    {
        foreach (var url in urls)
        {
            yield return StartCoroutine(FetchJson(url, WorldInfo.initeRepoNameList[urls.IndexOf(url)].Split("/")[1]));
        }


        // yield return new WaitForSeconds(20);

        // 在所有协程完成后执行的代码
        WorldManager.Instance.Inite();
    }



    public static IEnumerator GetRepoDetail(string _repoName,string _m)
    {
        string url = new string(WorldInfo.requestHead +
                    WorldInfo.platform + WorldInfo.httpSeparatorChar +
                    _repoName + WorldInfo.httpSeparatorChar + _m + ".json");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // 请求成功，处理 JSON 数据
                string jsonResponse = request.downloadHandler.text;
                //Debug.Log(jsonResponse);
                LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(jsonResponse);
                GameData.Instance.AddRepoDetail(Repo_Read_RepoDeveloperNet.ParseJson(jsonData));
            }

        }
    }



  

    public static IEnumerator GetRepoOpenRank(string _repoName, string _m="openrank")
    {
        string url = new string(WorldInfo.requestHead +
                    WorldInfo.platform + WorldInfo.httpSeparatorChar +
                    _repoName + WorldInfo.httpSeparatorChar + _m + ".json");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // 请求成功，处理 JSON 数据
                string jsonResponse = request.downloadHandler.text;
                //Debug.Log(jsonResponse);

                GameData.Instance.AddRepoOpenRankList(Repo_Read_OpenRank.ParseJson(jsonResponse));
            }
        }
    }

    public static IEnumerator GetDeveloperOpenRank(string _userName,string _m = "openrank")
    {
        string url = new string(WorldInfo.requestHead +
                   WorldInfo.platform + WorldInfo.httpSeparatorChar +
                   _userName + WorldInfo.httpSeparatorChar + _m + ".json");
        Debug.Log(url);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // 请求成功，处理 JSON 数据
                string jsonResponse = request.downloadHandler.text;

                Developer_Read_OpenRank developer = Developer_Read_OpenRank.ParseJson(jsonResponse);
                GameData.Instance.AddDeveloperOpenRankList(developer);
                Debug.Log("GetDeveloperOpenRank:" + jsonResponse);
                ChartManager.Instance.IniteDeveloperOpenRankChart(developer.developerOpenrankList);
                //GameData.Instance.AddRepoOpenRankList(Repo_Read_OpenRank.ParseJson(jsonResponse));
            }
        }
    }

    public static IEnumerator GetUserDeveloperOpenRank(string _userName, Action exceptionCallBack,string _m = "openrank")
    {
        string url = new string(WorldInfo.requestHead +
                   WorldInfo.platform + WorldInfo.httpSeparatorChar +
                   _userName + WorldInfo.httpSeparatorChar + _m + ".json");
        Debug.Log(url);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                exceptionCallBack();
            }
            else
            {
                // 请求成功，处理 JSON 数据
                string jsonResponse = request.downloadHandler.text;

                Developer_Read_OpenRank developer = Developer_Read_OpenRank.ParseJson(jsonResponse);
                GameData.Instance.AddDeveloperOpenRankList(developer);
                Debug.Log("GetDeveloperOpenRank:" + jsonResponse);
                //ChartManager.Instance.IniteDeveloperOpenRankChart(developer.developerOpenrankList);
                //GameData.Instance.AddRepoOpenRankList(Repo_Read_OpenRank.ParseJson(jsonResponse));
            }
        }
    }

}
