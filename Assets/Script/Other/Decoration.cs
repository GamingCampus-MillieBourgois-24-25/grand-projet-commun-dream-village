using UnityEngine;

[CreateAssetMenu(fileName = "Decoration", menuName = "ScriptableObjects/Decoration")]
public class Decoration : IScriptableElement
{
    #region Variable Serialized
    #region Basic Information
    [field: Header("Basic Information")]
    public override Player.ItemCategory Category => Player.ItemCategory.DecorationCategory;
    [field: SerializeField] public override string Name => base.Name;
    #endregion

    #region Economy
    [field: Header("Progression & Economy")]
    [field: SerializeField] public override int InitialPrice => base.InitialPrice;
    [field: SerializeField] public override int MaxOwned => base.MaxOwned;
    [field: SerializeField] public override int UnlockedAtLvl => base.UnlockedAtLvl;
    #endregion

    #region Visuals
    [field: Header("Visuals")]
    public override Sprite Icon => base.Icon;
    [field: SerializeField] public override GameObject InstantiatePrefab => base.InstantiatePrefab;
    #endregion
    #endregion
}
