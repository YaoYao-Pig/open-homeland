using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public class UserNode : Node
{

    public UserNode() : base()
    {

    }
    public UserNode(string _name,int _id,NodeType _type): base(_name,_id,_type)
    {
        scale = 0.2f;
        radius *= scale;
    }
}

public class UserNodeComponent : NodeComponent
{
}