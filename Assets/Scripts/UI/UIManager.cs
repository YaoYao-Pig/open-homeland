using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }
    #region USER
    public GameObject userDetailRoot;
    public GameObject userTableRoot;
    public BarChart userBarChart;
    #endregion
    private void Awake()
    {
        _instance = this;
    }
}
