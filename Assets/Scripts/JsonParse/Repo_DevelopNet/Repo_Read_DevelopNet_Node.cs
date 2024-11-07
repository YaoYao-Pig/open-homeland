using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repo_Read_DevelopNet_Node
{
    public string _name;
    public double _openRank;
    public Repo_Read_DevelopNet_Node(string _n, double _or) { _name = _n; _openRank = _or; }

    public override bool Equals(object obj)
    {
        return obj is Repo_Read_DevelopNet_Node node &&
               _name == node._name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_name);
    }

    public override string ToString()
    {
        return _name.ToString() + " " + _openRank.ToString() + "\n";
    }



}
