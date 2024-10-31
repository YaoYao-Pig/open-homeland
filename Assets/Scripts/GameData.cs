using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private static GameData _instance;
    public static GameData Instance { 
        get{
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameData>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(GameData).Name);
                    _instance=obj.AddComponent<GameData>();
                }
            }
            return _instance;
        }
        private set { } }

    public Dictionary<string,object> gameParams;


    public bool AddgameParams(string key,object value)
    {
        object r=new object();
        bool tmp= gameParams.TryGetValue(key, out r);
        if (tmp == false)
        {
            gameParams.Add(key, value);
            return true;
        }
        else return false;
    }
    public void ClearParams()
    {
        gameParams.Clear();
    }
    //��ӻ�ȡRepoName;
    public void AddRepoName(string _name){AddgameParams("Repo_Name",(object) _name);}

    public string GetRepoName(){
        object _name;
        if(gameParams.TryGetValue("Repo_Name",out _name))
        {
            return (string)_name;
        }
        else
        {
            return "";
        }
    }

    public void AddRepoOpenRank(float _openrank)
    {
        AddgameParams("Repo_OpenRank", (object)_openrank);
    }


    public float GetRepoOpenRank()
    {
        object _openrank;
        if (gameParams.TryGetValue("Repo_OpenRank", out _openrank))
        {
            return (float)_openrank;
        }
        else
        {
            return 0.0f;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this){
            Destroy(gameObject); // ȷ��Ψһ��
        }
        else
        {
            _instance = this;
            gameParams = new Dictionary<string, object>(); // ��ʼ���ֵ�
            DontDestroyOnLoad(gameObject); // �ڳ����л�ʱ��������
        }
    }


    public void AddRepoOpenRankList(Repo_Read_OpenRank repoOpenRank)
    {
        AddgameParams("Repo_OpenRankList", (object)repoOpenRank);
    }

    public Repo_Read_OpenRank GetRepoOpenRankList()
    {
        object _openrankList;
        if (gameParams.TryGetValue("Repo_OpenRankList", out _openrankList))
        {
            return (Repo_Read_OpenRank)_openrankList;
        }
        else
        {
            return new Repo_Read_OpenRank();
        }
    }

    public void AddRepoDeveloperNumber(int childCount)
    {
        AddgameParams("Repo_DeveloperNumber", (object)childCount);
    }

    public int GetRepoDeveloperNumber()
    {
        object _deveNum;
        if (gameParams.TryGetValue("Repo_DeveloperNumber", out _deveNum))
        {
            return (int)_deveNum;
        }
        else
        {
            return 0;
        }
    }
}
