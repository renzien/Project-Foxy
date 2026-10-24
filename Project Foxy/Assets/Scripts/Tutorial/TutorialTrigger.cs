using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup tutorialPopup;
    public Transform player;

    [Header("Follow Settings")]
    public bool followPlayer = true;
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    [Header("Fade & Timing Settings")]
    public float fadeDuration = 0.5f;
    public float displayDuration = 4f;
    public bool triggerOnlyOnce = true;
    public bool hideOnExit = false;

    private bool hasTriggered = false;
    private bool isShowing = false;
    private Coroutine activeCoroutine;
    private Canvas parentCanvas;
    private RectTransform popupRect;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (tutorialPopup != null)
        {
            popupRect = tutorialPopup.GetComponent<RectTransform>();
            parentCanvas = tutorialPopup.GetComponentInParent<Canvas>();
            tutorialPopup.alpha = 0f;
            tutorialPopup.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (isShowing && followPlayer && player != null && popupRect != null)
        {
            Vector3 targetWorldPos = player.position + offset;

            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                popupRect.position = Camera.main.WorldToScreenPoint(targetWorldPos);
            }
            else
            {
                popupRect.position = targetWorldPos;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;

            hasTriggered = true;

            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            activeCoroutine = StartCoroutine(ShowTutorialRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hideOnExit && isShowing)
        {
            if (activeCoroutine != null) StopCoroutine(activeCoroutine);
            activeCoroutine = StartCoroutine(FadePopup(0f, false));
        }
    }

    private IEnumerator ShowTutorialRoutine()
    {
        yield return StartCoroutine(FadePopup(1f, true));

        if (displayDuration > 0f && !hideOnExit)
        {
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadePopup(0f, false));
        }
    }

    private IEnumerator FadePopup(float targetAlpha, bool turnOn)
    {
        if (tutorialPopup == null) yield break;

        if (turnOn)
        {
            tutorialPopup.gameObject.SetActive(true);
            isShowing = true;
        }

        float startAlpha = tutorialPopup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            tutorialPopup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        tutorialPopup.alpha = targetAlpha;

        if (!turnOn)
        {
            isShowing = false;
            tutorialPopup.gameObject.SetActive(false);
        }
    }
}