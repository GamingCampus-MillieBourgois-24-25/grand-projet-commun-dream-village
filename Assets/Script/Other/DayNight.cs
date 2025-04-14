using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    [SerializeField] private bool isDay;
    
    [SerializeField] private TMP_Text timeText;

    [Header("Light Parameters")]
    [SerializeField] private Light sun;
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;
    [SerializeField] private Vector3 dayRotation;
    [SerializeField] private Vector3 nightRotation;
    
    [Header("Skybox Parameters")]
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    
    [Header("Curtain Parameters")]
    [SerializeField] private RawImage curtain;
    [SerializeField] private float animationDuration;
    [SerializeField] private Vector3 hiddenPosition;
    [SerializeField] private Vector3 shownPosition;

    // Start is called before the first frame update
    private void Awake()
    {
        sun.color = isDay ? dayColor : nightColor;
        sun.transform.rotation = Quaternion.Euler(isDay ? dayRotation : nightRotation);
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
    }
    
    public void ChangeTime()
    {
        isDay = !isDay;
        RectTransform transform = curtain.GetComponent<RectTransform>();
        LMotion.Create(hiddenPosition.x, shownPosition.x, animationDuration)
            .WithEase(Ease.OutCubic).WithOnComplete(SwitchTime)
            .Bind(x =>
            {
                var rect = transform.rect;
                transform.sizeDelta = new Vector2(x, rect.height);
            });
    }

    private void SwitchTime()
    {
        sun.color = isDay ? dayColor : nightColor;
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        sun.transform.rotation = Quaternion.Euler(isDay ? dayRotation : nightRotation);
        RectTransform transform = curtain.GetComponent<RectTransform>();
        LMotion.Create(shownPosition.x, hiddenPosition.x, animationDuration)
            .WithEase(Ease.OutCubic)
            .Bind(x =>
            {
                var rect = transform.rect;
                transform.sizeDelta = new Vector2(x, rect.height);
            });

        timeText.text = isDay ? "Day" : "Night";
    }
}
