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
    public float footstepInterval = 0.5f;

    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 currentVelocity;
    private Vector3 moveVelocitySmooth;
    private float verticalVelocity = 0f;  // 중력 전용
    private float xRotation = 0f;
    private float footstepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
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

        // 수평 이동
        Vector3 targetMove = (transform.right * moveX + transform.forward * moveZ).normalized * targetSpeed;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetMove, ref moveVelocitySmooth, smoothTime);

        // 중력 분리 처리
        if (characterController.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity -= 9.81f * Time.deltaTime;

        currentVelocity.y = verticalVelocity;
        characterController.Move(currentVelocity * Time.deltaTime);

        // 애니메이터 Speed
        float horizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
        if (animator != null)
            animator.SetFloat("Speed", horizontalSpeed);

        // 발소리
        if (horizontalSpeed > 0.1f && characterController.isGrounded)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = isSprinting ? footstepInterval * 0.6f : footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClip != null && audioSource != null)
            audioSource.PlayOneShot(footstepClip, 1.0f);
    }
}