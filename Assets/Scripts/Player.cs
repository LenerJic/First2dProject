using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
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

    private Animator animator;

    private int coins;
    public TMP_Text textCoins;

    public AudioSource audioSource;
    public AudioClip coinClip;
    public AudioClip barrelClip;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        animator.SetFloat(SpeedHash, Mathf.Abs(moveDirection.x));
        animator.SetFloat(VerticalVelocityHash, rb2D.linearVelocity.y);
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinClip);
            Destroy(collision.gameObject);
            coins++;
            textCoins.text = coins.ToString();
        }

        if (collision.transform.CompareTag("Spikes"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (collision.transform.CompareTag("Barrel"))
        {
            audioSource.PlayOneShot(barrelClip);
            Vector2 knockbackDir = (rb2D.position - (Vector2)collision.transform.position).normalized;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();

            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);
        }
    }

    private void Jump()
    {
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
    }
}
