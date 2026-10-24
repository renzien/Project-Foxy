using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepName;
        public DialogueData[] dialoguesBeforeStep;
        public CanvasGroup popupUI;
        public KeyCode[] requiredKeys;
        public bool requireAllKeys = true; 
        public float delayAfterInput = 0.5f; 
    }

    [Header("Player References")]
    public Transform player;
    public MonoBehaviour playerMovementScript;

    [Header("Tutorial Steps")]
    public TutorialStep[] tutorialSteps;

    [Header("Follow & Fade Settings")]
    public bool followPlayer = true;
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public float fadeDuration = 0.5f;
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private bool isShowingPopup = false;
    private RectTransform currentPopupRect;
    private Canvas parentCanvas;
    private Rigidbody2D playerRb;
    private Animator playerAnim;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerAnim = player.GetComponent<Animator>();
        }

        foreach (TutorialStep step in tutorialSteps)
        {
            if (step.popupUI != null)
            {
                step.popupUI.alpha = 0f;
                step.popupUI.gameObject.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        if (isShowingPopup && followPlayer && player != null && currentPopupRect != null)
        {
            Vector3 targetWorldPos = player.position + offset;

            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                currentPopupRect.position = Camera.main.WorldToScreenPoint(targetWorldPos);
            }
            else
            {
                currentPopupRect.position = targetWorldPos;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;

            hasTriggered = true;
            StartCoroutine(RunTutorialSequence());
        }
    }

    private IEnumerator RunTutorialSequence()
    {
        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            TutorialStep currentStep = tutorialSteps[i];

            // 1. Jalankan Dialog terlebih dahulu jika ada
            if (currentStep.dialoguesBeforeStep != null && currentStep.dialoguesBeforeStep.Length > 0)
            {
                yield return StartCoroutine(PlayStepDialogues(currentStep.dialoguesBeforeStep));
            }

            // 2. Munculkan Popup UI Tutorial (Fade In)
            if (currentStep.popupUI != null)
            {
                currentPopupRect = currentStep.popupUI.GetComponent<RectTransform>();
                parentCanvas = currentStep.popupUI.GetComponentInParent<Canvas>();

                yield return StartCoroutine(FadePopup(currentStep.popupUI, 1f, true));

                // 3. Tunggu sampai pemain menekan tombol yang diminta
                yield return StartCoroutine(WaitForPlayerInput(currentStep));

                // Jeda sedikit setelah tombol ditekan agar transisi tidak terlalu mendadak
                if (currentStep.delayAfterInput > 0f)
                {
                    yield return new WaitForSeconds(currentStep.delayAfterInput);
                }

                // 4. Sembunyikan Popup UI Tutorial (Fade Out)
                yield return StartCoroutine(FadePopup(currentStep.popupUI, 0f, false));
            }
        }
    }

    private IEnumerator PlayStepDialogues(DialogueData[] dialogues)
    {
        FreezePlayer(true);

        int index = 0;
        DialogueManager.instance.ShowDialogue(dialogues[index].npcName, dialogues[index].dialogueText, dialogues[index].npcVoice, null);

        while (index < dialogues.Length)
        {
            yield return null; 

            if (Input.GetKeyDown(KeyCode.E))
            {
                index++;
                if (index < dialogues.Length)
                {
                    DialogueManager.instance.ShowDialogue(dialogues[index].npcName, dialogues[index].dialogueText, dialogues[index].npcVoice, null);
                }
            }
        }

        DialogueManager.instance.HideDialogue();
        FreezePlayer(false);
    }

    private IEnumerator WaitForPlayerInput(TutorialStep step)
    {
        if (step.requiredKeys == null || step.requiredKeys.Length == 0)
        {
            yield return new WaitForSeconds(3f);
            yield break;
        }

        HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

        while (true)
        {
            foreach (KeyCode key in step.requiredKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    pressedKeys.Add(key);
                }
            }

            if (step.requireAllKeys)
            {
                if (pressedKeys.Count >= step.requiredKeys.Length)
                {
                    break; 
                }
            }
            else
            {
                if (pressedKeys.Count > 0)
                {
                    break; 
                }
            }

            yield return null;
        }
    }

    private IEnumerator FadePopup(CanvasGroup cg, float targetAlpha, bool turnOn)
    {
        if (turnOn)
        {
            cg.gameObject.SetActive(true);
            isShowingPopup = true;
        }

        float startAlpha = cg.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;

        if (!turnOn)
        {
            isShowingPopup = false;
            cg.gameObject.SetActive(false);
        }
    }

    private void FreezePlayer(bool freeze)
    {
        if (playerRb != null && freeze)
        {
            playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = !freeze;
        }

        if (playerAnim != null && freeze)
        {
            playerAnim.Rebind();
            playerAnim.Update(0f);
        }
    }
}