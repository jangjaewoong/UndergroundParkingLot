using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuardNPC : MonoBehaviour
{
    [Header("대사 설정")]
    public AudioClip normalClip;
    public AudioClip anomalyClip;
    public float dialogueDuration = 3.0f;

    [Header("감지 설정")]
    public float detectionRange = 3.0f;
    public float attackRange = 1.5f;
    public Transform player;

    [Header("UI 설정")]
    public Text interactText;
    public TextMeshProUGUI dialogueText;

    [Header("이상현상 설정")]
    public bool isAnomaly = false;
    public float chaseSpeed = 4.0f;
    public GameObject baton;

    [Header("갑툭튀 설정")]
    public Camera jumpScareCamera;
    public Camera playerCamera;
    public AudioClip jumpScareClip;
    public float jumpScareDuration = 2.0f;

    private AudioSource audioSource;
    private Animator animator;
    private bool isTalking = false;
    private bool isChasing = false;
    private bool isAttacking = false;
    private float talkTimer = 0f;
    private float jumpScareTimer = 0f;
    private CharacterController controller;
    private float verticalVelocity = 0f;

    // State: 0=idle, 1=running, 2=attack
    private void SetState(int state)
    {
        if (animator != null)
            animator.SetInteger("State", state);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
        if (baton != null)
            baton.SetActive(false);
        if (jumpScareCamera != null)
            jumpScareCamera.gameObject.SetActive(false);

        SetState(0);
    }

    void Update()
    {
        if (isAttacking)
        {
            jumpScareTimer -= Time.deltaTime;
            if (jumpScareTimer <= 0f)
            {
                if (jumpScareCamera != null)
                    jumpScareCamera.gameObject.SetActive(false);
                if (playerCamera != null)
                    playerCamera.gameObject.SetActive(true);
                isAttacking = false;
                SetState(0);
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            if (distance <= detectionRange)
            {
                if (interactText != null)
                    interactText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) && !isTalking)
                    StartTalking();
            }
            else
            {
                if (interactText != null)
                    interactText.gameObject.SetActive(false);
            }

            if (isTalking)
            {
                talkTimer -= Time.deltaTime;
                if (talkTimer <= 0f)
                {
                    if (isAnomaly)
                        StartChasing();
                    StopTalking();
                }
            }

            if (controller.isGrounded)
                verticalVelocity = -2f;
            else
                verticalVelocity -= 9.81f * Time.deltaTime;
            controller.Move(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
        }
        else
        {
            if (distance <= attackRange)
            {
                StartJumpScare();
                return;
            }

            if (controller.isGrounded)
                verticalVelocity = -2f;
            else
                verticalVelocity -= 9.81f * Time.deltaTime;

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
            dir.Normalize();
            Vector3 move = dir * chaseSpeed * Time.deltaTime;
            move.y = verticalVelocity * Time.deltaTime;
            controller.Move(move);
        }
    }

    void StartTalking()
    {
        isTalking = true;
        talkTimer = dialogueDuration;

        if (isAnomaly)
        {
            if (anomalyClip != null)
                audioSource.PlayOneShot(anomalyClip);
            if (dialogueText != null)
            {
                dialogueText.text = "살고싶으면... 지금 당장... 도망가...";
                dialogueText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (normalClip != null)
                audioSource.PlayOneShot(normalClip);
            if (dialogueText != null)
            {
                dialogueText.text = "아무 이상 없습니다.";
                dialogueText.gameObject.SetActive(true);
            }
        }
        if (animator != null)
            animator.SetBool("IsTalking", true);
    }

    void StopTalking()
    {
        isTalking = false;
        if (animator != null)
            animator.SetBool("IsTalking", false);
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
    }

    void StartChasing()
    {
        isChasing = true;
        SetState(1);
        if (baton != null)
            baton.SetActive(true);
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void StartJumpScare()
    {
        isAttacking = true;
        isChasing = false;
        SetState(2);
        jumpScareTimer = jumpScareDuration;

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);
        if (jumpScareCamera != null)
            jumpScareCamera.gameObject.SetActive(true);

        if (jumpScareClip != null && audioSource != null)
            audioSource.PlayOneShot(jumpScareClip, 1.0f);
    }

    public void ResetGuard()
    {
        isChasing = false;
        isTalking = false;
        isAttacking = false;
        talkTimer = 0f;
        isAnomaly = false;
        jumpScareTimer = 0f;

        if (animator != null)
        {
            animator.SetBool("IsTalking", false);
            SetState(0);
        }
        if (baton != null)
            baton.SetActive(false);
        if (interactText != null)
            interactText.gameObject.SetActive(false);
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
        if (jumpScareCamera != null)
            jumpScareCamera.gameObject.SetActive(false);
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}