using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievement 
{
    public string id;            // �ɾ͵�Ψһ��ʶ
    public string name;          // �ɾ�����
    public string description;   // �ɾ�����
    public Sprite icon;          // �ɾ�ͼ��
    public bool isUnlocked;      // �Ƿ��ѽ���
    public int currentProgress;  // ��ǰ����
    public int targetProgress;   // ��������Ŀ�����

}
