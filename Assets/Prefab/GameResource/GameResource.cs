using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResource : MonoBehaviour
{
    public static GameResource Instance;
    private void Awake()
    {
        Instance = this;
    }

    [Header("UI")]
    public GameObject mainUserUIRoot;
    public Animator mainUserUIAnimator;
}
