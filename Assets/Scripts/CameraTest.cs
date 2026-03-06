using UnityEngine;
using UnityEngine.InputSystem;
public class CameraTest : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private GameInputSystem gameInputSystem;
    private Vector2 moveInput;

    void Awake()
    {
        gameInputSystem = new GameInputSystem();
    }

    void OnEnable()
    {
        gameInputSystem.Camera.Enable();
        gameInputSystem.Camera.Move.performed += OnMove;
        gameInputSystem.Camera.Move.canceled += OnMove;
    }

    void OnDisable()
    {
        gameInputSystem.Camera.Move.performed -= OnMove;
        gameInputSystem.Camera.Move.canceled -= OnMove;
        gameInputSystem.Camera.Disable();
    }
    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void Update()
    {
        transform.position += new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.deltaTime;
    }
      
}
