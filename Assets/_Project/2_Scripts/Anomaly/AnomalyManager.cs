using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AnomalyManager : MonoBehaviour
{
    //[SerializeField] private CarSpawner carSpawner;
    //public List<AnomalyBase> allAnomalies;
    //private AnomalyBase currentAnomaly;
    //private bool hasAnomaly;
    //[Header("Debug")]
    //public bool debugMode = false;
    //public int debugAnomalyIndex = -1;

    //// 이미 실행된 이상현상 기록 (재등장 방지)
    //private HashSet<AnomalyBase> seenAnomalies = new HashSet<AnomalyBase>();

    //public void SetupFloor()
    //{
    //    // 1. 모든 이상현상 강제 정리 (안전망)
    //    foreach (var anomaly in allAnomalies)
    //    {
    //        anomaly.Deactivate();
    //    }

    //    currentAnomaly = null;
    //    hasAnomaly = false;

    //    if (debugMode)
    //    {
    //        if (debugAnomalyIndex >= 0 && debugAnomalyIndex < allAnomalies.Count)
    //        {
    //            hasAnomaly = true;
    //            currentAnomaly = allAnomalies[debugAnomalyIndex];
    //            ArmAnomaly(currentAnomaly);
    //        }
    //        // ★ debugMode에서도 차량 스폰 처리
    //        ApplyCarSpawn();
    //        return;
    //    }

    //    // 이미 본 이상현상 제외한 풀에서 랜덤 선택
    //    hasAnomaly = Random.value > 0.5f;
    //    if (hasAnomaly)
    //    {
    //        List<AnomalyBase> availablePool = new List<AnomalyBase>();
    //        foreach (var a in allAnomalies)
    //        {
    //            if (!seenAnomalies.Contains(a)) availablePool.Add(a);
    //        }

    //        // 다 본 상태면 전체에서 선택 (풀 고갈 방지)
    //        if (availablePool.Count == 0)
    //            availablePool = allAnomalies;

    //        currentAnomaly = availablePool[Random.Range(0, availablePool.Count)];
    //        seenAnomalies.Add(currentAnomaly); // 히스토리 기록
    //        ArmAnomaly(currentAnomaly);
    //    }
    //    carSpawner.RespawnCars(currentAnomaly.ShouldSpawnCars);
    //}

    //private void ArmAnomaly(AnomalyBase anomaly)
    //{
    //    anomaly.Activate();
    //    if (anomaly.anomalyTrigger != null)
    //        anomaly.anomalyTrigger.ArmTrigger();
    //    else
    //        Debug.LogWarning($"[AnomalyManager] {anomaly.name}에 anomalyTrigger가 연결되지 않음");
    //}

    //public bool CheckPlayerChoice(bool playerSaysAnomaly)
    //{
    //    return playerSaysAnomaly == hasAnomaly;
    //}

    ///// <summary>
    ///// 이상현상 히스토리 초기화 (필요 시 - 예: 완전 리셋).
    ///// 사망으로 인한 리셋에는 호출하지 않음 (본 것은 본 것).
    ///// </summary>
    //public void ClearHistory()
    //{
    //    seenAnomalies.Clear();
    //}

    ///// <summary>
    ///// 디버그용: 특정 인덱스의 이상현상으로 강제 SetupFloor.
    ///// -1이면 이상현상 없음.
    ///// </summary>
    //public void DebugForceAnomaly(int index)
    //{
    //    // 모든 이상현상 정리
    //    foreach (var anomaly in allAnomalies)
    //        anomaly.Deactivate();

    //    currentAnomaly = null;
    //    hasAnomaly = false;

    //    if (index < 0 || index >= allAnomalies.Count)
    //    {
    //        // None 처리
    //        return;
    //    }

    //    hasAnomaly = true;
    //    currentAnomaly = allAnomalies[index];
    //    ArmAnomaly(currentAnomaly);
    //}

    ///// <summary>
    ///// 외부에서 현재 이상현상 조회용
    ///// </summary>
    //public AnomalyBase GetCurrentAnomaly() => currentAnomaly;

    ///// <summary>
    ///// 이상현상 풀 조회용
    ///// </summary>
    //public List<AnomalyBase> GetAllAnomalies() => allAnomalies;
    ///// <summary>
    ///// 현재 이상현상에 따라 차량 스폰 처리
    ///// </summary>
    //private void ApplyCarSpawn()
    //{
    //    if (currentAnomaly != null)
    //    {
    //        // 이상현상이 있으면 그 설정 따름
    //        carSpawner.RespawnCars(currentAnomaly.ShouldSpawnCars);
    //    }
    //    else
    //    {
    //        // 이상현상 없으면 기본 (차량 스폰)
    //        carSpawner.RespawnCars(true);
    //    }
    //}
    private bool hasAnomaly;

    public bool HasAnomaly() => hasAnomaly;
}