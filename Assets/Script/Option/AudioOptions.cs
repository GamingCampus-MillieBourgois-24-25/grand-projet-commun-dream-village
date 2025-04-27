using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [Header("Audio Settings")] 
    [SerializeField] private float gameVolume = 1;
    [SerializeField] private bool gameVolumeEnabled = true;
    
    [SerializeField] private float musicVolume = 1;
    [SerializeField] private bool musicVolumeEnabled = true;
    
    [SerializeField] private Image gameVolumeButton;
    [SerializeField] private Image musicVolumeButton;
    [SerializeField] private Color disabledColor;
        
    public AudioMixer audioMixer;
    
    // Start is called before the first frame update
    void Start()
    {
        if (audioMixer != null)
        {
            SetGameVolume();
            SetMusicVolume();
        }
    }
    
    // public void SetGameVolume(float volume)
    // {
    //     gameVolume = Mathf.Clamp(volume, 0.0001f, 1);
    //     if (audioMixer != null)
    //     {
    //         audioMixer.SetFloat("GameVolume", Mathf.Log10(gameVolume) * 20);
    //     }
    // }
    
    // public void SetMusicVolume(float volume)
    // {
    //     musicVolume = Mathf.Clamp(volume, 0.0001f, 1);
    //     if (audioMixer != null)
    //     {
    //         audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
    //     }
    // }
    
    public void ActivateGameVolume()
    {
        gameVolumeEnabled = !gameVolumeEnabled;
        SetGameVolume();
    }
    
    public void ActivateMusicVolume()
    {
        musicVolumeEnabled = !musicVolumeEnabled;
        SetMusicVolume();
    }
    
    public void SetGameVolume()
    {
        audioMixer.SetFloat("GameVolume", gameVolumeEnabled ? Mathf.Log10(gameVolume) * 20 : -80f);
        gameVolumeButton.color = gameVolumeEnabled ? Color.white : disabledColor;
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicVolumeEnabled ? Mathf.Log10(musicVolume) * 20 : -80f);
        musicVolumeButton.color = musicVolumeEnabled ? Color.white : disabledColor;
    }
}
