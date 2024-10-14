using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// RepoDeveloperNet ���ڶ�ȡRepo��DeveloperNetWork����Ϣ
/// </summary>

public class RepoRepoNet
{

}


public class Repository
{

    public Repo_Read_RepoDeveloperNet developerNetwork;

    public RepoRepoNet repoRepoNet;

    public float repoOpenRank = 0.0f;


    public float developOpenRankAverage = 0.0f;
    public override string ToString()
    {
        return developerNetwork.ToString();
    }

    public float GetRepoDevelopNetAverageOpenRank()
    {
        if (developOpenRankAverage > 0.0) return developOpenRankAverage;
        
        foreach(var node in developerNetwork.nodes)
        {
            developOpenRankAverage +=(float) node._openRank;
        }
        developOpenRankAverage /= developerNetwork.nodes.Count;
        return developOpenRankAverage;

    }

}


