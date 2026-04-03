using System.Collections.Generic;
using UnityEngine;

/*
  이상현상 관리 스크립트
debugMode 선택시 원하는 이상현상 바로 실행 가능.
 */
public class AnomalyManager : MonoBehaviour
{
    public List<AnomalyBase> allAnomalies;
    private AnomalyBase currentAnomaly;
    private bool hasAnomaly;

    [Header("Debug")]
    public bool debugMode = false;
    public int debugAnomalyIndex = -1; // -1 = 랜덤, 0~n = 특정 이상현상

    public void SetupFloor()
    {
        if (currentAnomaly != null)
            currentAnomaly.Deactivate();

        if (debugMode)
        {
            // 인덱스 -1이면 이상현상 없음, 0이상이면 해당 인덱스
            if (debugAnomalyIndex >= 0 && debugAnomalyIndex < allAnomalies.Count)
            {
                hasAnomaly = true;
                currentAnomaly = allAnomalies[debugAnomalyIndex];
                currentAnomaly.Activate();
            }
            else
            {
                hasAnomaly = false;
                currentAnomaly = null;
            }
            return;
        }

        // 기존 랜덤 로직
        hasAnomaly = Random.value > 0.5f;
        if (hasAnomaly)
        {
            currentAnomaly = allAnomalies[Random.Range(0, allAnomalies.Count)];
            currentAnomaly.Activate();
        }
    }

    public bool CheckPlayerChoice(bool playerSaysAnomaly)
    {
        return playerSaysAnomaly == hasAnomaly; // 플레이어가 이상현상 true/false로 판단한 결과가 실제 값과 같은지 비교
    }
}