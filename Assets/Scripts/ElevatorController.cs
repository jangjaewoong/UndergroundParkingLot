using System.Collections;
using UnityEngine;
using UnityEngine.Playables; // PlayableDirector 사용하기 위한 네임스페이스

public class ElevatorController : Singleton<ElevatorController>
{
    // Elevator Animaton. 열림, 닫힘 존재
    // 탑승후 도착, 리스폰 제외한 상황에서는 항상 열려있는 상홤
    // 탑승 후 버튼 눌러야 움직임.
    // 탑승 후 버튼 누르면 타임라인 하나 재생 (문 닫힘 - 이동 소리, 흔들림 - 도착 문 열림 )
    // 이때 버튼 눌렀을때 엘레베이터 문 바라보는 쪽으로 자연스럽게 카메라 트랜지션, 못움직임
    // 문 닫히고 나서는 자유행동 가능
    // 도착 후 문열림 타임라인과 탑승 후 문닫힘 - 이동 타임라인을 따로만들어야 할듯
    // 그래야 리스폰시 문열림 타임라인만 사용할 수 있음.
    private bool isOpen = true;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private PlayableDirector departureTimeline;
    [SerializeField] private PlayableDirector arrivalTimeline = null;

    private void Start()
    {
        doorAnimator.SetBool("isOpen", isOpen);
        doorAnimator.Play("Open", 0, 0f);
        doorAnimator.Update(0f);
        // 시작하자마자 바로 Open 되게끔. 아니면 그냥 Animator에서 바로 Open으로 연결하는 방법도 존재. 이게 나을 수 도 있음.
    }

    public void requestDeparture()
    {
        departureTimeline.Play();
        Debug.Log("엘레베이터 출발!");
    }



}