using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform centerPlanet; // ��������
    public float orbitDistance = 5f; // ��������������֮��ľ���
    public float orbitSpeed = 30f; // ��ת�ٶȣ���/�룩

    private float angle = 0f; // ��ǰ�Ƕ�
    [SerializeField] private float offsetAngle = 100f;//Z��ƫ�Ƶ����ֵ

    void Update()
    {
        // ���½Ƕ�
        angle += orbitSpeed * Time.deltaTime;

        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * orbitDistance;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * orbitDistance;
        float y = offsetAngle * Mathf.Sin(angle * Mathf.Deg2Rad);
        // �����µ�λ��
        Vector3 offset = new Vector3(
            x,
            y,
            z);


        // ��������λ��
        transform.position = centerPlanet.position + offset;
    }
}
