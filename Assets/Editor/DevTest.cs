using UnityEditor;

public class DevTest
{
    [MenuItem("Tools/DevTest/GainXP")]
    public static void AddXP()
    {
        GM.Instance.player.AddXP(300);
    }

    [MenuItem("Tools/DevTest/Save")]
    public static void Save()
    {
        GM.Instance.SaveGame();
    }

    [MenuItem("Tools/DevTest/DeleteSave")]
    public static void DeleteSave()
    {
        SaveScript.DeleteSave();
    }
  
    [MenuItem("Tools/DevTest/AddItemInventory")]
    public static void AddItemInventory()
    {
        GM.Instance.player.AddToInventory(GM.Instance.inhabitants[0], 1);
    }

    [MenuItem("Tools/DevTest/RemoveItemInventory")]
    public static void RemoveItemInventory()
    {
        GM.Instance.player.RemoveFromInventory(GM.Instance.inhabitants[0], 1);
    }

    [MenuItem("Tools/DevTest/SimFlouz")]
    public static void AddSimflouz()
    {
        GM.Instance.player.AddStar(1000);
    }
}
