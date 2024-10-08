using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDShow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 2, 0);
    public void UpdateHud(Transform objectFollow,string hudName)
    {
        
        GetComponent<TextMeshProUGUI>().text = hudName;
        
        Vector3 pos = Camera.main.WorldToScreenPoint(objectFollow.position + offset);
        transform.position = pos;
    }
}
