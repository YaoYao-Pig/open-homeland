using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class ChartManager : MonoBehaviour
{
    private int kNum = 5;//չʾǰ���

    public Theme charTheme;
    public GameObject repoDevelopNetChart;
    public GameObject repoOpenRankChart;
    public GameObject repoDeveloperPercentChart;
    private string repoDevelopNetChartName = "Repo_TopKDeveloperChart";
    private string repoOpenrankChartName = "Repo_OpenRankChart";

    // �������ڴ洢���������ݵ��ֵ�
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
        title.text = "Top 5 OpenRank������";
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
        background.imageColor = new Color(0, 0, 0, 0); // ��ȫ͸��
        chart.theme.sharedTheme = charTheme;


        for (int i = 1; i <= topK.Count; ++i)
        {
            chart.AddYAxisData(topK[^i]._name);
            chart.AddData(0, topK[^i]._openRank);
        }

    }

    private void IniteCategorizedData(List<Repo_Read_OneOpenRank> dataList)
    {
        // ����ʱ�����򣨼��� dataTime ��ʽΪ "yyyy-MM-dd"��
        dataList.Sort((a, b) => DateTime.Parse(a.dataTime).CompareTo(DateTime.Parse(b.dataTime)));


        foreach (var data in dataList)
        {
            string key = data.type.ToString(); // ��ȡ���͵��ַ�����ʾ������ "Year", "Month", "Quarter"

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
        legend.itemWidth = 60; // ÿ��ͼ����Ŀ��
        legend.itemHeight = 30; // ÿ��ͼ����ĸ߶�
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
        // ���� dataList �Ѿ��������������
        List<Repo_Read_OneOpenRank> dataList = repoOpenRankList;
        // ����ֻ�����·ݵ�����
        List<Repo_Read_OneOpenRank> monthDataList = new List<Repo_Read_OneOpenRank>();

        foreach (var data in dataList)
        {
            if (data.type == Repo_Read_OneOpenRank.Repo_Read_TimeStamp.Month)
            {
                monthDataList.Add(data);
            }
        }

        // �����·�����
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

        //�������ţ�
        DataZoom dataZoom = chart.EnsureChartComponent<DataZoom>();
        dataZoom.enable = true; // ���� DataZoom
        dataZoom.supportInside = true; // ֧���ڲ����ţ����������ţ�
        dataZoom.supportSlider = false; // �������Ҫ��ʾ�ⲿ����������������Ϊ false
        dataZoom.zoomLock = false; // �ɸ�����Ҫ���û������������
        dataZoom.minShowNum = 10;  // ��ʾ����С���ݵ���
        dataZoom.orient = Orient.Horizonal; // �������ŷ��򣬿����� Horizontal��ˮƽ���� Vertical����ֱ��
        dataZoom.showDataShadow = true; // �����Ƿ���ʾ������


        // ����ͼ�����
        var title = chart.EnsureChartComponent<Title>();
        title.text = "OpenRank �¶ȱ仯ͼ";
        title.labelStyle.textStyle.fontSize = 50;

        // ���� X ��� Y ��
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.boundaryGap = false;
        xAxis.axisLabel.rotate = 45;
        xAxis.axisLabel.interval = 0;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        // �����ʾ���ͼ��
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.type = Tooltip.Type.Line;

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = false; // ֻ��һ��ϵ�У�����Ҫ��ʾͼ��

        // ���������
        chart.RemoveData();

        // ���ϵ��
        var serie = chart.AddSerie<Line>("OpenRank");
        chart.theme.sharedTheme = charTheme;
        // �������
        foreach (var data in monthDataList)
        {
            // ��� X �����ݣ��·ݣ�
            chart.AddXAxisData(data.dataTime);

            // ��� Y �����ݣ�OpenRank ֵ��
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
