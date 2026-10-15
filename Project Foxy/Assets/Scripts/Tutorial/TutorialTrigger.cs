using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject tutorialPopup;

    [Header("Serttings")]
    public bool triggerOnlyOnce = false;
    public bool hideOnExit = true;

    private bool hasTriggered = false;

    void Start()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;
            if (tutorialPopup != null)
            {
                tutorialPopup.SetActive(true);
            }

            hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hideOnExit)
        {
            if (tutorialPopup != null)
            {
                tutorialPopup.SetActive(false);
            }

            if (triggerOnlyOnce)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
