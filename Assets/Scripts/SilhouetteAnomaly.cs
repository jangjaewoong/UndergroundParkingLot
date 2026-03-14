using UnityEngine;

/// <summary>
/// 이상현상: 저 멀리 서 있는 사람 실루엣
/// - 플레이어가 가까이 다가가면 순간 사라짐
/// - 플레이어가 뒤돌면 바로 뒤에 서있음 + 소름 사운드
/// - 3초 후 그냥 사라짐 → 플레이어가 출구로 판단하러 감
/// </summary>
public class SilhouetteAnomaly : MonoBehaviour
{
    [Header("위치 설정")]
    public Transform spawnPointFar;        // 처음 멀리 서 있는 위치
    public Transform spawnPointBehind;     // 뒤에 나타날 위치 (비우면 자동으로 플레이어 뒤)
    public float disappearDistance = 6f;   // 이 거리 이하로 가까워지면 사라짐

    [Header("사운드 설정")]
    public AudioClip behindSound;          // 뒤에 나타날 때 소름돋는 배경음
    public float behindStandDuration = 3f; // 뒤에 서있는 시간 (초)

    [Header("시야 설정")]
    public float behindCheckAngle = 100f;  // 플레이어가 뒤돌았는지 판단하는 각도

    [Header("딜레이 설정")]
    public float reappearDelay = 1.5f;     // 사라진 후 뒤에 나타나기까지 대기 시간

    [Header("플레이어 설정")]
    public Transform player;               // Inspector에서 직접 연결
    public Camera playerCamera;            // 플레이어 카메라

    private AudioSource audioSource;
    private Renderer[] meshRenderers;      // SetActive 대신 렌더러 토글

    private enum State
    {
        Idle,           // 비활성
        ShowingFar,     // 멀리 서 있는 상태
        Hidden,         // 사라진 후 대기 중
        ShowingBehind,  // 뒤에 서있는 상태
    }

    private State currentState = State.Idle;
    private float stateTimer = 0f;
    private bool behindTriggered = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 렌더러 전부 수집 (자식 포함)
        meshRenderers = GetComponentsInChildren<Renderer>();

        // Inspector에서 연결 안 됐으면 태그로 자동 탐색
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // 시작할 때 안 보이게
        SetVisible(false);
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.ShowingFar:
                UpdateShowingFar();
                break;

            case State.Hidden:
                UpdateHidden();
                break;

            case State.ShowingBehind:
                UpdateShowingBehind();
                break;
        }
    }

    // ───────────────────────────────────────────────
    // State: ShowingFar — 멀리 서 있음
    // ───────────────────────────────────────────────
    void UpdateShowingFar()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= disappearDistance)
        {
            SetVisible(false);
            SetState(State.Hidden);
        }
    }

    // ───────────────────────────────────────────────
    // State: Hidden — 사라진 후 대기
    // ───────────────────────────────────────────────
    void UpdateHidden()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            PlaceBehindPlayer();
            SetState(State.ShowingBehind);
        }
    }

    // ───────────────────────────────────────────────
    // State: ShowingBehind — 뒤에 서있음
    // 플레이어가 뒤돌면 소름 사운드 재생, 3초 후 사라짐
    // ───────────────────────────────────────────────
    void UpdateShowingBehind()
    {
        // 플레이어가 뒤돌아서 실루엣을 봤을 때 사운드 재생 (1번만)
        if (!behindTriggered && IsPlayerLookingAt(transform.position))
        {
            behindTriggered = true;
            PlayBehindSound();
        }

        // 3초 후 사라짐
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            SetVisible(false);
            SetState(State.Idle);
        }
    }

    // ───────────────────────────────────────────────
    // 내부 함수들
    // ───────────────────────────────────────────────

    void SetVisible(bool visible)
    {
        if (meshRenderers == null) return;
        foreach (var r in meshRenderers)
            r.enabled = visible;
    }

    bool IsPlayerLookingAt(Vector3 targetPos)
    {
        if (playerCamera == null) return false;

        Vector3 dirToTarget = (targetPos - playerCamera.transform.position).normalized;
        float angle = Vector3.Angle(playerCamera.transform.forward, dirToTarget);
        return angle < behindCheckAngle / 2f;
    }

    void PlaceBehindPlayer()
    {
        if (spawnPointBehind != null)
        {
            transform.position = spawnPointBehind.position;
            transform.rotation = spawnPointBehind.rotation;
        }
        else
        {
            // 동적으로 플레이어 바로 뒤에 배치
            Vector3 behind = player.position - player.forward * 1.5f;
            behind.y = player.position.y;
            transform.position = behind;

            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }

        SetVisible(true);
    }

    void PlayBehindSound()
    {
        if (behindSound != null && audioSource != null)
            audioSource.PlayOneShot(behindSound, 3.0f);
    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.ShowingFar:
                transform.position = spawnPointFar != null
                    ? spawnPointFar.position
                    : transform.position;
                transform.rotation = spawnPointFar != null
                    ? spawnPointFar.rotation
                    : transform.rotation;
                SetVisible(true);
                break;

            case State.Hidden:
                stateTimer = reappearDelay;
                break;

            case State.ShowingBehind:
                behindTriggered = false;
                stateTimer = behindStandDuration;
                PlayBehindSound();
                break;

            case State.Idle:
                break;
        }
    }

    // ───────────────────────────────────────────────
    // 외부 호출 (GameManager에서 사용)
    // ───────────────────────────────────────────────

    public void Activate()
    {
        behindTriggered = false;
        SetState(State.ShowingFar);
    }

    public void ResetAnomaly()
    {
        currentState = State.Idle;
        stateTimer = 0f;
        behindTriggered = false;
        SetVisible(false);

        if (audioSource != null)
            audioSource.Stop();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, disappearDistance);
    }
}