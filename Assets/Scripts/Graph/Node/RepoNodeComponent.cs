using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepoNodeComponent : NodeComponent
{
    public Repository repository;

    private void Awake()
    {
        repository = new Repository();
    }

}