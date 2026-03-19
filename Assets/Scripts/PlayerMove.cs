using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float smoothTime = 0.1f;

    [Header("카메라 설정")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    public Transform cameraTransform;

    [Header("발소리 설정")]
    public AudioClip footstepClip;

    [Header("Head Bob 설정")]
    public float walkBobAmount = 0.04f;   // 걸을 때 bob 크기
    public float runBobAmount = 0.07f;    // 뛸 때 bob 크기
    public float bobReturnSpeed = 8f;     // 원래 위치로 돌아오는 속도

    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 currentVelocity;
    private Vector3 moveVelocitySmooth;
    private float verticalVelocity = 0f;
    private float xRotation = 0f;

    // Head Bob
    private Vector3 cameraOriginPos;
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

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        cameraOriginPos = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 targetMove = (transform.right * moveX + transform.forward * moveZ).normalized * targetSpeed;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetMove, ref moveVelocitySmooth, smoothTime);

        if (characterController.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity -= 9.81f * Time.deltaTime;

        currentVelocity.y = verticalVelocity;
        characterController.Move(currentVelocity * Time.deltaTime);

        float horizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        if (animator != null)
            animator.SetFloat("Speed", horizontalSpeed);
    }

    void HandleHeadBob()
    {
        float currentY = cameraTransform.localPosition.y;
        float currentX = cameraTransform.localPosition.x;

        float targetY = cameraOriginPos.y + bobTargetY;
        float targetX = cameraOriginPos.x + bobTargetX;

        float newY = Mathf.Lerp(currentY, targetY, Time.deltaTime * bobReturnSpeed);
        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * bobReturnSpeed);

        cameraTransform.localPosition = new Vector3(newX, newY, cameraOriginPos.z);

        // 내려갔다가 다시 올라오는 처리
        if (bobGoingDown && Mathf.Abs(currentY - targetY) < 0.005f)
        {
            bobGoingDown = false;
            bobTargetY = 0f;
            bobTargetX = 0f;
        }

        // 움직임 멈추면 원래 위치로
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

        // Y축 - 발 디딜 때 살짝 내려감
        bobTargetY = -amount;
        bobGoingDown = true;

        // X축 - 발 디딜 때마다 좌우 번갈아가며 흔들림
        bobTargetX = bobTargetX > 0 ? -amount * 0.5f : amount * 0.5f;
    }
}