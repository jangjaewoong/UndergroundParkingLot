using UnityEngine;
using UnityEngine.UI;

public class GuardNPC : MonoBehaviour
{
    [Header("대사 설정")]
    public AudioClip dialogueClip;
    public float dialogueDuration = 3.0f;

    [Header("감지 설정")]
    public float detectionRange = 3.0f;
    public Transform player;

    [Header("UI 설정")]
    public Text interactText;

    private AudioSource audioSource;
    private Animator animator;
    private bool isTalking = false;
    private float talkTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // 감지 범위 안에 있으면 UI 표시
        if (distance <= detectionRange)
        {
            if (interactText != null)
                interactText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartTalking();
            }
        }
        else
        {
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }

        // 대사 재생 타이머
        if (isTalking)
        {
            talkTimer -= Time.deltaTime;
            if (talkTimer <= 0f)
            {
                StopTalking();
            }
        }
    }

    void StartTalking()
    {
        isTalking = true;
        talkTimer = dialogueDuration;
        audioSource.PlayOneShot(dialogueClip);
        animator.SetBool("IsTalking", true);
    }

    void StopTalking()
    {
        isTalking = false;
        animator.SetBool("IsTalking", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}