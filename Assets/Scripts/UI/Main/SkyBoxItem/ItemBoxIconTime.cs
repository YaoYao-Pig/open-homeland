using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxIconTime : MonoBehaviour
{
    [SerializeField] private List<Material> skyboxList;
    public float blendSpeed = 10.0f;

    private Material blendedSkybox; // ���ڶ�̬��ϵĲ���

    private bool startBlend = false;

    private int currentSkyboxIndex = 1;
    private int nextSkyboxIndex = 2; // ��һ����պ�����
    private float blendFactor = 0.0f;
    private void Start()
    {
        blendedSkybox = skyboxList[currentSkyboxIndex];
        
    }
    void Update()
    {
        
        if (startBlend && skyboxList != null)
        {
            blendFactor = Mathf.PingPong(Time.time * blendSpeed, 1.0f);
            
            if (blendFactor >= 1.0f)
            {
                // ������ɣ��л�����һ����պ�
                blendFactor = 0.0f;
                currentSkyboxIndex = nextSkyboxIndex;
                nextSkyboxIndex = (nextSkyboxIndex + 1) % skyboxList.Count; // ѭ������
            }
            blendedSkybox.Lerp(skyboxList[currentSkyboxIndex], skyboxList[nextSkyboxIndex], blendFactor);
            RenderSettings.skybox = blendedSkybox;
        }
    }
    public void SetStartBlend()
    {
        startBlend = !startBlend;
    }
    
}
