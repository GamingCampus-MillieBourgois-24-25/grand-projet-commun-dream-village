using System.Collections;
using System.Collections.Generic;
using LitMotion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingClouds : MonoBehaviour
{
    [SerializeField] List<CloudsPos> cloudsBack;
    [SerializeField] List<CloudsPos> cloudsFront;

    [SerializeField] List<ColorClouds> colors;


    public bool cloudOuting = true;
    [SerializeField] float minTimeWait = 0.5f;

    int cloudsNotFinished = 0;






    [System.Serializable]
    [SerializeField] protected class ColorClouds
    {
        [SerializeField] string name;
        [SerializeField] public List<Color> frontColors;
        [SerializeField] public List<Color> backColors;
    }

    [System.Serializable]
    [SerializeField] protected class CloudsPos
    {
        public GameObject cloud;

        public Vector3 startPos;
        public Vector3 endPos;
        public float duration;
        public float delay;

        public bool finished = false;
    }



    private void Awake()
    {
        int randomColorIndex = Random.Range(0, colors.Count);

        for (int i = 0; i < cloudsBack.Count; i++)
        {
            cloudsBack[i].cloud.GetComponent<Image>().color = colors[randomColorIndex].backColors[Random.Range(0, colors[randomColorIndex].backColors.Count)];
        }
        for (int i = 0; i < cloudsFront.Count; i++)
        {
            cloudsFront[i].cloud.GetComponent<Image>().color = colors[randomColorIndex].frontColors[Random.Range(0, colors[randomColorIndex].frontColors.Count)];
        }

        cloudsNotFinished = cloudsBack.Count + cloudsFront.Count;
    }



    private void Start()
    {
        // Démarrer la coroutine pour charger la scène
        StartCoroutine(LoadingScene());
    }



    private IEnumerator LoadingScene()
    {
        foreach (CloudsPos cloud in cloudsBack)
        {
            cloud.cloud.transform.localPosition = cloud.startPos;
            if (!cloudOuting)
                cloud.cloud.transform.localPosition = cloud.endPos;
        }
        foreach (CloudsPos cloud in cloudsFront)
        {
            cloud.cloud.transform.localPosition = cloud.startPos;
            if (!cloudOuting)
                cloud.cloud.transform.localPosition = cloud.endPos;
        }




        // Charger la scène "MainScene" de manière asynchrone
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        
        // Délai minimum d'une seconde
        yield return new WaitForSeconds(minTimeWait);

        // Attendre que le chargement soit terminé
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Debug lorsque le chargement est terminé
        foreach (CloudsPos cloud in cloudsBack)
        {
            CloudMovement(cloud);
        }
        foreach (CloudsPos cloud in cloudsFront)
        {
            CloudMovement(cloud);
        }

        // Attendre que tous les nuages aient terminé leur animation
        while (cloudsNotFinished > 0)
        {
            yield return null;
        }

        // Delete la scene de loading
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }


    private void CloudMovement(CloudsPos cloud)
    {
        Debug.Log("CloudMovement");

        Vector3 startPos = cloud.startPos;
        Vector3 endPos = cloud.endPos;
        if (!cloudOuting)
        {
            startPos = cloud.endPos;
            endPos = cloud.startPos;
        }

        LMotion.Create(startPos, endPos, cloud.duration)

            .WithEase(Ease.InOutSine)
            .WithDelay(cloud.delay)
            .WithOnComplete(OnCloudAnimationFinished)
            .Bind((Vector3 pos) =>
             {
                 cloud.cloud.transform.localPosition = pos;
             });
    }

    private void OnCloudAnimationFinished()
    {
        cloudsNotFinished--;
    }
}
