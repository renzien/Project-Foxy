using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { Patrolling, Chasing, Resetting };
    public State currentState = State.Patrolling;

    // Enemy Settings
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    // Detection Settings
    public Transform player;
    public float detectionRadius = 5f;
    public float chaseSpeed = 3.5f;
    public float minChaseTime = 10f;
    public float maxChaseTime = 15f;
    public float resetDuration = 2f;

    // Waypoint Initiation
    private int currentWaypointIndex = 0;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private float chaseTimer = 0f;
    private float resetTimer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
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
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);

        // Spritenya di balik
        FlipSprite(target.position.x - transform.position.x);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isWaiting = true;
            waitCounter = waitTime;
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

        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        FlipSprite(player.position.x - transform.position.x);

        if (chaseTimer <= 0f)
        {
            currentState = State.Resetting;
            resetTimer = resetDuration;
        }
    }

    void ResetBehavior()
    {
        resetTimer -= Time.deltaTime;
        if (resetTimer <= 0f)
        {
            currentState = State.Patrolling;
        }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0.1f && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < -0.1f && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
