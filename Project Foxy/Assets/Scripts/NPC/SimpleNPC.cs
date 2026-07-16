using UnityEngine;

public class SimpleNPC : MonoBehaviour
{
    public Transform player;
    public GameObject dialogInfo;
    public float interactionRadius = 1.5f;

    public DialogueData[] defaultDialogues;
    
    private DialogueData[] currentDialogues;
    private int currentDialogueIndex = 0;

    private bool isFacingLeft = true;
    private bool isDialogueActive = false;
    private bool isWaitingForChoice = false;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        dialogInfo.SetActive(false);
        currentDialogues = defaultDialogues;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= interactionRadius)
        {
            dialogInfo.SetActive(!isDialogueActive);
            FacePlayer();
            
            if (Input.GetKeyDown(KeyCode.E) && !isWaitingForChoice)
            {
                if (currentDialogues == null || currentDialogues.Length == 0) return;

                if (!isDialogueActive)
                {
                    PlayCurrentDialogue();
                    isDialogueActive = true;
                }
                else
                {
                    currentDialogueIndex++;

                    if (currentDialogueIndex < currentDialogues.Length)
                    {
                        PlayCurrentDialogue();
                    }
                    else
                    {
                        EndDialogue();
                    }
                }
            }
        }
        else
        {
            dialogInfo.SetActive(false);
            if (isDialogueActive)
            {
                EndDialogue();
            }
        }
    }

    private void PlayCurrentDialogue()
    {
        DialogueData data = currentDialogues[currentDialogueIndex];
        isWaitingForChoice = data.isQuestion;
        
        DialogueManager.instance.ShowDialogue(data.npcName, data.dialogueText, data.npcVoice, this);
    }

    public bool IsWaitingForChoice()
    {
        return isWaitingForChoice;
    }

    public void ReceiveChoice(bool isYes)
    {
        isWaitingForChoice = false;
        DialogueData data = currentDialogues[currentDialogueIndex];

        if (isYes && data.yesPath != null && data.yesPath.Length > 0)
        {
            currentDialogues = data.yesPath;
        }
        else if (!isYes && data.noPath != null && data.noPath.Length > 0)
        {
            currentDialogues = data.noPath;
        }
        else
        {
            EndDialogue();
            return;
        }

        currentDialogueIndex = 0;
        PlayCurrentDialogue();
    }

    private void EndDialogue()
    {
        DialogueManager.instance.HideDialogue();
        isDialogueActive = false;
        isWaitingForChoice = false;
        currentDialogueIndex = 0;
        currentDialogues = defaultDialogues;
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