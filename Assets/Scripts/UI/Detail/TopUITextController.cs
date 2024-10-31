using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopUITextController : MonoBehaviour
{
    Text testMesh;
    public void SetTopUITextController(float _openRank)
    {
        testMesh = GetComponent<Text>();
        testMesh.text = "OpenRank: " + (int)(_openRank);
    }
}
