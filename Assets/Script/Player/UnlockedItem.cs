using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockedItem : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;

    public void SetItemContent(Sprite _icon, string _name)
    {
        itemIcon.sprite = _icon;
        itemName.text = _name;
    }
}
