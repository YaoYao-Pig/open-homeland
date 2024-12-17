using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AchievementUIList : MonoBehaviour
{
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject achievementItemPrefab;

    public void OnAchievementTableOpen()
    {
        ClearAchievementTable();
        List<Achievement> achievements= AchievementSystemController.Instance.GetAchievements();
        foreach(var a in achievements)
        {
            GameObject g= Instantiate(achievementItemPrefab, achievementPanel.transform);
            Image[] images= g.GetComponentsInChildren<Image>();
            foreach(var i in images)
            {
                if (i.name == "Icon")
                {
                    i.sprite = a.icon;
                }

                if (i.name == "Mask")
                {
                    if (a.isUnlocked)
                    {
                        i.gameObject.SetActive(false);
                    }
                    else
                    {
                        i.gameObject.SetActive(true);
                    }
                }
            }
            
            TextMeshProUGUI[] ts = g.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(var t in ts)
            {
                if(t.name== "achieveName")
                {
                    t.text = a.name;
                }
                if(t.name== "achieveDescription")
                {
                    t.text = a.description;
                }
            }
        }
    }
    private void ClearAchievementTable()
    {
        foreach(Transform p in achievementPanel.transform)
        {
            Destroy(p.gameObject);
        }
    }
}
