using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxIconClick : MonoBehaviour
{
    [SerializeField] private Material skyBoxMaterial;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite iconSprite;
    private void Start()
    {
        iconImage.sprite = iconSprite;
    }
    public Material GetSelectedSkyBox()
    {
        return skyBoxMaterial;
    }

    public void SelectNewSkyBox()
    {
        SkyBoxController.Instance.SetSkyBox(skyBoxMaterial);
    }

}
