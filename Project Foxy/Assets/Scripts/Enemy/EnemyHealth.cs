using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Death Animation Settings")]
    public float deathAnimDuration = 0.5f;
    public AudioClip deathSound;

    [Header("Knockback Settings")]
    public float knockbackForceX = 6f;
    public float knockbackForceY = 3f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private EnemyAI enemyAI;
    private Coroutine knockbackCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage, Transform attacker = null)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (attacker != null && rb != null)
        {
            if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = StartCoroutine(KnockbackRoutine(attacker));
        }
    }

    private IEnumerator KnockbackRoutine(Transform attacker)
    {
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        float knockbackDir = Mathf.Sign(transform.position.x - attacker.position.x);
        if (knockbackDir == 0f) knockbackDir = 1f;

        rb.linearVelocity = new Vector2(knockbackDir * knockbackForceX, knockbackForceY);

        yield return new WaitForSeconds(knockbackDuration);

        if (!isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (enemyAI != null)
            {
                enemyAI.enabled = true;
            }
        }
    }

    void Die()
    {
        isDead = true;

        // 1. Hentikan Knockback dan matikan EnemyAI agar musuh diam di tempat
        if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
        if (enemyAI != null) enemyAI.enabled = false;

        // 2. Matikan efek fisika dan collider agar tidak jatuh atau menghalangi Foxy
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (col != null) col.enabled = false;

        // 3. Jalankan animasi ledakan di Animator musuh
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 4. Putar suara mati (opsional)
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // 5. Hapus objek musuh setelah durasi animasi selesai
        Destroy(gameObject, deathAnimDuration);
    }
}