using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    // Initiate
    public static DialogueManager instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public AnimationCurve bounceCurve;
    public float bounceDuration = 0.3f;

    private Coroutine bounceCoroutine;

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

        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }

        bounceCoroutine = StartCoroutine(AnimateBounce());
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
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
