using UnityEngine;

public class FrogPatrol : MonoBehaviour
{
    public float jumpForceX = 3f;
    public float jumpForceY = 6f;
    public float minIdleTime = 0.5f;
    public float maxIdleTime = 2.5f;

    [Header("Checks")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float timer;
    private bool isGrounded;
    private bool isFacingLeft = true; // Default sprite kodok SunnyLand hadap kiri

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetRandomIdleTime();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        bool hitWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);

        // Kalau nabrak tembok pas di tanah, balik badan
        if (hitWall && isGrounded)
        {
            Flip();
        }

        // Logika Timer dan Melompat
        if (isGrounded)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0f)
            {
                Jump();
                SetRandomIdleTime();
            }
        }

        // Kirim data ke Animator (Persis kayak Foxy)
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private void Jump()
    {
        // Tentukan arah lompatan berdasarkan hadap kodok
        float direction = isFacingLeft ? -1f : 1f;
        
        // Berikan dorongan linear velocity
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
    }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void SetRandomIdleTime()
    {
        timer = Random.Range(minIdleTime, maxIdleTime);
    }
}