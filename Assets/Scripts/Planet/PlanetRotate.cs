using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    public float rotateSpeed = 1f;

    public void Rotate()
    {
        Quaternion quaterniony = Quaternion.Euler(0,rotateSpeed * Time.deltaTime, 0);
        Quaternion quaternionx= Quaternion.Euler(rotateSpeed * Time.deltaTime,0,  0);
        transform.rotation *= quaternionx*quaterniony;
    }
    private void Update()
    {
        Rotate();
    }
}
