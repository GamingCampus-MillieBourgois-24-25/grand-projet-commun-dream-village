using System;
using System.Collections;
using System.Collections.Generic;
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
    public ShadowQualityParameter shadowQuality = ShadowQualityParameter.All;
    public VsyncParameter vsync = VsyncParameter.On;
    public int targetFrameRate = 60;
    public Vector2Int fpsLimitRange = new Vector2Int(30, 60);
    
    private void Start()
    {
        QualitySettings.shadows = (ShadowQuality)shadowQuality;
        QualitySettings.vSyncCount = (int)vsync;
        Application.targetFrameRate = targetFrameRate;
    }

    private void OnValidate()
    {
        QualitySettings.shadows = (ShadowQuality)shadowQuality;
        QualitySettings.vSyncCount = (int)vsync;
        Application.targetFrameRate = targetFrameRate;
    }
    
    public void SetShadowQuality(int _quality)
    {
        shadowQuality = (ShadowQualityParameter)_quality;
        QualitySettings.shadows = (ShadowQuality)shadowQuality;
    }
    
    public void SetTargetFrameRate(float _frameRate)
    {
        targetFrameRate = (int) Mathf.Lerp(fpsLimitRange.x, fpsLimitRange.y, _frameRate);
        Application.targetFrameRate = targetFrameRate;
    }
}
