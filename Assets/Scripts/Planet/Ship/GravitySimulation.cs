using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    public GameObject planet; // 星球对象
    private float gravitationalConstant = -10f; // 引力常数
    private Rigidbody rb;
    private void Awake()
    {
        planet = GameObject.Find("CenterPlanet");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (planet != null)
        {
            // 计算与星球的距离
            Vector3 directionToPlanet = (transform.position-planet.transform.position);
            Vector3 force = directionToPlanet.normalized * 10;

            Vector3 bodyUp = transform.up;
            transform.rotation = Quaternion.FromToRotation(bodyUp, directionToPlanet) * transform.rotation;
            // 应用引力
            rb.AddForce(gravitationalConstant*force);
        }
    }
}

