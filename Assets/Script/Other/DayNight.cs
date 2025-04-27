using UnityEngine;
using LitMotion;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Drawing;
using Color = UnityEngine.Color;

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
    [SerializeField] private Texture2D dayCurtainTexture;    // ⬅️ Texture pour le jour
    [SerializeField] private Texture2D nightCurtainTexture;

    [Header("Music Settings")]
    [SerializeField] private AudioClip dayMusic;
    [SerializeField] private AudioClip nightMusic;
    [SerializeField] private AudioClip dayTransitionMusic;
    [SerializeField] private AudioClip nightTransitionMusic;
    [SerializeField] private AudioClip noneButtonSFX;
    
    private Coroutine activityErrorCoroutine;
    public Coroutine nightDreamTimeCoroutine;

    int notificationID = -1;


    private TextMeshProUGUI starText;



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
        // TEST TEXTURE
        //curtain.texture = isDay ?  nightCurtainTexture : dayCurtainTexture;
        
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
        
        Debug.Log(target.x);

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
            int lastWholeMinutes = Mathf.CeilToInt(TimeRemaining / 60f);
            UpdateSkipText(lastWholeMinutes);
            AddBSSkipFunctions();

            if (notificationID == -1)
            {
                string title = "The dawn is near!";
                string text = "Let’s start a new day together!";
                notificationID = NotificationManager.CreateNotification(title, text, TimeRemaining);
            }


            timeContainer.SetActive(true);
            while (TimeRemaining > 0f)
            {
                TimeRemaining -= Time.deltaTime;
                timeText.text = GM.Instance.DisplayFormattedTime(TimeRemaining);

                int currentWholeMinutes = Mathf.CeilToInt(TimeRemaining / 60f);
                if (GM.Instance.skipWithStarButton.gameObject.activeSelf)
                {
                    if (currentWholeMinutes != lastWholeMinutes)
                    {
                        lastWholeMinutes = currentWholeMinutes;
                        UpdateSkipText(currentWholeMinutes);
                    }
                }

                yield return null;
            }

            yield return null;
            nightDreamTimeCoroutine = null;
            timeText.text = GM.Instance.DisplayFormattedTime(0f); // Assure l'affichage à 00:00

            ChangeTime(); // Day automatique


            yield return new WaitForSeconds(1f);
            GM.DMM.ApplySelectedDreams(notificationID);
            timeContainer.SetActive(false);

        }
    }
    
    public bool IsDay => isDay;


    private void AddBSSkipFunctions()
    {
        Button starButton = GM.Instance.skipWithStarButton;

        if (starButton != null && starText != null)
        {
            starButton.onClick.RemoveAllListeners();
            starButton.onClick.AddListener(() =>
            {
                GM.Instance.TrySkipNightWithStars(starText);
            });
        }

        Button adButton = GM.Instance.skipWithAdButton;

        if (adButton != null)
        {
            adButton.onClick.RemoveAllListeners();
            adButton.onClick.AddListener(() =>
            {
                GM.Instance.SkipNightWithADS();
            });
        }
    }

    private void UpdateSkipText(int remainingMinutes)
    {
        if (starText == null)
        {
            starText = GM.Instance.skipWithStarButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        }
        if (starText == null) return;

        starText.text = remainingMinutes.ToString();

        UpdateStarBTNInteractable(remainingMinutes);
    }

    private void UpdateStarBTNInteractable(int stars)
    {
        Button starButton = GM.Instance.skipWithStarButton;
        if (GM.Instance.player.CanSpendStar(stars))
        {
            starButton.interactable = true;
        }
        else
        {
            starButton.interactable = false;
        }
    }



    IEnumerator RestartCoroutine()
    {
        yield return null;

        if (!isDay && nightDreamTimeCoroutine == null)
        {
            TimeSpan elapsedTime = System.DateTime.Now - GM.Instance.GetLastTimeSaved();
            TimeRemaining -= (float)elapsedTime.TotalSeconds;

            nightDreamTimeCoroutine = StartCoroutine(StartWaitingTime());
        }
    }


    #region Check Game closed
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if(nightDreamTimeCoroutine != null)
            {
                StopCoroutine(nightDreamTimeCoroutine);
                nightDreamTimeCoroutine = null;
            }
        }
        else
        {
            StartCoroutine(RestartCoroutine());
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (nightDreamTimeCoroutine != null)
            {
                StopCoroutine(nightDreamTimeCoroutine);
                nightDreamTimeCoroutine = null;
            }
        }
        else
        {
            StartCoroutine(RestartCoroutine());
        }
    }

    private void OnApplicationQuit()
    {
        if (nightDreamTimeCoroutine != null)
        {
            StopCoroutine(nightDreamTimeCoroutine);
            nightDreamTimeCoroutine = null;
        }
    }
    #endregion

}
