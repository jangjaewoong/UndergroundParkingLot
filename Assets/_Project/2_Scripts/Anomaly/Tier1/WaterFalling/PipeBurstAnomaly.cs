using UnityEngine;
using System.Collections;
public class PipeBurstAnomaly : AnomalyBase
{
    [Header("Horizontal Pipes (2 rows)")]
    public GameObject horizontalRow1Parent;  // 위쪽 줄
    public GameObject horizontalRow2Parent;  // 아래쪽 줄

    [Header("Vertical Pipes (2 rows)")]
    public GameObject verticalRow1Parent;    // 왼쪽 줄
    public GameObject verticalRow2Parent;    // 오른쪽 줄

    [Header("Connected Anomaly")]
    public Tier1_WaterFalling waterFallingAnomaly;

    [Header("Timing")]
    [SerializeField] private float intervalBetweenSteps = 0.3f;
    [SerializeField] private float waterRiseDelayAfterBurst = 1f;

    private PipeBurstUnit[] horizontalRow1;  // 7개
    private PipeBurstUnit[] horizontalRow2;  // 7개
    private PipeBurstUnit[] verticalRow1;    // 4개
    private PipeBurstUnit[] verticalRow2;    // 4개

    private bool isRunning = false;

    void Awake()
    {
        horizontalRow1 = horizontalRow1Parent.GetComponentsInChildren<PipeBurstUnit>(true);
        horizontalRow2 = horizontalRow2Parent.GetComponentsInChildren<PipeBurstUnit>(true);
        verticalRow1 = verticalRow1Parent.GetComponentsInChildren<PipeBurstUnit>(true);
        verticalRow2 = verticalRow2Parent.GetComponentsInChildren<PipeBurstUnit>(true);
    }

    public override void OnTriggered()
    {
        if (isRunning) return;
        Activate();
    }

    public override void Activate()
    {
        isRunning = true;
        StartCoroutine(BurstSequence());
    }

    // 가로 두 줄을 같은 인덱스로 동시에 터뜨리는 헬퍼
    private void TriggerHorizontalAt(int index)
    {
        horizontalRow1[index].Trigger();
        horizontalRow2[index].Trigger();
    }

    // 세로 두 줄을 같은 인덱스로 동시에 터뜨리는 헬퍼
    private void TriggerVerticalAt(int index)
    {
        verticalRow1[index].Trigger();
        verticalRow2[index].Trigger();
    }

    private IEnumerator BurstSequence()
    {
        // 가로: 중앙(3) → 양옆(2,4) → (1,5) → (0,6)
        TriggerHorizontalAt(3);
        yield return new WaitForSeconds(intervalBetweenSteps);

        TriggerHorizontalAt(2);
        TriggerHorizontalAt(4);
        yield return new WaitForSeconds(intervalBetweenSteps);

        TriggerHorizontalAt(1);
        TriggerHorizontalAt(5);
        yield return new WaitForSeconds(intervalBetweenSteps);

        TriggerHorizontalAt(0);
        TriggerHorizontalAt(6);
        yield return new WaitForSeconds(intervalBetweenSteps);

        // 세로: 안쪽(1,2) → 바깥(0,3)
        TriggerVerticalAt(1);
        TriggerVerticalAt(2);
        yield return new WaitForSeconds(intervalBetweenSteps);

        TriggerVerticalAt(0);
        TriggerVerticalAt(3);

        // 배관 다 터지고 → 물 차오르기
        yield return new WaitForSeconds(waterRiseDelayAfterBurst);
        if (waterFallingAnomaly != null)
            waterFallingAnomaly.StartWaterRising();
    }

    public override void Deactivate()
    {
        StopAllCoroutines();
        // 모든 배관 즉시 정리
        ResetAllPipes(horizontalRow1);
        ResetAllPipes(horizontalRow2);
        ResetAllPipes(verticalRow1);
        ResetAllPipes(verticalRow2);
        isRunning = false;
    }


    private void ResetAllPipes(PipeBurstUnit[] pipes)
    {
        foreach (var pipe in pipes) pipe.ResetBurst();
    }


    public override bool IsAnomaly() => isRunning;
}