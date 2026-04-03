// 모든 이상현상이 상속받는 베이스
using UnityEngine;

public abstract class AnomalyBase : MonoBehaviour
{
    public abstract void Activate();    // 이상현상 등장
    public abstract void Deactivate();  // 이상현상 제거
    public abstract bool IsAnomaly();   // 이상현상 여부 반환
}