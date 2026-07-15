using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    
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

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void ShowDialogue(string npcName, string text, AudioClip voiceClip)
    {
        isTalking = true;

        if (popSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popSound, popVolume);
        }

        bool isOpening = !dialogueBox.activeSelf;
        
        dialogueBox.SetActive(true);
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

    public void HideDialogue()
    {
        isTalking = false;

        if (popSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popSound, popVolume);
        }

        dialogueBox.SetActive(false);
        if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
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