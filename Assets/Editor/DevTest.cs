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
}
