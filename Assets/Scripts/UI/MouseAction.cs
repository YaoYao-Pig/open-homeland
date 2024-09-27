using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAction : MonoBehaviour
{

    private Material material;
    private void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
    }




    private void OnMouseEnter()
    {
        material.SetFloat("_EdgeVisible", 1.0f);
    }
    private void OnMouseExit()
    {
        material.SetFloat("_EdgeVisible", 0.0f);
    }
}
