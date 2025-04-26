using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommitMeteorSystem : MonoBehaviour
{
    [Header("Meteor Settings")]
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private Transform planetTransform;
    [Range(5f, 50f)]
    [SerializeField] private float meteorSpeed = 20f;
    [Range(10f, 100f)]
    [SerializeField] private float spawnHeight = 50f;
    [Range(1, 10)]
    [SerializeField] private int meteorsPerCommit = 3;
    [Range(0.1f, 2f)]
    [SerializeField] private float spawnInterval = 0.2f;
    
    [Header("Commit Data")]
    [SerializeField] private float checkInterval = 60f; // 检查新提交的间隔（秒）
    [SerializeField] private string repoOwner = ""; // GitHub 仓库所有者
    [SerializeField] private string repoName = ""; // GitHub 仓库名称
    
    private float lastCheckTime = 0f;
    private List<string> processedCommits = new List<string>();
    private bool isShowering = false;
    
    // 模拟提交数据，实际项目中可以通过GitHub API获取
    private Queue<CommitData> pendingCommits = new Queue<CommitData>();
    
    private void Start()
    {
        if (planetTransform == null)
        {
            planetTransform = transform;
        }
        
        // 初始化时触发一次流星雨作为演示
        StartCoroutine(SimulateCommits());
    }
    
    private void Update()
    {
        // 检查是否有待处理的提交
        if (pendingCommits.Count > 0 && !isShowering)
        {
            CommitData commit = pendingCommits.Dequeue();
            TriggerCommitMeteorShower(commit);
        }
    }
    
    // 模拟定期获取提交数据
    private IEnumerator SimulateCommits()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 15f));
            
            // 模拟1-3个新提交
            int commitCount = UnityEngine.Random.Range(1, 4);
            for (int i = 0; i < commitCount; i++)
            {
                CommitData newCommit = new CommitData
                {
                    id = Guid.NewGuid().ToString(),
                    message = "Simulated commit " + DateTime.Now.ToString(),
                    size = UnityEngine.Random.Range(1, 10) // 提交大小（修改的文件数）
                };
                
                pendingCommits.Enqueue(newCommit);
            }
        }
    }
    
    // 当检测到新提交时调用
    public void TriggerCommitMeteorShower(CommitData commit)
    {
        if (processedCommits.Contains(commit.id))
            return;
            
        processedCommits.Add(commit.id);
        StartCoroutine(SpawnMeteorShower(commit.size * meteorsPerCommit));
    }
    
    private IEnumerator SpawnMeteorShower(int count)
    {
        isShowering = true;
        
        for (int i = 0; i < count; i++)
        {
            SpawnMeteor();
            yield return new WaitForSeconds(UnityEngine.Random.Range(spawnInterval * 0.5f, spawnInterval * 1.5f));
        }
        
        isShowering = false;
    }
    
    private void SpawnMeteor()
    {
        if (meteorPrefab == null)
            return;
            
        // 随机生成流星位置
        Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
        Vector3 spawnPosition = planetTransform.position + randomDirection * spawnHeight;
        
        // 计算目标位置（星球表面）
        Vector3 targetPosition = planetTransform.position + randomDirection * (planetTransform.localScale.x * 0.5f);
        
        // 创建流星并设置方向
        GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
        MeteorController controller = meteor.GetComponent<MeteorController>();
        
        if (controller != null)
        {
            controller.Initialize(targetPosition, meteorSpeed);
        }
    }
    
    // 提交数据结构
    [System.Serializable]
    public class CommitData
    {
        public string id;
        public string message;
        public int size; // 提交大小（可以是修改的文件数或代码行数）
    }
}
