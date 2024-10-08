using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject hubNamePrefab;
    private Transform parentUI;
    
    private GameObject hubNameObj;
    private HUDShow hubShow;
    public bool showHud = false;

    private void Awake()
    {

    }


    private void InitializedHud()
    {
        parentUI = GameObject.Find("HUD").transform;
        hubNameObj = Instantiate(hubNamePrefab, parentUI);
        hubShow = hubNameObj.GetComponent<HUDShow>();
    }

    private void Update()
    {
        if (showHud)
        {
            if (hubNameObj == null)
            {
                InitializedHud();
            }
            hubNameObj.SetActive(true);
            hubShow.UpdateHud(transform,gameObject.name);
        }
        else
        {
            if (hubNameObj != null) hubNameObj.SetActive(false);
        }
    }
}
