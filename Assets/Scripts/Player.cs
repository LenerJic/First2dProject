using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public float speed = 2f;
    private Vector2 moveDirection;
    public InputActionReference move;
    public InputActionReference jump;
    public float jumpForce = 4f;
    private bool isGrounded;
    public Transform grounCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
    }

    void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
    }

    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        if (moveDirection.x != 0) transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);

        isGrounded = Physics2D.OverlapCircle(grounCheck.position, groundRadius, groundLayer);

        if(jump.action.WasPressedThisFrame() && isGrounded) Jump();
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(moveDirection.x * speed, rb2D.linearVelocity.y);
    }

    private void Jump()
    {
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
    }
}
