using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("이상현상 오브젝트")]
    public GameObject ghost;
    public GuardNPC guard;
    public SilhouetteAnomaly silhouette; // ★ 새로 추가

    [Header("플레이어")]
    public GameObject player;

    // 0 = 귀신, 1 = 경비, 2 = 실루엣, -1 = 없음
    private int anomalyIndex = -1;
    private bool roundOver = false;

    // 원래 위치
    private Vector3 playerStartPos   = new Vector3(13.37f, 0.937f, 2.084f);
    private Vector3 guardStartPos    = new Vector3(1.25f,  1.01f,  1.48f);
    private Quaternion guardStartRot = Quaternion.Euler(0, 82.149f, 0);
    private Vector3 ghostStartPos    = new Vector3(-2.17f, 0.76f, -9.26f);

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
            GhostAI ghostAI = ghost.GetComponent<GhostAI>();
            if (ghostAI != null) ghostAI.ResetGhost();
        }

        // ── 실루엣 리셋 ──
        if (silhouette != null)
            silhouette.ResetAnomaly();

        // ── 랜덤 이상현상 선택 ──
        // anomalyIndex: 0=귀신, 1=경비, 2=실루엣, -1=없음
        // 없음 확률을 높이고 싶으면 Range 범위 늘리면 됨 (예: 0~4 → -1 확률 40%)
        int roll = 2; // 0,1,2 = 이상현상 / 3 = 없음

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