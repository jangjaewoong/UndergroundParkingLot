using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("이상현상 오브젝트")]
    public GameObject ghost;
    public GuardNPC guard;

    [Header("플레이어")]
    public GameObject player;

    private int anomalyIndex = -1;

    // 원래 위치
    private Vector3 playerStartPos = new Vector3(13.37f, 0.937f, 2.084f);
    private Vector3 guardStartPos = new Vector3(1.25f, 1.01f, 1.48f);
    private Quaternion guardStartRot = Quaternion.Euler(0, 82.149f, 0);
    private Vector3 ghostStartPos = new Vector3(-2.17f, 0.76f, -9.26f);

    void Start()
    {
        StartRound();
    }

    void StartRound()
    {
        // 위치 리셋
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = playerStartPos;
            if (cc != null) cc.enabled = true;
        }

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

        if (ghost != null)
        {
            ghost.SetActive(false);
            ghost.transform.position = ghostStartPos;
            GhostAI ghostAI = ghost.GetComponent<GhostAI>();
            if (ghostAI != null) ghostAI.ResetGhost();
        }

        // 랜덤 이상현상
        anomalyIndex = Random.Range(0, 3);

        if (anomalyIndex == 0)
        {
            if (ghost != null) ghost.SetActive(true);
            Debug.Log("이상현상: 귀신");
        }
        else if (anomalyIndex == 1)
        {
            if (guard != null) guard.isAnomaly = true;
            Debug.Log("이상현상: 경비");
        }
        else
        {
            anomalyIndex = -1;
            Debug.Log("이상현상 없음");
        }
    }

    public void OnPlayerExit(bool playerReportedAnomaly)
    {
        bool hasAnomaly = anomalyIndex != -1;

        if (playerReportedAnomaly == hasAnomaly)
        {
            Debug.Log("정답! 통과!");
        }
        else
        {
            Debug.Log("틀림! 처음으로!");
        }

        StartRound();
    }
}