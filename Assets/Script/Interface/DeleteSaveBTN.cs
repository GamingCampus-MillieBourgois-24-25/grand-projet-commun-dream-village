using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteSaveBTN : MonoBehaviour
{
    public void BS_DeleteSave()
    {
        StartCoroutine(BS_DeleteSaveCoroutine());
    }



    IEnumerator BS_DeleteSaveCoroutine()
    {
        LoadingClouds.cloudOuting = false;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SaveScript.DeleteSave();
    }
}
