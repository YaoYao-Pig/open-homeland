using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystemController : MonoBehaviour
{
    [SerializeField] private List<Achievement> achievementList;

    public delegate void AchievementUnlocked(Achievement achievement);
    public event AchievementUnlocked OnAchievementUnlocked;
    public static AchievementSystemController Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void UpdateProgress(string achievementId, int amount)
    {
        Achievement achievement = achievementList.Find(a => a.id == achievementId);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.currentProgress += amount;

            // 检查是否达成成就
            if (achievement.currentProgress >= achievement.targetProgress)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    public bool CheckAchieveStatus(string achievementId)
    {
        var achievement = achievementList.Find(x => x.id == achievementId);
        if (achievement == null) throw new System.Exception("AchievementSystemController: No Achievement");
        return achievement.isUnlocked;
    }
    private void UnlockAchievement(Achievement achievement)
    {
        Debug.Log(achievement.name);
        achievement.isUnlocked = true;
        achievement.currentProgress = achievement.targetProgress;

        OnAchievementUnlocked?.Invoke(achievement);
    }
    public List<Achievement> GetAchievements()
    {
        return achievementList;
    }
}
