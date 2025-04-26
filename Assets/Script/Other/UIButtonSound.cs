using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        GM.SM.PlaySFX(GM.SM.defaultClickSound);
    }
}

