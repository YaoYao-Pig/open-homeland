using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonUIRotate : MonoBehaviour
{
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localRotation *= Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.forward);
    }
}
