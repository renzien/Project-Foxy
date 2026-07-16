using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    [TextArea]
    public string dialogueText;
    public AudioClip npcVoice;

    public bool isQuestion;
    public DialogueData[] yesPath;
    public DialogueData[] noPath;
}