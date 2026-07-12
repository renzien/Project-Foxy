using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    public Transform player;
    public GameObject dialogInfo;
    public float interactionRadius = 1.5f;

    private bool isFacingLeft = true;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        dialogInfo.SetActive(false);
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= interactionRadius)
        {
            dialogInfo.SetActive(true);
            FacePlayer();
        }
        else
        {
            dialogInfo.SetActive(false);
        }
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && isFacingLeft)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && !isFacingLeft)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
