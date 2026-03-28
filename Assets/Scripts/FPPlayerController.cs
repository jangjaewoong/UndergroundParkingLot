using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Camera")]
    public Transform cameraTarget;      // 머리 위치 빈 오브젝트
    public Transform cinemachineCam;    // Hierarchy에서 CinemachineCamera 드래그

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleAnimator();
    }

    void HandleMouseLook()
    {
        if (cinemachineCam == null) return;

        // Cinemachine 카메라의 Y회전을 캐릭터 몸체에 동기화
        float camY = cinemachineCam.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, camY, 0f);
    }

    void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 이동 입력
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 카메라 방향 기준으로 이동
        Vector3 forward = cinemachineCam.forward;
        Vector3 right = cinemachineCam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = right * input.x + forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);

        // 달리기
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAnimator()
    {
        if (animator == null) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        float moveAmount = input.magnitude;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        animator.SetFloat("Speed", moveAmount * (isRunning ? 2f : 1f), 0.1f, Time.deltaTime);
    }
}