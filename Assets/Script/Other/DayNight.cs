using UnityEngine;
using LitMotion;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class DayNight : MonoBehaviour
{
    [SerializeField] public bool isDay;

    [SerializeField] private Sprite daySprite;
    [SerializeField] private Sprite nightSprite;
    [SerializeField] private TMP_Text activityErrorText;

    [Header("NightTimer")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject timeContainer;
    [SerializeField] private Image dayNightButton;
    public float TimeRemaining = 0f;

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
    [SerializeField] private Color dayCurtainColor = new Color(0.5f, 0.7f, 1f, 1f); // Bleu clair
    [SerializeField] private Color nightCurtainColor = new Color(0.1f, 0.1f, 0.3f, 1f); // Bleu foncé/violet

    [Header("Music Settings")]
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private AudioClip dayTransitionMusic;
    [SerializeField] private AudioClip nightTransitionMusic;
    [SerializeField] private AudioClip noneButtonSFX;
    
    private Coroutine activityErrorCoroutine;
    public Coroutine nightDreamTimeCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        sun.color = isDay ? dayColor : nightColor;
        sun.transform.rotation = Quaternion.Euler(isDay ? dayRotation : nightRotation);
        RenderSettings.skybox = isDay ? daySkybox : nightSkybox;
        if (TimeRemaining > 0f)
        {
            TimeSpan elapsedTime = System.DateTime.Now - GameManager.instance.GetLastTimeSaved();
            TimeRemaining -= (float)elapsedTime.TotalSeconds;

            nightDreamTimeCoroutine = StartCoroutine(StartWaitingTime());
        }
        else
        {
            timeContainer.SetActive(false);
        }
        dayNightButton.sprite = isDay ? nightSprite : daySprite;
        if (isDay)
        {
            GM.SM.PlayMusic(dayMusic, true);
        }
        else
        {
            GM.SM.PlayMusic(nightMusic, true);
        }
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
                    GM.SM.PlaySFX(noneButtonSFX);
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
            if (nightDreamTimeCoroutine != null) // attendre le temps de rêve
            {
                return;
            }
            
        }
        
        isDay = !isDay;
        TimeRemaining = 0f;

        GM.Instance.SaveGame();

        RectTransform transform = curtain.GetComponent<RectTransform>();
        Vector2 target = curtain.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        LMotion.Create(curtain.color, isDay ? dayCurtainColor : nightCurtainColor, animationDuration)
            .WithEase(Ease.OutCubic)
            .Bind(color => curtain.color = color);

        LMotion.Create(0, target.x, animationDuration)
            .WithEase(Ease.OutCubic).WithOnComplete(SwitchTime)
            .Bind(x =>
            {
                var rect = transform.rect;
                transform.sizeDelta = new Vector2(x, rect.height);
            });
        if (timeContainer.activeSelf)
        {
            timeContainer.SetActive(false);
        }
        if (isDay)
        {
            GM.SM.PlayMusic(dayTransitionMusic, false, () =>
            {
                GM.SM.PlayMusic(dayMusic, true);
            });
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
            GM.SM.PlayMusic(nightTransitionMusic, false, () =>
            {
                GM.SM.PlayMusic(nightMusic, true);
            });
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

        dayNightButton.sprite = isDay ? nightSprite : daySprite;
        //timeText.text = isDay ? "Night" : "Day";

        /*if (isDay)
        {
            GM.SM.PlayMusic(dayMusic, true);
        }
        else
        {
            GM.SM.PlayMusic(nightMusic, true);
        }*/
    }

    public IEnumerator StartWaitingTime()
    {
        GM.Instance.SaveGame();
        if (nightDreamTimeCoroutine == null)
        {
            timeContainer.SetActive(true);
            while (TimeRemaining > 1f)
            {
                TimeRemaining -= Time.deltaTime;
                timeText.text = GM.Instance.DisplayFormattedTime(TimeRemaining);
                yield return null;
            }

            nightDreamTimeCoroutine = null;
            timeText.text = GM.Instance.DisplayFormattedTime(0f); // Assure l'affichage à 00:00
            ChangeTime(); // Day automatique

            yield return new WaitForSeconds(1f);
            GM.DMM.ApplySelectedDreams();
            timeContainer.SetActive(false);
        }
    }
    
    public bool IsDay => isDay;

}
