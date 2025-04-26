using UnityEditor;
using UnityEngine;

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

    [MenuItem("Tools/DevTest/AddStars")]
    public static void AddSimflouz()
    {
        GM.Instance.player.AddStar(2000);
    }

    [MenuItem("Tools/Tutorials/PlayAllTuto")]
    public static void PlayAllTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.PlayAllTutorials();
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayHouseTuto")]
    public static void PlayHouseTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.House);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayActivityTuto")]
    public static void PlayActivityTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Activity);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayDreamTuto")]
    public static void PlayDreamTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Dream);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayShopTuto")]
    public static void PlayShopTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Shop);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayEditTuto")]
    public static void PlayEditTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Edit);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }

    [MenuItem("Tools/Tutorials/PlayHeartTuto")]
    public static void PlayHeartTuto()
    {
        TutorialsManager tutorialsManager = GM.Tm;
        if (tutorialsManager != null)
        {
            tutorialsManager.GetTutoDialogues(Dialogues.TutorialType.Heart);
        }
        else
        {
            Debug.LogError("TutorialsManager not found in the scene.");
        }
    }
}
