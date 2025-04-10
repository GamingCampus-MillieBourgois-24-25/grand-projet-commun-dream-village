using System.Linq;
using TMPro;
using UnityEngine;

public class GraphicsOptions : MonoBehaviour
{
    public enum ShadowQualityParameter
    {
        Disable = ShadowQuality.Disable,
        HardOnly = ShadowQuality.HardOnly,
        All = ShadowQuality.All,
    }
    
    public enum VsyncParameter
    {
        Off = 0,
        On = 1,
        Adaptive = 2,
    }
    
    [Header("Graphics Settings")]
    public VsyncParameter vsync = VsyncParameter.On;
    public int targetFrameRate = 60;
    public Vector2Int fpsLimitRange = new Vector2Int(30, 60);
    public string[] qualitySettings;
    public string currentQualitySettings;

    [Header("Interface")] 
    public TMP_Text fpsText;
    
    private void Start()
    {
        QualitySettings.vSyncCount = (int)vsync;
        Application.targetFrameRate = targetFrameRate;
        fpsText.text = targetFrameRate.ToString();
        qualitySettings = QualitySettings.names;
        currentQualitySettings = qualitySettings[QualitySettings.GetQualityLevel()];
    }

    private void OnValidate()
    {
        QualitySettings.vSyncCount = (int)vsync;
        Application.targetFrameRate = targetFrameRate;
    }
    
    public void SetQualitySettings(int quality)
    {
        currentQualitySettings = qualitySettings[quality];
        QualitySettings.SetQualityLevel(qualitySettings.ToList().IndexOf(currentQualitySettings), false);
    }
    
    public void SetTargetFrameRate(float frameRate)
    {
        targetFrameRate = (int) Mathf.Lerp(fpsLimitRange.x, fpsLimitRange.y, frameRate);
        Application.targetFrameRate = targetFrameRate;
        fpsText.text = targetFrameRate.ToString();
    }
}
