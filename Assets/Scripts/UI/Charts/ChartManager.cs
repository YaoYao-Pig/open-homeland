using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class ChartManager : MonoBehaviour
{
    private int kNum = 5;//展示前五个

    public Theme charTheme;
    public GameObject repoDevelopNetChart;
    public GameObject repoOpenRankChart;
    public GameObject repoDeveloperPercentChart;
    private string repoDevelopNetChartName = "Repo_TopKDeveloperChart";
    private string repoOpenrankChartName = "Repo_OpenRankChart";

    // 创建用于存储分类后的数据的字典
    Dictionary<string, List<Repo_Read_OneOpenRank>> categorizedData = new Dictionary<string, List<Repo_Read_OneOpenRank>>();

    private static ChartManager _instance;
    public static ChartManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ChartManager>();
            }
            return _instance;
        }
        private set
        {
            ;
        }
    }

    private void Awake()
    {
        if (_instance == null && _instance!=this)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance);
        }
        //repoDevelopNetChart = GameObject.Find(repoDevelopNetChartName);
        //repoOpenRankChart = GameObject.Find(repoOpenrankChartName);
        //if (repoDevelopNetChart == null) throw new System.Exception("RepoSphereClicker:Awake=> repoDevelopNetChart not found");
    }

    private void IniteRepoDevelopNetChart(List<Repo_Read_DevelopNet_Node> topK)
    {
        BarChart chart = repoDevelopNetChart.GetComponent<BarChart>();
        if(chart==null)
            chart = repoDevelopNetChart.AddComponent<BarChart>();
        chart.Init();
        chart.SetSize(1200, 600);

        var title = chart.EnsureChartComponent<Title>();
        title.text = "Top 5 OpenRank开发者";
        title.labelStyle.textStyle.fontSize = 50;
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
        chart.theme.sharedTheme = charTheme;


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

    


    private void DestoryRepoDevelopNetChart()
    {
        BarChart barChartComponent = repoDevelopNetChart.GetComponent<BarChart>();
        if (barChartComponent != null)
        {
            Destroy(barChartComponent);
        }
    }

    private void DestoryRepoOpenRankChar()
    {
        LineChart LinearChartComponent = repoOpenRankChart.GetComponent<LineChart>();
        if (LinearChartComponent != null)
        {
            Destroy(LinearChartComponent);
        }
    }
    private void DestoryRepoDeveloperPercentageChar()
    {
        PieChart chart = repoDeveloperPercentChart.GetComponent<PieChart>();
        if (chart != null)
        {
            Destroy(chart);
        }
    }

    public void IniteDeveloperPercentChart(Repo_Read_RepoDeveloperNet r)
    {
        PieChart chart = repoDeveloperPercentChart.GetComponent<PieChart>();
        if (chart == null)
            chart = repoDeveloperPercentChart.AddComponent<PieChart>();
        

        chart.Init();
        chart.SetSize(1200, 600);

        var title = chart.EnsureChartComponent<Title>();
        title.text = "Developer OpenRank Percentage";
        title.labelStyle.textStyle.fontSize = 50;
        chart.RemoveData();
        var serie=chart.AddSerie<Pie>();
        serie.center = new float[] { 0.3f, 0.5f };


        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.type = Tooltip.Type.Line;

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.itemWidth = 60; // 每个图例项的宽度
        legend.itemHeight = 30; // 每个图例项的高度
        legend.orient = Orient.Vertical;
        legend.location.align = Location.Align.TopRight;
        chart.ClearData();
        var nodeList = r.nodes;
        chart.theme.sharedTheme = charTheme;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            chart.AddData(serie.serieName, nodeList[i]._openRank, nodeList[i]._name);
            legend.AddData(nodeList[i]._name);

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


        LineChart chart = repoOpenRankChart.GetComponent<LineChart>();
        if (chart == null)
            chart = repoOpenRankChart.AddComponent<LineChart>();
        chart.Init();
        chart.SetSize(1200, 600);

        //设置缩放：
        DataZoom dataZoom = chart.EnsureChartComponent<DataZoom>();
        dataZoom.enable = true; // 启用 DataZoom
        dataZoom.supportInside = true; // 支持内部缩放（鼠标滚轮缩放）
        dataZoom.supportSlider = false; // 如果不需要显示外部缩放条，可以设置为 false
        dataZoom.zoomLock = false; // 可根据需要启用或禁用缩放锁定
        dataZoom.minShowNum = 10;  // 显示的最小数据点数
        dataZoom.orient = Orient.Horizonal; // 设置缩放方向，可以是 Horizontal（水平）或 Vertical（垂直）
        dataZoom.showDataShadow = true; // 控制是否显示缩放条


        // 设置图表标题
        var title = chart.EnsureChartComponent<Title>();
        title.text = "OpenRank 月度变化图";
        title.labelStyle.textStyle.fontSize = 50;

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
        chart.theme.sharedTheme = charTheme;
        // 添加数据
        foreach (var data in monthDataList)
        {
            // 添加 X 轴数据（月份）
            chart.AddXAxisData(data.dataTime);

            // 添加 Y 轴数据（OpenRank 值）
            chart.AddData(serie.index, (double)data.openRank);
        }
    }
    public void IniteTopKDeveloperChart(Repo_Read_RepoDeveloperNet r)
    {
        List<Repo_Read_DevelopNet_Node> topK = r.GetOpenRankTopK(kNum);
        IniteRepoDevelopNetChart(topK);
    }
    public void DestoryCharts()
    {
        DestoryRepoDevelopNetChart();
        DestoryRepoOpenRankChar();
        DestoryRepoDeveloperPercentageChar();
    }

}
