using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum NodeType{
    User,Repo
}

public class Node
{
    public string nodeName;
    public int nodeId;
    public NodeType nodeType; //标记节点类型

    public Vector3 position { get; set; }

    public Node(string _name,int _id,NodeType _type){
        position = Vector3.zero;
    }

    

}
