using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [SerializeField] private bool allowMusic = true;
    [SerializeField] private bool allowSFX = true;

    [Header("Sounds Generics")]
    [SerializeField] public AudioClip defaultClickSound;

    private Action onMusicEndCallback;
    private bool waitingForMusicToEnd;

    private void Update()
    {
        if (waitingForMusicToEnd && !musicSource.isPlaying && musicSource.clip != null)
        {
            waitingForMusicToEnd = false;
            onMusicEndCallback?.Invoke();
        }
    }

    // --- MUSIC ---

    public void PlayMusic(AudioClip clip, bool loop = true, Action onEnd = null)
    {
        if (!allowMusic || clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();

        if (!loop && onEnd != null)
        {
            onMusicEndCallback = onEnd;
            waitingForMusicToEnd = true;
        }
        else
        {
            onMusicEndCallback = null;
            waitingForMusicToEnd = false;
        }
    }



    public void StopMusic()
    {
        musicSource.Stop();
    }

    // --- SFX ---

    public void PlaySFX(AudioClip clip)
    {
        if (!allowSFX || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float delay)
    {
        if (!allowSFX || clip == null) return;

        StartCoroutine(DelayedSFX(clip, delay));
    }

    private System.Collections.IEnumerator DelayedSFX(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxSource.PlayOneShot(clip);
    }

    // --- Volume Control ---

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
}
