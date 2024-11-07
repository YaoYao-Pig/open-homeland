using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repo_Read_RepoDeveloperNet
{


    public List<Repo_Read_DevelopNet_Node> nodes;
    public List<Repo_Read_DevelopNet_Edge> edges;

    public List<Repo_Read_DevelopNet_Node> GetNodes()
    {
        return nodes;
    }
    public List<Repo_Read_DevelopNet_Edge> GetEdges()
    {
        return edges;
    }
    public Repo_Read_RepoDeveloperNet()
    {
        nodes = new List<Repo_Read_DevelopNet_Node>();
        edges = new List<Repo_Read_DevelopNet_Edge>();
    }


    /// <summary>
    /// 读入JsonData，转换为RepoDeveloperNet类
    /// </summary>
    /// <param name="_data">输入的JsonData数据</param>
    /// <returns>RepoDeveloperNet类</returns>
    public static Repo_Read_RepoDeveloperNet ParseJson(LitJson.JsonData _data)
    {
        Repo_Read_RepoDeveloperNet result = new Repo_Read_RepoDeveloperNet();

        if (_data["nodes"] == null) throw new System.Exception("Repository:RepoDeveloperNet::ParseJson," +
                               "JsonData don't contains \"nodes\"");

        if (_data["edges"] == null) throw new System.Exception("Repository:RepoDeveloperNet::ParseJson," +
                               "JsonData don't contains \"edges\"");
        //处理点
        foreach (LitJson.JsonData _node in _data["nodes"])
        {

            //Debug.Log(_node[1].ToString());
            result.nodes.Add(new Repo_Read_DevelopNet_Node(_node[0].ToString(), (double.Parse(_node[1].ToString()))));
        }

        //处理边
        foreach (LitJson.JsonData _edge in _data["edges"])
        {
            result.edges.Add(new Repo_Read_DevelopNet_Edge(_edge[0].ToString(), _edge[1].ToString(), (double.Parse(_edge[2].ToString()))));
        }

        return result;
    }



    public List<Repo_Read_DevelopNet_Node> GetOpenRankTopK(int k)
    {
        return nodes.GetRange(0, k);
    }
    public override string ToString()
    {
        string result = "";
        foreach (var node in nodes)
        {
            result += node.ToString();
        }
        foreach (var edge in edges)
        {
            result += edge.ToString();
        }
        return result;
    }

}
