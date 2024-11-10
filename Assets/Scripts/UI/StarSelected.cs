using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSelected : MonoBehaviour
{
    [SerializeField] private Material searchSelectedMaterial;
    private Material material;
    
    public void SetSelected()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
        gameObject.GetComponent<MeshRenderer>().material = searchSelectedMaterial;
    }
    public void UnSelelected()
    {
        gameObject.GetComponent<MeshRenderer>().material= material;
    }
}
