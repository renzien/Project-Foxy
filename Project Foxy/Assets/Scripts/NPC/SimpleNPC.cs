using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    public Transform player;
    public GameObject dialogInfo;
    public float interactionRadius = 1.5f;

    public string npcName = "Ujang Kasep";

    [TextArea]
    public string dialogueText = "Lorem ipsum dolor sit amet blablablablabla... Capek";

    private bool isFacingLeft = true;
    private bool isDialogueActive = false;

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isDialogueActive)
                {
                    DialogueManager.instance.ShowDialogue(npcName, dialogueText);
                    isDialogueActive = true;
                }
                else
                {
                    DialogueManager.instance.HideDialogue();
                    isDialogueActive = false;
                }
            }
        }
        else
        {
            dialogInfo.SetActive(false);
            if (isDialogueActive)
            {
                DialogueManager.instance.HideDialogue();
                isDialogueActive = false;
            }
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
