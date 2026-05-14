using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    [Header("마우스 회전")]
    public float mouseSensitivity = 0.15f;
    public float verticalClamp = 80f;
    public Transform cameraTransform;

    private Rigidbody _rb;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private float _verticalRotation;

    // ⭐ 입력 잠금 플래그
    private bool _inputLocked = false;
    public bool IsInputLocked => _inputLocked;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        // InputAction 직접 생성 - Asset 없어도 동작
        _moveAction = new InputAction("Move", InputActionType.Value,
            binding: "<Gamepad>/leftStick");
        _moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        _lookAction = new InputAction("Look", InputActionType.Value,
            binding: "<Mouse>/delta");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        _moveAction.Enable();
        _lookAction.Enable();
    }

    void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
    }

    void Update()
    {
        // ⭐ 잠금 상태면 회전 동기화도 스킵 (카메라가 안 돌아가니 회전도 멈춤)
        if (_inputLocked) return;
        if (cameraTransform != null)
        {
            Vector3 camEuler = cameraTransform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);
        }
    }

    void FixedUpdate()
    {
        // ⭐ 잠금 상태면 속도 0으로 고정
        if (_inputLocked)
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }
        Vector2 move = _moveAction.ReadValue<Vector2>();
        Vector3 dir = transform.right * move.x + transform.forward * move.y;
        Vector3 vel = dir.normalized * moveSpeed;
        _rb.linearVelocity = new Vector3(vel.x, _rb.linearVelocity.y, vel.z);
    }
    // ⭐ 외부에서 호출할 잠금 제어
    public void SetInputLocked(bool locked)
    {
        _inputLocked = locked;

        if (locked)
        {
            // 잠글 때 현재 수평 속도 즉시 0으로
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            _rb.angularVelocity = Vector3.zero;
        }
    }
}