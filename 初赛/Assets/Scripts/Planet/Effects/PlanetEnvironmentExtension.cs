using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Planet))]
public class PlanetEnvironmentExtension : MonoBehaviour
{
    [Header("Environment Effects")]
    [SerializeField] private bool enableEnvironmentEffects = true;
    
    private Planet planet;
    private GameObject environmentEffectsObject;
    private EnvironmentalEffectsManager effectsManager;
    
    private void Awake()
    {
        planet = GetComponent<Planet>();
    }
    
    private void Start()
    {
        // 等待星球完全初始化
        StartCoroutine(SetupEnvironmentEffectsDelayed());
    }
    
    private IEnumerator SetupEnvironmentEffectsDelayed()
    {
        // 等待一帧，确保星球完全初始化
        yield return null;
        
        if (enableEnvironmentEffects)
        {
            SetupEnvironmentEffects();
        }
    }
    
    private void SetupEnvironmentEffects()
    {
        // 创建环境效果对象
        if (environmentEffectsObject == null)
        {
            environmentEffectsObject = new GameObject("EnvironmentEffects");
            environmentEffectsObject.transform.parent = transform;
            environmentEffectsObject.transform.localPosition = Vector3.zero;
            environmentEffectsObject.transform.localRotation = Quaternion.identity;
            
            // 添加环境效果管理器
            effectsManager = environmentEffectsObject.AddComponent<EnvironmentalEffectsManager>();
            
            // 设置星球引用
            var planetTransform = planet.transform;
            effectsManager.SendMessage("InitializeEffects", SendMessageOptions.DontRequireReceiver);
            
            // 尝试获取仓库数据
            StartCoroutine(UpdateEffectsWithRepositoryData());
        }
    }
    
    private IEnumerator UpdateEffectsWithRepositoryData()
    {
        // 等待PlanetManager完成数据加载
        PlanetManager planetManager = FindObjectOfType<PlanetManager>();
        if (planetManager != null)
        {
            // 等待PlanetManager完成初始化
            while (planetManager.gameObject.name == "PlanetManager")
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // 获取OpenRank数据
            Repo_Read_OpenRank openRankData = GameData.Instance.GetRepoOpenRankList();
            if (openRankData != null && effectsManager != null)
            {
                // 计算活跃度（基于OpenRank）
                float openRank = openRankData.lastOpenRank;
                // 将OpenRank映射到0-1范围（假设最大值为10）
                float activityLevel = Mathf.Clamp01(openRank / 10f);
                
                // 更新极光效果
                effectsManager.SendMessage("SetActivityLevel", activityLevel, SendMessageOptions.DontRequireReceiver);
                
                // 触发一次流星雨效果
                effectsManager.SendMessage("TriggerCommitEffect", 2, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    public void ToggleEnvironmentEffects(bool enable)
    {
        enableEnvironmentEffects = enable;
        
        if (enable && environmentEffectsObject == null)
        {
            SetupEnvironmentEffects();
        }
        else if (!enable && environmentEffectsObject != null)
        {
            Destroy(environmentEffectsObject);
            environmentEffectsObject = null;
            effectsManager = null;
        }
    }
    
    // 当星球更新时调用此方法
    public void UpdateEnvironmentEffects()
    {
        if (enableEnvironmentEffects && effectsManager != null)
        {
            StartCoroutine(UpdateEffectsWithRepositoryData());
        }
    }
    
    // 手动触发流星雨效果
    public void TriggerMeteorShower(int size)
    {
        if (effectsManager != null)
        {
            effectsManager.SendMessage("TriggerCommitEffect", size, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    // 手动设置极光活跃度
    public void SetAuroraActivity(float level)
    {
        if (effectsManager != null)
        {
            effectsManager.SendMessage("SetActivityLevel", level, SendMessageOptions.DontRequireReceiver);
        }
    }
}
