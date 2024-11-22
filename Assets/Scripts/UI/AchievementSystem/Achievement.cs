using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement 
{
    public string id;            // 成就的唯一标识
    public string name;          // 成就名称
    public string description;   // 成就描述
    public Sprite icon;          // 成就图标
    public bool isUnlocked;      // 是否已解锁
    public int currentProgress;  // 当前进度
    public int targetProgress;   // 完成所需的目标进度

}
