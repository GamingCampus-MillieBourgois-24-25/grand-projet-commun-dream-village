using UnityEngine;
using UnityEngine.Audio;

public class AudioOptions : MonoBehaviour
{
    [Header("Audio Settings")] 
    [SerializeField] private float gameVolume = 1;
    [SerializeField] private float musicVolume = 1;
    [SerializeField] private float uiVolume = 1;
    public AudioMixer audioMixer;
    
    // Start is called before the first frame update
    void Start()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("GameVolume", Mathf.Log10(gameVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("UISFX", Mathf.Log10(uiVolume) * 20);
        }
    }
    
    public void SetGameVolume(float volume)
    {
        gameVolume = Mathf.Clamp(volume, 0.0001f, 1);
        if (audioMixer != null)
        {
            audioMixer.SetFloat("GameVolume", Mathf.Log10(gameVolume) * 20);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0.0001f, 1);
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        }
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp(volume, 0.0001f, 1);
        if (audioMixer != null)
        {
            audioMixer.SetFloat("UISFX", Mathf.Log10(uiVolume) * 20);
        }
    }
}
