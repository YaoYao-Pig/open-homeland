using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// RepoDeveloperNet ���ڶ�ȡRepo��DeveloperNetWork����Ϣ
/// </summary>
public class RepoDeveloperNet
{
    public class _Read_Node
    {
        public string _name;
        public double _openRank;
        public _Read_Node(string _n, double _or) { _name = _n;_openRank = _or; }

        public override string ToString()
        {
            return _name.ToString() + " " + _openRank.ToString() + "\n";
        }
    }

    public class _Read_Edge
    {
        public string _startNodeName;
        public string _endNodeName;
        public double _relative;

        public _Read_Edge(string _s,string _e, double _r) { _startNodeName = _s; _endNodeName = _e; _relative = _r; }
        public override string ToString()
        {
            return _startNodeName.ToString() + " " + _endNodeName.ToString()+" "+ _relative.ToString() + "\n";
        }
    }

    public List<_Read_Node> nodes;
    public List<_Read_Edge> edges;

    public RepoDeveloperNet()
    {
        nodes = new List<_Read_Node>();
        edges = new List<_Read_Edge>();
    }


    /// <summary>
    /// ����JsonData��ת��ΪRepoDeveloperNet��
    /// </summary>
    /// <param name="_data">�����JsonData����</param>
    /// <returns>RepoDeveloperNet��</returns>
    public static RepoDeveloperNet ParseJson(LitJson.JsonData _data)
    {
        RepoDeveloperNet result=new RepoDeveloperNet();

        if (_data["nodes"] == null) throw new System.Exception("Repository:RepoDeveloperNet::ParseJson," +
                               "JsonData don't contains \"nodes\"");

        if (_data["edges"] == null) throw new System.Exception("Repository:RepoDeveloperNet::ParseJson," +
                               "JsonData don't contains \"edges\"");
        //�����
        foreach (LitJson.JsonData _node in _data["nodes"])
        {
            
            //Debug.Log(_node[1].ToString());
            result.nodes.Add(new _Read_Node(_node[0].ToString(), (double.Parse(_node[1].ToString()))));
        }

        //�����
        foreach (LitJson.JsonData _edge in _data["edges"])
        {
            result.edges.Add(new _Read_Edge(_edge[0].ToString(), _edge[1].ToString(),(double.Parse(_edge[2].ToString()))));
        }

        return result;
    }

    public override string ToString()
    {
        string result = "";
        foreach(var node in nodes)
        {
            result+=node.ToString();
        }
        foreach(var edge in edges)
        {
            result += edge.ToString();
        }
        return result;
    }

}

public class RepoRepoNet
{

}


public class Repository
{

    public RepoDeveloperNet developerNetwork;

    public RepoRepoNet repoRepoNet;


    public override string ToString()
    {
        return developerNetwork.ToString();
    }


}


