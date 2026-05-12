using UnityEngine;

/// <summary>
/// Timeline Signal에서 호출하기 위한 문 제어 프록시.
/// Animator의 Bool 파라미터를 토글하는 단순한 래퍼.
/// </summary>
public class DoorAnimatorProxy : MonoBehaviour
{
    [Tooltip("엘레베이터 문 Animator")]
    public Animator doorAnimator;

    [Tooltip("Bool 파라미터 이름")]
    public string paramName = "isOpen";

    public void OpenDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetBool(paramName, true);
    }

    public void CloseDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetBool(paramName, false);
    }
}