using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Resetting }
    public State currentState = State.Patrolling;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float jumpForce = 7f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private Vector3[] waypointPositions;
    public float waitTime = 1f;
    private int currentWaypointIndex = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    [Header("Chase Settings")]
    public Transform player;
    public float detectionRadius = 5f;
    public float minChaseTime = 10f;
    public float maxChaseTime = 15f;
    public float resetDuration = 2f;
    private float chaseTimer = 0f;
    private float resetTimer = 0f;

    [Header("Environment Checks")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.6f;
    public float wallCheckDistance = 0.5f;

    private Rigidbody2D rb;
    private bool isFacingLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (waypoints != null && waypoints.Length > 0)
        {
            waypointPositions = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                waypointPositions[i] = waypoints[i].position;
            }
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                PatrolBehavior();
                CheckForPlayer();
                break;
            case State.Chasing:
                ChaseBehavior();
                break;
            case State.Resetting:
                ResetBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        if (waypointPositions == null || waypointPositions.Length == 0) return;

        if (isWaiting)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        Vector3 targetPosition = waypointPositions[currentWaypointIndex];
        float distanceX = Mathf.Abs(targetPosition.x - transform.position.x);

        if (distanceX < 0.1f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypointPositions.Length;
            isWaiting = true;
            waitCounter = waitTime;
        }
        else
        {
            MoveTowardsTarget(targetPosition.x, patrolSpeed);
        }
    }

    void CheckForPlayer()
    {
        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            currentState = State.Chasing;
            chaseTimer = Random.Range(minChaseTime, maxChaseTime);
        }
    }

    void ChaseBehavior()
    {
        if (player == null) return;

        chaseTimer -= Time.deltaTime;
        
        MoveTowardsTarget(player.position.x, chaseSpeed);

        if (chaseTimer <= 0f)
        {
            currentState = State.Resetting;
            resetTimer = resetDuration;
        }
    }

    void ResetBehavior()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        resetTimer -= Time.deltaTime;
        if (resetTimer <= 0f)
        {
            currentState = State.Patrolling;
        }
    }

    void MoveTowardsTarget(float targetX, float speed)
    {
        float dirX = Mathf.Sign(targetX - transform.position.x);
        rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);
        
        FlipSprite(dirX);
        HandleJump(dirX);
    }

    void HandleJump(float dirX)
    {
        Vector2 groundCheckPos = transform.position;
        bool isGrounded = Physics2D.Raycast(groundCheckPos, Vector2.down, groundCheckDistance, groundLayer);

        Vector2 wallCheckDir = new Vector2(dirX, 0);
        bool isWallAhead = Physics2D.Raycast(transform.position, wallCheckDir, wallCheckDistance, groundLayer);

        if (isGrounded && isWallAhead)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0.01f && isFacingLeft)
        {
            Flip();
        }
        else if (directionX < -0.01f && !isFacingLeft)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        
        Gizmos.color = Color.red;
        float dir = isFacingLeft ? -1f : 1f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * dir * wallCheckDistance);
    }
}