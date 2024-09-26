using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RepoNode : Node
{
    public RepoNode(string _name, int _id, NodeType _type) : base(_name, _id, _type)
    {
        scale = 4.0f;
    }
}
public class RepoNodeComponent : NodeComponent
{
}