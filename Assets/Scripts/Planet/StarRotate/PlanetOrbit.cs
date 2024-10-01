using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform centerPlanet; // 中心星球
    public float orbitDistance = 5f; // 星球与中心星球之间的距离
    public float orbitSpeed = 30f; // 公转速度（度/秒）

    private float angle = 0f; // 当前角度

    void Update()
    {
        // 更新角度
        angle += orbitSpeed * Time.deltaTime;

        // 计算新的位置
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * orbitDistance, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * orbitDistance);

        // 更新星球位置
        transform.position = centerPlanet.position + offset;
    }
}
