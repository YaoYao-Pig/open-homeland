using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : MonoBehaviour
{

    private NodeComponent nodeComponent;
    private Material material;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // Start is called before the first frame update
    void Start()
    {
        nodeComponent = GetComponent<NodeComponent>();
        if(nodeComponent!=null&&nodeComponent is UserNodeComponent)
        {
            material.SetColor("_Color", Color.yellow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
