using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Roll Dash Settings")]
    public float dashSpeed = 8f;
    public float dashDuration = 0.25f;
    public MonoBehaviour playerMovementScript;

    [Header("Player Recoil (Knockback) Settings")]
    public float recoilForceX = 5f;
    public float recoilForceY = 3.5f;
    public float recoilDuration = 0.2f;

    [Header("Detection")]
    public LayerMask enemyLayers;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isRolling = false;
    private float lastMoveDir = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(inputX) > 0.01f)
        {
            lastMoveDir = Mathf.Sign(inputX);
        }

        if (Time.time >= nextAttackTime && !isRolling)
        {
            if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
            {
                StartCoroutine(RollAttackRoutine());
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private IEnumerator RollAttackRoutine()
    {
        isRolling = true;

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        float dashDir = GetFacingDirection();
        HashSet<Collider2D> hitEnemiesRecord = new HashSet<Collider2D>();
        float timer = 0f;
        bool hasHitEnemy = false;

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;

            if (rb != null)
            {
                rb.linearVelocity = new Vector2(dashDir * dashSpeed, rb.linearVelocity.y);
            }

            if (attackPoint != null)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                foreach (Collider2D enemy in hitEnemies)
                {
                    if (!hitEnemiesRecord.Contains(enemy))
                    {
                        hitEnemiesRecord.Add(enemy);
                        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(attackDamage, transform);
                            hasHitEnemy = true;
                        }
                    }
                }
            }

            // Jika mengenai musuh, hentikan dash maju dan langsung picu pantulan mundur
            if (hasHitEnemy)
            {
                break;
            }

            yield return null;
        }

        // Berikan efek knockback mundur pada Player jika serangan mengenai musuh
        if (hasHitEnemy && rb != null)
        {
            rb.linearVelocity = new Vector2(-dashDir * recoilForceX, recoilForceY);
            yield return new WaitForSeconds(recoilDuration);
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        isRolling = false;
    }

    private float GetFacingDirection()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(inputX) > 0.01f)
        {
            return Mathf.Sign(inputX);
        }

        if (transform.localScale.x < 0f) return -1f;
        if (spriteRenderer != null && spriteRenderer.flipX) return -1f;

        return lastMoveDir;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}