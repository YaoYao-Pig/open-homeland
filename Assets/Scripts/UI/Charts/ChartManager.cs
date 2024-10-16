using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class ChartManager : MonoBehaviour
{
    private int kNum = 5;//展示前五个

    private GameObject repoDevelopNetChart;
    private GameObject repoOpenRankChart;
    private string repoDevelopNetChartName = "Repo_TopKDeveloperChart";
    private string repoOpenrankChartName = "Repo_OpenRankChart";

    // 创建用于存储分类后的数据的字典
    Dictionary<string, List<Repo_Read_OneOpenRank>> categorizedData = new Dictionary<string, List<Repo_Read_OneOpenRank>>();


    private void Awake()
    {
        repoDevelopNetChart = GameObject.Find(repoDevelopNetChartName);
        repoOpenRankChart = GameObject.Find(repoOpenrankChartName);
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

    private void IniteCategorizedData(List<Repo_Read_OneOpenRank> dataList)
    {
        // 按照时间排序（假设 dataTime 格式为 "yyyy-MM-dd"）
        dataList.Sort((a, b) => DateTime.Parse(a.dataTime).CompareTo(DateTime.Parse(b.dataTime)));


        foreach (var data in dataList)
        {
            string key = data.type.ToString(); // 获取类型的字符串表示，例如 "Year", "Month", "Quarter"

            if (!categorizedData.ContainsKey(key))
            {
                categorizedData[key] = new List<Repo_Read_OneOpenRank>();
            }
            categorizedData[key].Add(data);
        }
    }

    public void IniteRepoOpenRankChart(List<Repo_Read_OneOpenRank> repoOpenRankList)
    {
        // 假设 dataList 已经填充了您的数据
        List<Repo_Read_OneOpenRank> dataList = repoOpenRankList;
        // 过滤只保留月份的数据
        List<Repo_Read_OneOpenRank> monthDataList = new List<Repo_Read_OneOpenRank>();

        foreach (var data in dataList)
        {
            if (data.type == Repo_Read_OneOpenRank.Repo_Read_TimeStamp.Month)
            {
                monthDataList.Add(data);
            }
        }

        // 按照月份排序
        monthDataList.Sort((a, b) =>
        {
            DateTime dateA = DateTime.ParseExact(a.dataTime, "yyyy-MM", null);
            DateTime dateB = DateTime.ParseExact(b.dataTime, "yyyy-MM", null);
            return dateA.CompareTo(dateB);
        });

        // 创建图表
        var chart = repoOpenRankChart.AddComponent<LineChart>();
        chart.Init();
        chart.SetSize(580, 300);

        // 设置图表标题
        var title = chart.EnsureChartComponent<Title>();
        title.text = "OpenRank 月度变化图";

        // 设置 X 轴和 Y 轴
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.boundaryGap = false;
        xAxis.axisLabel.rotate = 45;
        xAxis.axisLabel.interval = 0;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        // 添加提示框和图例
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.type = Tooltip.Type.Line;

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = false; // 只有一个系列，不需要显示图例

        // 清除旧数据
        chart.RemoveData();

        // 添加系列
        var serie = chart.AddSerie<Line>("OpenRank");

        // 添加数据
        foreach (var data in monthDataList)
        {
            // 添加 X 轴数据（月份）
            chart.AddXAxisData(data.dataTime);

            // 添加 Y 轴数据（OpenRank 值）
            chart.AddData(serie.index, (double)data.openRank);
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

    public void ParseJsonToChart(Repo_Read_RepoDeveloperNet r)
    {
        List<Repo_Read_DevelopNet_Node> topK = r.GetOpenRankTopK(kNum);
        IniteCharts(topK);
    }
    public void DestoryCharts()
    {
        DestoryRepoDevelopNetChart();
    }

}
