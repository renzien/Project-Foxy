using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    // Initiate
    public static DialogueManager instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void ShowDialogue(string npcName, string text)
    {
        dialogueBox.SetActive(true);
        nameText.text = npcName;
        dialogueText.text = text;

        dialogueBox.transform.localScale = Vector3.one;
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
}
