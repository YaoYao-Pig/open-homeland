using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAction : MonoBehaviour
{

    private Material material;
    private DrawLineToParent[] drawLineToParents;
    private NodeComponent nodeComponent;

    private void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        nodeComponent = gameObject.GetComponent<NodeComponent>();
        if (nodeComponent!=null&&nodeComponent is RepoNodeComponent)
        {
            drawLineToParents=GetComponentsInChildren<DrawLineToParent>();
        }
    
    }

    private void OnMouseEnter()
    {
        material.SetFloat("_EdgeVisible", 1.0f);
        if (nodeComponent != null && nodeComponent is RepoNodeComponent)
        {
            foreach(var drawLine in drawLineToParents)
            {
                drawLine.Draw();
            }
            
        }
        

    }
    private void OnMouseExit()
    {
        material.SetFloat("_EdgeVisible", 0.0f);
        if (nodeComponent != null && nodeComponent is RepoNodeComponent)
        {
            foreach (var drawLine in drawLineToParents)
            {
                drawLine.ReFresh();
            }

        }
    }
}
