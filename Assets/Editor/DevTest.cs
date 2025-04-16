using UnityEditor;
using UnityEngine;

public class DevTest
{
    [MenuItem("Tools/DevTest/GainXP")]
    public static void AddXP()
    {
        GM.Instance.player.AddXP(300);
    }
}
