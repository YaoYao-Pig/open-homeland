using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repo_Read_DevelopNet_Edge
{
    public string _startNodeName;
    public string _endNodeName;
    public double _relative;

    public Repo_Read_DevelopNet_Edge(string _s, string _e, double _r) { _startNodeName = _s; _endNodeName = _e; _relative = _r; }
    public override string ToString()
    {
        return _startNodeName.ToString() + " " + _endNodeName.ToString() + " " + _relative.ToString() + "\n";
    }
}