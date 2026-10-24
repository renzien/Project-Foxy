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
        // Rekam arah gerak terakhir agar saat diam Foxy tetap roll ke arah hadapnya
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

        // Matikan sementara skrip jalan biasa agar kecepatan dash tidak tertimpa
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        float dashDir = GetFacingDirection();
        HashSet<Collider2D> hitEnemiesRecord = new HashSet<Collider2D>();
        float timer = 0f;

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;

            // Berikan dorongan maju selama menggelinding
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(dashDir * dashSpeed, rb.linearVelocity.y);
            }

            // Deteksi musuh sepanjang gerakan roll (1 musuh maksimal kena 1x per roll)
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
                            enemyHealth.TakeDamage(attackDamage);
                        }
                    }
                }
            }

            yield return null;
        }

        // Hentikan sisa luncuran dan aktifkan kembali skrip jalan Foxy
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