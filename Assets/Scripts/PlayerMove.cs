using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float smoothTime = 0.1f;

    [Header("발소리 설정")]
    public AudioClip footstepClip;

    [Header("Head Bob 설정")]
    public float walkBobAmount = 0.04f;
    public float runBobAmount = 0.07f;
    public float bobReturnSpeed = 8f;
    public Transform headTransform; // Head 오브젝트 연결

    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 currentVelocity;
    private Vector3 moveVelocitySmooth;
    private float verticalVelocity = 0f;

    // Head Bob
    private Vector3 headOriginPos;
    private float bobTargetY = 0f;
    private float bobTargetX = 0f;
    private bool bobGoingDown = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (headTransform != null)
            headOriginPos = headTransform.localPosition;
    }

    void Update()
    {
        HandleMovement();
        HandleHeadBob();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        // 카메라 방향 기준으로 이동 (Y축 무시)
        Camera cam = Camera.main;
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(cam.transform.right.x, 0f, cam.transform.right.z).normalized;

        Vector3 targetMove = (camRight * moveX + camForward * moveZ).normalized * targetSpeed;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetMove, ref moveVelocitySmooth, smoothTime);

        // 중력
        if (characterController.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity -= 9.81f * Time.deltaTime;

        currentVelocity.y = verticalVelocity;
        characterController.Move(currentVelocity * Time.deltaTime);

        // 이동 방향으로 플레이어 회전 (좌우만)
        if (new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude > 0.1f)
        {
            Vector3 lookDir = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
        }

        // 애니메이터
        float horizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        if (animator != null)
            animator.SetFloat("Speed", horizontalSpeed);
    }

    void HandleHeadBob()
    {
        if (headTransform == null) return;

        float currentY = headTransform.localPosition.y;
        float currentX = headTransform.localPosition.x;

        float targetY = headOriginPos.y + bobTargetY;
        float targetX = headOriginPos.x + bobTargetX;

        float newY = Mathf.Lerp(currentY, targetY, Time.deltaTime * bobReturnSpeed);
        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * bobReturnSpeed);

        headTransform.localPosition = new Vector3(newX, newY, headOriginPos.z);

        if (bobGoingDown && Mathf.Abs(currentY - targetY) < 0.005f)
        {
            bobGoingDown = false;
            bobTargetY = 0f;
            bobTargetX = 0f;
        }

        float horizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        if (horizontalSpeed < 0.1f)
        {
            bobTargetY = 0f;
            bobTargetX = 0f;
        }
    }

    // Animation Event 에서 호출
    public void PlayFootstep()
    {
        if (footstepClip != null && audioSource != null)
            audioSource.PlayOneShot(footstepClip, 1.0f);

        if (!characterController.isGrounded) return;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float amount = isSprinting ? runBobAmount : walkBobAmount;

        bobTargetY = -amount;
        bobGoingDown = true;
        bobTargetX = bobTargetX > 0 ? -amount * 0.5f : amount * 0.5f;
    }
}