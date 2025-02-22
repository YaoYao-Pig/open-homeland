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

        // ȷ���ļ��д���
        string directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // д���ļ�
        File.WriteAllText(fullPath, json);
        Debug.Log($"JSON data saved to: {fullPath}");
    }
    IEnumerator FetchJson(string _url,string _name)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_url))
        {
            // �������󲢵ȴ���Ӧ
            yield return request.SendWebRequest();

            // ��������Ƿ�ɹ�
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                // ����ɹ������� JSON ����
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("JSON Response: " + jsonResponse);
                //�洢Json����
                SaveJsonToFile(jsonResponse, "Resources/repo/develop/"+ _name + ".json");

                
                //  JSON ���ݴ洢

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

        // ������Э����ɺ�ִ�еĴ���
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
                // ����ɹ������� JSON ����
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
                // ����ɹ������� JSON ����
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
                // ����ɹ������� JSON ����
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
                // ����ɹ������� JSON ����
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
