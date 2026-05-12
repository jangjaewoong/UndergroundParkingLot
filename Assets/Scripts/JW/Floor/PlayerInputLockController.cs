using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// 플레이어 입력 잠금 전담 싱글톤.
/// DeathCutsceneManager, ElevatorController 등 여러 시스템이 공용으로 사용.
/// </summary>
public class PlayerInputLockController : MonoBehaviour
{
    public static PlayerInputLockController Instance { get; private set; }

    [Header("References")]
    public PlayerController playerController;
    public CinemachinePanTilt playerPanTilt;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 플레이어 입력 전체 잠금/해제.
    /// - PlayerController: 이동 + 몸체 회전 차단
    /// - CinemachinePanTilt: 마우스 카메라 회전 차단
    /// </summary>
    public void SetLocked(bool locked)
    {
        if (playerController != null)
            playerController.SetInputLocked(locked);

        if (playerPanTilt != null)
            playerPanTilt.enabled = !locked;
    }
}