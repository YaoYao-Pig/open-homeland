using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum NodeType{
    User,Repo
}


[System.Serializable]
public class Node
{
    public string nodeName;
    public int nodeId;
    public NodeType nodeType; //标记节点类型

    public float openRank;

    public float scale;

    public float radius;
    public Vector3 position { get; set; }
    public Node() { }
    public Node(string _name,int _id,NodeType _type){
        radius = 1.0f;
        nodeName = _name;
        nodeId = _id;
        nodeType = _type;
        position = Vector3.zero;

        scale = 1.0f;
        radius *= scale;
    }

    

}

[System.Serializable]
public class NodeComponent:MonoBehaviour{
    public Node node;
    public Material lineMaterial;
}