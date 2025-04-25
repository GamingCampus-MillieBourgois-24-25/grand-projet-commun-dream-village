using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSkip : MonoBehaviour
{

    public void BS_OpenSkipCanvas()
    {
        GM.Instance.chooseSkipCanvas.gameObject.SetActive(true);
    }

    public void BS_CloseSkipCanvas()
    {
        GM.Instance.chooseSkipCanvas.gameObject.SetActive(false);
    }
}
