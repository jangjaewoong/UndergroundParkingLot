using UnityEngine;

/// <summary>
/// 플레이어 리스폰 지점 마커.
/// Empty GameObject에 붙이면 됨. 위치/회전이 리스폰 기준.
/// </summary>
public class RespawnPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Scene 뷰에서 보이게
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
    }
}