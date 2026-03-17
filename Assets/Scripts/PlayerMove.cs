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
    public AudioClip runFootstepClip;

    private CharacterController characterController;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 currentVelocity;
    private Vector3 moveVelocitySmooth;
    private float verticalVelocity = 0f;
    private float xRotation = 0f;
    private float footstepCooldown = 0f;

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

        if (footstepCooldown > 0f)
            footstepCooldown -= Time.deltaTime;
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

    void PlayFootstep()
    {
        if (audioSource == null) return;
        if (footstepCooldown > 0f) return;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        AudioClip clip = (isSprinting && runFootstepClip != null) ? runFootstepClip : footstepClip;

        if (clip != null)
        {
            audioSource.PlayOneShot(clip, 1.0f);
            footstepCooldown = 0.3f;
        }
    }
}