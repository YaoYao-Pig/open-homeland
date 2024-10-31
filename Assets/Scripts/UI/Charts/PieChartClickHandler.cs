using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCharts.Runtime;

public class PieChartClickHandler : MonoBehaviour
{
    private PieChart chart;
    public PieChart Chart { set { chart = value; chart.onSerieClick = OnPointerClick; } }


    public void OnPointerClick(SerieEventData eventData)
    {
        //Debug.Log("click:" + eventData.serieIndex+"  "+eventData.dataIndex);
        
        Pie serie =(Pie) chart.series[eventData.serieIndex];
        SerieData data = serie.data[eventData.dataIndex];
        //Debug.Log("SerieData Data:" + data.name + " " + data.data);
        string _name = data.name;
        string _m = "openrank";
        StartCoroutine(WebController.GetDeveloperOpenRank(_name,_m));
    }

}
