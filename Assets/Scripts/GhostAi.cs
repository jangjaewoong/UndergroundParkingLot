using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public float detectionRange = 8.0f;
    public Transform player;
    public float crawlSpeed = 3.0f;
    public float lookDuration = 2.0f;
    public float attackRange = 1.5f;

    [Header("사운드 설정")]
    public AudioClip chaseClip;    // 쫓아올때 소리
    public AudioClip jumpScareClip; // 갑툭튀 소리

    [Header("갑툭튀 설정")]
    public Camera jumpScareCamera;
    public Camera playerCamera;
    public float jumpScareDuration = 2.0f;

    private Animator animator;
    private CharacterController controller;
    private AudioSource audioSource;

    private enum GhostState { Idle, Looking, Crawling, Attack }
    private GhostState currentState = GhostState.Idle;

    private float lookTimer = 0f;
    private bool playerDetected = false;
    private float verticalVelocity = 0f;
    private float jumpScareTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (jumpScareCamera != null)
            jumpScareCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (controller.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity -= 9.81f * Time.deltaTime;

        Vector3 gravity = new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);

        if (currentState == GhostState.Idle)
        {
            controller.Move(gravity);
            if (distance <= detectionRange && !playerDetected)
            {
                playerDetected = true;
                SetState(GhostState.Looking);
            }
        }
        else if (currentState == GhostState.Looking)
        {
            controller.Move(gravity);
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 3.0f * Time.deltaTime);

            float progress = 1f - (lookTimer / lookDuration);
            audioSource.volume = Mathf.Lerp(0.1f, 0.7f, progress);

            lookTimer -= Time.deltaTime;
            if (lookTimer <= 0f)
                SetState(GhostState.Crawling);
        }
        else if (currentState == GhostState.Crawling)
        {
            if (distance <= attackRange)
            {
                SetState(GhostState.Attack);
                return;
            }

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            dir.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 3.0f * Time.deltaTime);
            Vector3 move = dir * crawlSpeed * Time.deltaTime;
            move.y = verticalVelocity * Time.deltaTime;
            controller.Move(move);
        }
        else if (currentState == GhostState.Attack)
        {
            controller.Move(gravity);
            jumpScareTimer -= Time.deltaTime;
            if (jumpScareTimer <= 0f)
            {
                jumpScareCamera.gameObject.SetActive(false);
                playerCamera.gameObject.SetActive(true);
            }
        }
    }

    void SetState(GhostState newState)
    {
        currentState = newState;
        if (newState == GhostState.Idle)
            animator.SetInteger("State", 0);
        else if (newState == GhostState.Looking)
        {
            animator.SetInteger("State", 1);
            lookTimer = lookDuration;
            audioSource.volume = 0.1f;
            audioSource.loop = true;
            audioSource.clip = chaseClip;
            audioSource.Play();
        }
        else if (newState == GhostState.Crawling)
        {
            animator.SetInteger("State", 2);
            audioSource.volume = 1.0f;
        }
        else if (newState == GhostState.Attack)
        {
            animator.SetInteger("State", 3);
            audioSource.Stop();

            playerCamera.gameObject.SetActive(false);
            jumpScareCamera.gameObject.SetActive(true);
            jumpScareTimer = jumpScareDuration;

            if (jumpScareClip != null)
                audioSource.PlayOneShot(jumpScareClip, 1.0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}