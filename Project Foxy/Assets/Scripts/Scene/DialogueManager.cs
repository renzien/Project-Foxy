using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    
    public GameObject choiceBox;
    public Button yesButton;
    public Button noButton;

    public AudioSource audioSource;

    [Header("Audio UI")]
    public AudioClip popSound;
    [Range(0f, 1f)] public float popVolume = 0.4f;

    [Header("Animasi Bouncy")]
    public AnimationCurve bounceCurve;
    public float bounceDuration = 0.3f;
    private Coroutine bounceCoroutine;

    [Header("Efek Typewriter")]
    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    public bool isTalking = false;
    private SimpleNPC currentNPC;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        dialogueBox.SetActive(false);
        if (choiceBox != null) choiceBox.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
    }

    public void ShowDialogue(string npcName, string text, AudioClip voiceClip, SimpleNPC npc = null)
    {
        isTalking = true;
        currentNPC = npc;

        if (popSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popSound, popVolume);
        }

        bool isOpening = !dialogueBox.activeSelf;
        
        dialogueBox.SetActive(true);
        if (choiceBox != null) choiceBox.SetActive(false);
        nameText.text = npcName;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));

        if (voiceClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(voiceClip);
        }

        if (isOpening)
        {
            if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
            bounceCoroutine = StartCoroutine(AnimateBounce());
        }
        else
        {
            dialogueBox.transform.localScale = Vector3.one;
        }
    }

    public void ShowChoiceBox()
    {
        if (choiceBox != null) choiceBox.SetActive(true);
    }

    public void HideDialogue()
    {
        isTalking = false;
        currentNPC = null;

        if (popSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popSound, popVolume);
        }

        dialogueBox.SetActive(false);
        if (choiceBox != null) choiceBox.SetActive(false);
        
        if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
    }

    private void OnYesClicked()
    {
        if (popSound != null && audioSource != null) audioSource.PlayOneShot(popSound, popVolume);
        if (choiceBox != null) choiceBox.SetActive(false);
        if (currentNPC != null) currentNPC.ReceiveChoice(true);
    }

    private void OnNoClicked()
    {
        if (popSound != null && audioSource != null) audioSource.PlayOneShot(popSound, popVolume);
        if (choiceBox != null) choiceBox.SetActive(false);
        if (currentNPC != null) currentNPC.ReceiveChoice(false);
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        foreach (char c in text.ToCharArray())
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        if (currentNPC != null && currentNPC.IsWaitingForChoice())
        {
            ShowChoiceBox();
        }
    }

    private IEnumerator AnimateBounce()
    {
        float timer = 0f;
        dialogueBox.transform.localScale = Vector3.zero;

        while (timer < bounceDuration)
        {
            timer += Time.unscaledDeltaTime;
            float scale = bounceCurve.Evaluate(timer / bounceDuration);
            dialogueBox.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        
        dialogueBox.transform.localScale = Vector3.one;
    }
}