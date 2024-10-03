using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RepoNode : Node
{
    public RepoNode(string _name, int _id, NodeType _type) : base(_name, _id, _type)
    {
        scale = 10.0f;
        radius *= scale;
    }
}
public class RepoNodeComponent : NodeComponent
{
    public Repository repository;

    private void Awake()
    {
        repository = new Repository();
    }

}