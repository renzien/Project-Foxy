using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Detection")]
    public LayerMask enemyLayers;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
}
