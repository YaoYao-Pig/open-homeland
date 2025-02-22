using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAutoSize : LayoutGroup
{
    [SerializeField] private TextMeshProUGUI text;

    public override void CalculateLayoutInputVertical()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, text.preferredHeight);
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
        
    }
}
