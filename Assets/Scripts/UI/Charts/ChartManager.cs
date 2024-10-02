using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class ChartManager : MonoBehaviour
{
    private int kNum = 5;//展示前五个

    private GameObject repoDevelopNetChart;
    private string repoDevelopNetChartName = "Repo_TopKDeveloperChart";


    private void Awake()
    {
        repoDevelopNetChart = GameObject.Find(repoDevelopNetChartName);
        if (repoDevelopNetChart == null) throw new System.Exception("RepoSphereClicker:Awake=> repoDevelopNetChart not found");
    }

    private void IniteRepoDevelopNetChart(List<Repo_Read_DevelopNet_Node> topK)
    {
        var chart = repoDevelopNetChart.AddComponent<BarChart>();
        chart.Init();
        chart.SetSize(580, 300);

        var title = chart.EnsureChartComponent<Title>();
        title.text = "TopK developer";

        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.splitNumber = 10;
        xAxis.boundaryGap = true;
        xAxis.type = Axis.AxisType.Value;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Category;

        chart.RemoveData();
        chart.AddSerie<Bar>("bar");

        var background = chart.EnsureChartComponent<Background>();
        background.imageColor = new Color(0, 0, 0, 0); // 完全透明


        for (int i = 1; i <= topK.Count; ++i)
        {
            chart.AddYAxisData(topK[^i]._name);
            chart.AddData(0, topK[^i]._openRank);
        }

    }

    private void IniteCharts(List<Repo_Read_DevelopNet_Node> repo_Read_DevelopNet_Nodes)
    {
        IniteRepoDevelopNetChart(repo_Read_DevelopNet_Nodes);
    }

    private void DestoryRepoDevelopNetChart()
    {
        BarChart barChartComponent = repoDevelopNetChart.GetComponent<BarChart>();
        if (barChartComponent != null)
        {
            Destroy(barChartComponent);
        }
    }


    /// <summary>
    /// 把json转换为图表,本函数用于转换developer数据
    /// </summary>
    public void ParseJsonToChart(string _json)
    {
        Debug.Log(_json);
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(_json);
        Repo_Read_RepoDeveloperNet r = Repo_Read_RepoDeveloperNet.ParseJson(jsonData);
        List<Repo_Read_DevelopNet_Node> topK = r.GetOpenRankTopK(kNum);
        IniteCharts(topK);
    }

    public void DestoryCharts()
    {
        DestoryRepoDevelopNetChart();
    }

}
