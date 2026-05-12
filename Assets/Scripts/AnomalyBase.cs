// 모든 이상현상이 상속받는 베이스
using UnityEngine;

public abstract class AnomalyBase : MonoBehaviour
{
    [Header("Anomaly Trigger")]

    [Header("이상현상 설정")]
    [SerializeField] protected bool spawnCars = true; // ★ 추가

    public bool ShouldSpawnCars => spawnCars;
    [Header("Anomaly Content")]
    [Tooltip("이 이상현상의 시각/청각 요소를 모은 부모 GameObject. Arm 시 활성화, Disarm 시 비활성화")]
    public GameObject anomalyContent;

    public abstract void Activate();    // 이상현상 등장
    public abstract void Deactivate();  // 이상현상 제거
    public abstract bool IsAnomaly();   // 이상현상 여부 반환
    /// <summary>
    /// 트리거 박스에 플레이어가 닿았을 때 호출됨.
    /// 실제 이상현상 진행(타이머, 물 상승 등)은 여기서 시작.
    /// </summary>
    public virtual void OnTriggered() { }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }
    /// <summary>
    /// 콘텐츠 부모 활성/비활성 헬퍼.
    /// 자식 클래스의 Activate/Deactivate에서 호출.
    /// </summary>
    protected void SetContentActive(bool active)
    {
        if (anomalyContent != null)
            anomalyContent.SetActive(active);
    }
}