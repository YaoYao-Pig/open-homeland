using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform centerPlanet; // ��������
    public float orbitDistance = 5f; // ��������������֮��ľ���
    public float orbitSpeed = 30f; // ��ת�ٶȣ���/�룩

    private float angle = 0f; // ��ǰ�Ƕ�

    void Update()
    {
        // ���½Ƕ�
        angle += orbitSpeed * Time.deltaTime;

        // �����µ�λ��
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * orbitDistance, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * orbitDistance);

        // ��������λ��
        transform.position = centerPlanet.position + offset;
    }
}
