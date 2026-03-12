using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;

    [Header("마우스 설정")]
    public float mouseSensitivity = 2.0f;
    public Transform cameraTransform;
    public float cameraHeight = 1.7f;

    [Header("발소리 설정")]
    public AudioClip footstepClip;
    public float footstepInterval = 0.5f;

    private CharacterController controller;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float xRotation = 0f;
    private float footstepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseLook();
        Move();
        ApplyGravity();
        UpdateCamera();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = transform.right * h + transform.forward * v;
        moveDir.Normalize();

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (moveDir.magnitude >= 0.1f)
        {
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
            animator.SetFloat("Speed", currentSpeed);

            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = Input.GetKey(KeyCode.LeftShift)
                    ? footstepInterval * 0.6f
                    : footstepInterval;
            }
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            footstepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClip != null && audioSource != null)
            audioSource.PlayOneShot(footstepClip, 1.0f);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateCamera()
    {
        cameraTransform.position = transform.position + Vector3.up * cameraHeight;
    }
}