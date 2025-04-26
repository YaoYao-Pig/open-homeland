using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnvironmentalEffectsManager : MonoBehaviour
{
    [Header("Effects References")]
    [SerializeField] private CommitMeteorSystem meteorSystem;
    [SerializeField] private AuroraController northAurora;
    [SerializeField] private AuroraController southAurora;
    
    [Header("Planet Reference")]
    [SerializeField] private Transform planetTransform;
    
    [Header("Data Settings")]
    [SerializeField] private float dataUpdateInterval = 60f; // 数据更新间隔（秒）
    [SerializeField] private bool useSimulatedData = true; // 使用模拟数据
    
    private float lastDataUpdateTime = 0f;
    private float currentActivityLevel = 0.5f;
    
    private void Start()
    {
        // 初始化引用
        if (planetTransform == null)
        {
            planetTransform = transform;
        }
        
        InitializeEffects();
        
        // 开始数据更新循环
        StartCoroutine(DataUpdateLoop());
    }
    
    private void InitializeEffects()
    {
        // 初始化流星系统
        if (meteorSystem == null)
        {
            GameObject meteorSystemObj = new GameObject("CommitMeteorSystem");
            meteorSystemObj.transform.parent = transform;
            meteorSystem = meteorSystemObj.AddComponent<CommitMeteorSystem>();
        }
        
        // 初始化北极光
        if (northAurora == null)
        {
            GameObject auroraObj = new GameObject("NorthAurora");
            auroraObj.transform.parent = transform;
            auroraObj.transform.localPosition = Vector3.zero;
            northAurora = auroraObj.AddComponent<AuroraController>();
            northAurora.AdjustToMatchPlanet(planetTransform);
        }
        
        // 初始化南极光（翻转的北极光）
        if (southAurora == null)
        {
            GameObject auroraObj = new GameObject("SouthAurora");
            auroraObj.transform.parent = transform;
            auroraObj.transform.localPosition = Vector3.zero;
            auroraObj.transform.localRotation = Quaternion.Euler(180, 0, 0); // 翻转
            southAurora = auroraObj.AddComponent<AuroraController>();
            southAurora.AdjustToMatchPlanet(planetTransform);
        }
    }
    
    private IEnumerator DataUpdateLoop()
    {
        while (true)
        {
            if (useSimulatedData)
            {
                UpdateWithSimulatedData();
            }
            else
            {
                yield return StartCoroutine(UpdateWithRealData());
            }
            
            yield return new WaitForSeconds(dataUpdateInterval);
        }
    }
    
    private void UpdateWithSimulatedData()
    {
        // 模拟活跃度变化
        currentActivityLevel = Mathf.PingPong(Time.time * 0.01f, 1f);
        
        // 更新极光效果
        UpdateAuroraEffects(currentActivityLevel);
        
        // 流星雨由CommitMeteorSystem内部模拟
    }
    
    private IEnumerator UpdateWithRealData()
    {
        // 这里可以添加从GameData或WebController获取真实数据的代码
        // 例如：
        
        // 获取仓库名称
        string repoName = GameData.Instance.GetRepoName();
        if (string.IsNullOrEmpty(repoName))
        {
            yield break;
        }
        
        // 获取OpenRank数据
        Repo_Read_OpenRank openRankData = GameData.Instance.GetRepoOpenRankList();
        if (openRankData != null)
        {
            // 计算活跃度（基于OpenRank）
            float openRank = openRankData.lastOpenRank;
            // 将OpenRank映射到0-1范围（假设最大值为10）
            float activityLevel = Mathf.Clamp01(openRank / 10f);
            
            // 更新极光效果
            UpdateAuroraEffects(activityLevel);
        }
        
        // 获取提交数据可以在这里添加
        // ...
        
        yield return null;
    }
    
    private void UpdateAuroraEffects(float activityLevel)
    {
        if (northAurora != null)
        {
            northAurora.UpdateAurora(activityLevel);
        }
        
        if (southAurora != null)
        {
            southAurora.UpdateAurora(activityLevel);
        }
    }
    
    // 公共方法，可以从其他脚本调用
    public void TriggerCommitEffect(int commitSize)
    {
        if (meteorSystem != null)
        {
            CommitMeteorSystem.CommitData commitData = new CommitMeteorSystem.CommitData
            {
                id = Guid.NewGuid().ToString(),
                message = "External commit trigger",
                size = commitSize
            };
            
            meteorSystem.TriggerCommitMeteorShower(commitData);
        }
    }
    
    public void SetActivityLevel(float level)
    {
        currentActivityLevel = Mathf.Clamp01(level);
        UpdateAuroraEffects(currentActivityLevel);
    }
}
