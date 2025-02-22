using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    private TextMeshProUGUI text;


    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Inite(string str)
    {
        text.text = str;
    }
}
