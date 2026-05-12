using TMPro;
using UnityEngine;



/// <summary>
/// 층 상태 관리 + 정답 판정.
/// 연출(사망/리스폰/터널 통과)은 각 Controller에 위임.
/// </summary>
public class FloorManager : Singleton<FloorManager>
{
    //public static FloorManager Instance { get; private set; }

    //[Header("Floor State")]
    //public AnomalyManager anomalyManager;
    //public int currentFloor = 7;

    //[Header("Clear")]
    //[Tooltip("클리어 도달 층 (B1)")]
    //public int clearFloor = 1;

    //[Header("Tunnel Availability")]
    //[Tooltip("내려가는 터널 트리거. B7에선 비활성")]
    //public PassageTrigger downTunnelTrigger;
    //[Tooltip("올라가는 터널 트리거. 항상 활성")]
    //public PassageTrigger upTunnelTrigger;
    //[Tooltip("내려가는 터널  B7에서만 비활성")]
    //public GameObject downTunnel;

    //[Header("Respawn Points")]
    //[Tooltip("엘레베이터 리스폰 지점 (사망 후 또는 정답 엘베)")]
    //public RespawnPoint EleRespawnPoint;
    //[Tooltip("터널 리스폰 지점 (정답 터널)")]
    //public RespawnPoint TunnelRespawnPoint;

    //[Header("Floor Display")]
    //[SerializeField] private TMP_Text floorDisplayText1;
    //[SerializeField] private TMP_Text floorDisplayText2;

    //void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}

    //void Start()
    //{
    //    anomalyManager.SetupFloor();
    //    UpdateTunnelAvailability();
    //    UpdateFloorDisplay();
    //}

    //// ============================================================
    //// 정답 판정 + 층 상태 갱신
    //// Controller(Elevator/Tunnel)가 호출. 결과(bool)에 따라 후속 연출 결정.
    //// ============================================================

    ///// <summary>
    ///// 통과 지점에서 호출. 정답이면 currentFloor 감소 + SetupFloor 호출.
    ///// 정답/오답 결과 반환 (Controller가 후속 처리 결정).
    ///// </summary>
    //public bool CheckPassageAndAdvance(PassageType passage)
    //{
    //    bool hasAnomaly = anomalyManager.HasAnomaly();
    //    bool isCorrect = IsCorrectChoice(passage, hasAnomaly);

    //    Debug.Log($"[FloorManager] Passage {passage}, 이상현상={hasAnomaly}, 결과={(isCorrect ? "정답" : "오답")}");

    //    if (isCorrect)
    //    {
    //        currentFloor--;

    //        if (currentFloor <= clearFloor)
    //        {
    //            OnGameClear();
    //            return true;
    //        }

    //        anomalyManager.SetupFloor();
    //        UpdateTunnelAvailability();
    //        UpdateFloorDisplay();
    //    }

    //    return isCorrect;
    //}

    //private bool IsCorrectChoice(PassageType passage, bool hasAnomaly)
    //{
    //    if (hasAnomaly)
    //        return passage == PassageType.Elevator;
    //    else
    //        return passage == PassageType.UpTunnel;
    //}

    //private void OnGameClear()
    //{
    //    Debug.Log("[FloorManager] CLEAR!");
    //    // TODO: 클리어 연출
    //}

    //// ============================================================
    //// B7 리셋 (사망 시 DeathCutsceneManager가 호출)
    //// ============================================================

    ///// <summary>
    ///// 사망 시 호출. 층을 B7로 리셋하고 이상현상 재배정.
    ///// 이상현상 히스토리는 AnomalyManager가 관리.
    ///// </summary>
    //public void ResetToB7()
    //{
    //    currentFloor = 7;
    //    anomalyManager.SetupFloor();
    //    UpdateTunnelAvailability();
    //    UpdateFloorDisplay();
    //}

    //// ============================================================
    //// 리스폰 지점 조회 (DeathCutsceneManager가 호출)
    //// ============================================================

    //public Transform GetEleRespawnTransform()
    //{
    //    return EleRespawnPoint != null ? EleRespawnPoint.transform : null;
    //}

    //public Transform GetTunnelRespawnTransform()
    //{
    //    return TunnelRespawnPoint != null ? TunnelRespawnPoint.transform : null;
    //}

    //// ============================================================
    //// 층별 터널 가용성 / 층 표시
    //// ============================================================

    ///// <summary>
    ///// 현재 층에 맞춰 터널 활성/비활성 갱신.
    ///// B7(맨 아래층)일 때 내려가는 터널 막힘.
    ///// </summary>
    //private void UpdateTunnelAvailability()
    //{
    //    bool isBottomFloor = (currentFloor == 7);

    //    // 내려가는 터널 트리거: B7에서만 비활성
    //    if (downTunnelTrigger != null)
    //    {
    //        if (isBottomFloor)
    //            downTunnelTrigger.Disarm();
    //        else
    //            downTunnelTrigger.Arm();
    //    }

    //    // 막힘 시각 표현: B7에서만 활성
    //    if (downTunnel != null)
    //        downTunnel.SetActive(isBottomFloor);

    //    // 올라가는 터널: 항상 활성
    //    if (upTunnelTrigger != null)
    //        upTunnelTrigger.Arm();
    //}

    //private void UpdateFloorDisplay()
    //{
    //    if (floorDisplayText1 != null)
    //        floorDisplayText1.text = $"B{currentFloor}";
    //    if (floorDisplayText2 != null)
    //        floorDisplayText2.text = $"B{currentFloor}";
    //}

    //// ============================================================
    //// Debug 메서드
    //// ============================================================

    //[ContextMenu("Debug - Reset to B7")]
    //private void DebugResetToB7()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        Debug.LogWarning("[FloorManager] 플레이 모드에서만 동작합니다.");
    //        return;
    //    }
    //    ResetToB7();
    //}

    //[ContextMenu("Debug - Print Current Floor")]
    //private void DebugPrintCurrentFloor()
    //{
    //    Debug.Log($"[FloorManager] currentFloor = B{currentFloor}");
    //}

    [SerializeField] private int currentFloor = 7;
    [SerializeField] private int clearFloor = 0;
    [SerializeField] private AnomalyManager anomalyManager;

    void ChangeFloor(string path)
    {
        if (isCorrectPath(path))
        {
            FloorUp();
        }
        else
        {
            ResetToB7();
        }

    }

    bool isCorrectPath(string path)
    {
        if (anomalyManager.HasAnomaly() && path == "Elevator")
        {
            return true;
        }
        else if (!anomalyManager.HasAnomaly() && path == "upTunnel")
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void FloorUp()
    {
        if (currentFloor != clearFloor)
        {
            currentFloor--;
        }
        else
        {
            Clear();
        }
        
    }

    void ResetToB7()
    {
        currentFloor = 7;
    }

    

    void Clear()
    {
        Debug.Log("로비 도달 : 엔딩");
    }
}