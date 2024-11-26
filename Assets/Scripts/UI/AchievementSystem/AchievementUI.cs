using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementItemUI;
    [SerializeField] private AchievementUIList achievementTable;

    // Start is called before the first frame update
    void Start()
    {
        AchievementSystemController.Instance.OnAchievementUnlocked += ShowAchievementUI;
    }
    private void ShowAchievementUI(Achievement achievement)
    {

        achievementItemUI.SetActive(true);
        TextMeshProUGUI text = achievementItemUI.GetComponentInChildren<TextMeshProUGUI>();
        Image[] images= achievementItemUI.GetComponentsInChildren<Image>();
        Image icon = null;
        foreach (var i in images)
        {
            if (i.name == "Icon") icon = i;

        }
         
        icon.sprite = achievement.icon;
        text.text = achievement.name;

        // ����Э������ UI
        StartCoroutine(HideAchievementUIAfterDelay(3.0f)); // 3�������

    }
    private IEnumerator HideAchievementUIAfterDelay(float delay)
    {
        // �ȴ�ָ��������
        yield return new WaitForSeconds(delay);

        // ���سɾ�UI
        achievementItemUI.SetActive(false);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(achievementTable.gameObject.activeSelf == false)
            {
                achievementTable.gameObject.SetActive(true);
                achievementTable.OnAchievementTableOpen();
            }
            else
            {
                achievementTable.gameObject.SetActive(false);
            }
        }
    }
}
