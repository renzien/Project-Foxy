using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Effects")]
    public ParticleSystem dustParticle; // Referensi untuk partikel debu

    private bool isGrounded;
    private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Cek apakah sedang dialog
        if (DialogueManager.instance.isTalking)
        {
            moveInput = 0f;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0f);
            
            // Matikan debu saat dialog
            if (dustParticle != null && dustParticle.isPlaying) dustParticle.Stop();
            return;
        }

        moveInput = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Logika Particle Debu
        if (dustParticle != null)
        {
            // Jika menjejak tanah DAN ada pergerakan WASD
            if (isGrounded && Mathf.Abs(moveInput) > 0f)
            {
                if (!dustParticle.isPlaying) dustParticle.Play();
            }
            else
            {
                if (dustParticle.isPlaying) dustParticle.Stop();
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        FlipController();
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void FlipController()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}