using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Decoration", menuName = "ScriptableObjects/Decoration")]
public class Decoration : IScriptableElement
{
    public override Player.ItemCategory Category => Player.ItemCategory.DecorationCategory;
}
