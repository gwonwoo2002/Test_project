using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 9f;
    public float mouseSensitivity = 3f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        // 2. 이동
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * vertical + transform.right * horizontal;

        // 3. 달리기 + Stamina
        bool wantSprint = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        bool isSprinting = wantSprint && (PlayerStats.Instance == null || PlayerStats.Instance.CanSprint());

        PlayerStats.Instance?.UpdateStamina(isSprinting);

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. 중력
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. 애니메이터
        if (animator != null)
        {
            float moveZ = Vector3.Dot(move.normalized, transform.forward);
            animator.SetFloat("MoveZ", moveZ);
        }
    }
}
