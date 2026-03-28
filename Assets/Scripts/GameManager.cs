using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("이상현상 오브젝트")]
    public GameObject ghost;
    public GuardNPC guard;
    public SilhouetteAnomaly silhouette;
    public BabyLaughAnomaly babyLaugh; // ★ 새로 추가

    [Header("플레이어")]
    public GameObject player;

    // 0 = 귀신, 1 = 경비, 2 = 실루엣, -1 = 없음
    private int anomalyIndex = -1;
    private bool roundOver = false;

    // 원래 위치
    private Vector3 playerStartPos   = new Vector3(22.49f, -11f, -94.374f);
    private Vector3 guardStartPos    = new Vector3(68.7533f, -10.970f, -80.558f);
    private Quaternion guardStartRot = Quaternion.Euler(0, 186.662f, 0);
    private Vector3 ghostStartPos    = new Vector3(52.831f, -11.109f, -61.106f);
    private Quaternion ghostStartRot = Quaternion.Euler(0, 180f, 0);

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        roundOver = false;

        // ── 플레이어 위치 리셋 ──
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = playerStartPos;
            if (cc != null) cc.enabled = true;
        }

        // ── 경비 리셋 ──
        if (guard != null)
        {
            CharacterController cc = guard.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            guard.transform.position = guardStartPos;
            guard.transform.rotation = guardStartRot;
            if (cc != null) cc.enabled = true;
            guard.isAnomaly = false;
            guard.ResetGuard();
        }

        // ── 귀신 리셋 ──
        if (ghost != null)
        {
            ghost.SetActive(false);
            ghost.transform.position = ghostStartPos;
            ghost.transform.rotation = ghostStartRot;
            GhostAI ghostAI = ghost.GetComponent<GhostAI>();
            if (ghostAI != null) ghostAI.ResetGhost();
        }

        // ── 실루엣 리셋 ──
        if (silhouette != null)
            silhouette.ResetAnomaly();

        // ── 애기 웃음소리 리셋 ──
        if (babyLaugh != null)
            babyLaugh.ResetAnomaly();

        // ── 랜덤 이상현상 선택 ──
        // 0=귀신, 1=경비, 2=실루엣, 3=애기웃음, 4=없음
        int roll = 3;

        if (roll == 0 && ghost != null)
        {
            anomalyIndex = 0;
            ghost.SetActive(true);
            Debug.Log("[GameManager] 이상현상: 귀신");
        }
        else if (roll == 1 && guard != null)
        {
            anomalyIndex = 1;
            guard.isAnomaly = true;
            Debug.Log("[GameManager] 이상현상: 경비");
        }
        else if (roll == 2 && silhouette != null)
        {
            anomalyIndex = 2;
            silhouette.Activate();
            Debug.Log("[GameManager] 이상현상: 실루엣");
        }
        else if (roll == 3 && babyLaugh != null)
        {
            anomalyIndex = 3;
            babyLaugh.Activate();
            Debug.Log("[GameManager] 이상현상: 애기 웃음소리");
        }
        else
        {
            anomalyIndex = -1;
            Debug.Log("[GameManager] 이상현상 없음");
        }
    }

    /// <summary>ExitTrigger에서 플레이어가 Y/N 선택 시 호출</summary>
    public void OnPlayerExit(bool playerReportedAnomaly)
    {
        if (roundOver) return;
        roundOver = true;

        bool hasAnomaly = (anomalyIndex != -1);

        if (playerReportedAnomaly == hasAnomaly)
            Debug.Log("[GameManager] 정답! 통과!");
        else
            Debug.Log("[GameManager] 틀림! 처음으로!");

        StartRound();
    }

    /// <summary>SilhouetteAnomaly 점프스케어가 끝났을 때 자동 호출됨</summary>
    public void OnJumpScareEnd()
    {
        if (roundOver) return;
        roundOver = true;

        Debug.Log("[GameManager] 실루엣 점프스케어 → 라운드 재시작");
        StartRound();
    }
}