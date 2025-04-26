using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private TrailRenderer trail;
    private ParticleSystem impactParticles;
    
    [SerializeField] private GameObject impactEffectPrefab;
    
    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        impactParticles = GetComponentInChildren<ParticleSystem>();
    }
    
    public void Initialize(Vector3 target, float moveSpeed)
    {
        targetPosition = target;
        speed = moveSpeed;
        
        // 设置流星朝向目标
        transform.LookAt(targetPosition);
    }
    
    private void Update()
    {
        // 移动流星
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // 当接近星球表面时销毁
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            // 创建撞击效果
            CreateImpactEffect();
            Destroy(gameObject);
        }
    }
    
    private void CreateImpactEffect()
    {
        if (impactEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);
            Destroy(impactEffect, 2f); // 2秒后销毁撞击效果
        }
        else if (impactParticles != null)
        {
            // 分离粒子系统，使其不随流星一起销毁
            impactParticles.transform.parent = null;
            impactParticles.Play();
            Destroy(impactParticles.gameObject, 2f);
        }
    }
}
