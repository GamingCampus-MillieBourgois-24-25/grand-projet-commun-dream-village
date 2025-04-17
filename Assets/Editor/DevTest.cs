using UnityEditor;

public class DevTest
{
    [MenuItem("Tools/DevTest/GainXP")]
    public static void AddXP()
    {
        GM.Instance.player.AddXP(300);
    }

    [MenuItem("Tools/DevTest/AddItemInventory")]
    public static void AddItemInventory()
    {
        GM.Instance.player.AddToInventory(GM.Instance.inhabitants[0], 1, GM.Instance.player.InhabitantInventory);
    }
}
