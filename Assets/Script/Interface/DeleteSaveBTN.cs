using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteSaveBTN : MonoBehaviour
{
    public void BS_DeleteSave()
    {
        StartCoroutine(GM.Instance.DeleteSaveCoroutine());
    }
}
