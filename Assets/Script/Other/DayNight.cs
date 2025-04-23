using UnityEngine;
using LitMotion;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DayNight : MonoBehaviour
{
    [SerializeField] private bool isDay;
    
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text activityErrorText;

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

    [Header("Music Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    
    private Coroutine activityErrorCoroutine;

    // Start is called before the first frame update
    private void Awake()
    {
        sun.color = isDay ? dayColor : nightColor;
        sun.transform.rotation = Quaternion.Euler(isDay ? dayRotation : nightRotation);
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        musicSource.clip = isDay ? dayMusic : nightMusic;
        musicSource.Play();
    }

    private IEnumerator ShowActivityErrorText()
    {
        if (activityErrorText == null) yield break;

        CanvasGroup canvasGroup = activityErrorText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = activityErrorText.gameObject.AddComponent<CanvasGroup>();
        }

        activityErrorText.gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(2f);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        activityErrorText.gameObject.SetActive(false);
    }

    public void ChangeTime()
    {
        // Pour passer à la nuit
        if (isDay)
        {
            foreach (InhabitantInstance inhabitant in GM.VM.inhabitants)
            {
                if (inhabitant.isInActivity) // Pas passer en mode nuit si un habitant est en activité
                {
                    if (activityErrorCoroutine != null)
                    {
                        StopCoroutine(activityErrorCoroutine);
                    }
                    activityErrorCoroutine = StartCoroutine(ShowActivityErrorText());
                    return;
                }
            }
        }
        //Pour passer au jour
        else
        {
            // attendre le temps de rêve
        }
        
        isDay = !isDay;
        RectTransform transform = curtain.GetComponent<RectTransform>();
        Vector2 target = curtain.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        LMotion.Create(0, target.x, animationDuration)
            .WithEase(Ease.OutCubic).WithOnComplete(SwitchTime)
            .Bind(x =>
            {
                var rect = transform.rect;
                transform.sizeDelta = new Vector2(x, rect.height);
            });
        if(isDay)
        {
            GM.Cjm.CheckStatsAndHandleDeparture();
            GM.Cjm.CheckForHeartBonus();
            Debug.Log("Daytime: Checking stats and handling departure.");

            LMotion.Create(0, target.x, animationDuration)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() =>
                {
                    GM.DreamPanel.SetActive(false);
                    GM.JournalPanel.SetActive(true);
                    GM.ShopPanel.SetActive(true);
                    GM.InventoryPanel.SetActive(true);
                })
                .Bind(x =>
                {
                    var rect = transform.rect;
                    transform.sizeDelta = new Vector2(x, rect.height);
                });
        }
        else
        {
            LMotion.Create(0, target.x, animationDuration)
                .WithEase(Ease.OutCubic)
                .WithOnComplete(() =>
                {
                    GM.DreamPanel.SetActive(true);
                    GM.JournalPanel.SetActive(false);
                    GM.ShopPanel.SetActive(false);
                    GM.InventoryPanel.SetActive(false);
                })
                .Bind(x =>
                {
                    var rect = transform.rect;
                    transform.sizeDelta = new Vector2(x, rect.height);
                });
        }
    }

    private void SwitchTime()
    {
        sun.color = isDay ? dayColor : nightColor;
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        sun.transform.rotation = Quaternion.Euler(isDay ? dayRotation : nightRotation);
        
        RectTransform transform = curtain.GetComponent<RectTransform>();
        Vector2 target = curtain.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        LMotion.Create(target.x, 0, animationDuration)
            .WithEase(Ease.OutCubic)
            .Bind(x =>
            {
                var rect = transform.rect;
                transform.sizeDelta = new Vector2(x, rect.height);
            });

        timeText.text = isDay ? "Night" : "Day";
        musicSource.clip = isDay ? dayMusic : nightMusic;
        musicSource.Play();
    }

    
    public bool IsDay => isDay;

}
