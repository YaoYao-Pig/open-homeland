using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    public GameObject planet; // �������
    private float gravitationalConstant = -10f; // ��������
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
            // ����������ľ���
            Vector3 directionToPlanet = (transform.position-planet.transform.position);
            Vector3 force = directionToPlanet.normalized * 10;

            Vector3 bodyUp = transform.up;
            transform.rotation = Quaternion.FromToRotation(bodyUp, directionToPlanet) * transform.rotation;
            // Ӧ������
            rb.AddForce(gravitationalConstant*force);
        }
    }
}

