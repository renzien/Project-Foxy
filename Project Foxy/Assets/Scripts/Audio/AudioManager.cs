using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource ambientSource;

    public AudioClip bgmClip;
    public AudioClip[] windClips;

    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float ambientVolume = 0.2f;

    private int currentWindIndex = 0;

    void Start()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.volume = bgmVolume;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        if (ambientSource != null && windClips != null && windClips.Length > 0)
        {
            ambientSource.volume = ambientVolume;
            StartCoroutine(PlayWindAlternating());
        }
    }

    private IEnumerator PlayWindAlternating()
    {
        while (true)
        {
            if (windClips == null || windClips.Length == 0) yield break;

            if (currentWindIndex >= windClips.Length || currentWindIndex < 0)
            {
                currentWindIndex = 0;
            }

            AudioClip clipToPlay = windClips[currentWindIndex];

            if (clipToPlay != null)
            {
                ambientSource.clip = clipToPlay;
                ambientSource.Play();
                yield return new WaitForSeconds(clipToPlay.length);
            }
            else
            {
                yield return null;
            }

            currentWindIndex++;
        }
    }
}